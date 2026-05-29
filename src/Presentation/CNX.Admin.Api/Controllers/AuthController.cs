using CNX.Infrastructure.Data;
using CNX.Infrastructure.Data.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CNX.Admin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly MasterDbContext _masterDb;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        MasterDbContext masterDb)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _masterDb = masterDb;
    }

    /// <summary>
    /// Login qua Master DB → trả về token + danh sách tenants user có quyền truy cập.
    /// Client sau đó chọn tenant → gọi API với tenant ID để switch database.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { Message = "Email hoặc mật khẩu không đúng" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { Message = "Email hoặc mật khẩu không đúng" });

        // Lấy danh sách tenants mà user có quyền truy cập
        var availableTenants = await GetUserTenantsAsync(user);

        var token = GenerateJwtToken(user, request.TenantId);
        return Ok(new
        {
            Token = token,
            User = new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.IsSystemAdmin,
                user.TenantId
            },
            AvailableTenants = availableTenants
        });
    }

    /// <summary>
    /// Chọn tenant sau khi login - tạo token mới gắn với tenant đã chọn.
    /// </summary>
    [HttpPost("select-tenant/{tenantId}")]
    [Authorize]
    public async Task<IActionResult> SelectTenant(Guid tenantId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            return Unauthorized();

        // Verify user has access to this tenant
        var hasAccess = user.IsSystemAdmin || user.TenantId == tenantId
            || await _masterDb.TenantUsers.AnyAsync(tu => tu.UserId == userId && tu.TenantId == tenantId);

        if (!hasAccess)
            return Forbid();

        var tenant = await _masterDb.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);
        if (tenant == null)
            return NotFound(new { Message = "Tenant không tồn tại" });

        // Generate new token with tenant info
        var token = GenerateJwtToken(user, tenantId);
        return Ok(new
        {
            Token = token,
            Tenant = new
            {
                tenant.Id,
                tenant.CompanyName,
                tenant.Subdomain,
                tenant.TaxCode
            }
        });
    }

    [HttpPost("register")]
    [Authorize(Policy = "SystemAdmin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            IsSystemAdmin = request.IsSystemAdmin,
            TenantId = request.TenantId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });

        return Ok(new { Message = "Tạo tài khoản thành công", UserId = user.Id });
    }

    private async Task<List<TenantSummary>> GetUserTenantsAsync(ApplicationUser user)
    {
        if (user.IsSystemAdmin)
        {
            // System admin có quyền truy cập tất cả tenants
            return await _masterDb.Tenants
                .Where(t => !t.IsDeleted)
                .Select(t => new TenantSummary { Id = t.Id, CompanyName = t.CompanyName, Subdomain = t.Subdomain })
                .ToListAsync();
        }

        // User thường - lấy tenants qua TenantUser mapping hoặc TenantId trực tiếp
        var tenantIds = await _masterDb.TenantUsers
            .Where(tu => tu.UserId == user.Id)
            .Select(tu => tu.TenantId)
            .ToListAsync();

        if (user.TenantId.HasValue && !tenantIds.Contains(user.TenantId.Value))
            tenantIds.Add(user.TenantId.Value);

        return await _masterDb.Tenants
            .Where(t => tenantIds.Contains(t.Id) && !t.IsDeleted)
            .Select(t => new TenantSummary { Id = t.Id, CompanyName = t.CompanyName, Subdomain = t.Subdomain })
            .ToListAsync();
    }

    private string GenerateJwtToken(ApplicationUser user, Guid? tenantId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new("IsSystemAdmin", user.IsSystemAdmin.ToString().ToLower()),
        };

        var effectiveTenantId = tenantId ?? user.TenantId;
        if (effectiveTenantId.HasValue)
            claims.Add(new Claim("TenantId", effectiveTenantId.Value.ToString()));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Optional: Nếu biết trước tenant cần truy cập, truyền vào để gắn vào token ngay
    /// </summary>
    public Guid? TenantId { get; set; }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsSystemAdmin { get; set; } = false;
    public Guid? TenantId { get; set; }
}

public class TenantSummary
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
}
