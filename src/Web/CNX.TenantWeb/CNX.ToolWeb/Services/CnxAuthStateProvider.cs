using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CNX.ToolWeb.Services;

/// <summary>
/// Qu?n lý tr?ng thái ??ng nh?p cho ?ng d?ng qu?n tr?.
/// L?u phiên ??ng nh?p vào ProtectedLocalStorage ?? gi? tr?ng thái khi t?i l?i trang.
/// </summary>
public sealed class CnxAuthStateProvider : AuthenticationStateProvider
{
    private const string StorageKey = "cnx_auth_user";

    // Tài kho?n qu?n tr? m?c ??nh.
    private const string AdminUserName = "admin";
    private const string AdminPassword = "Chucpc12!";

    private readonly ProtectedLocalStorage _localStorage;
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CnxAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>L?y tr?ng thái xác th?c hi?n t?i t? b? nh? trình duy?t.</summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<string>(StorageKey);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
            {
                return BuildState(result.Value);
            }
        }
        catch
        {
            // B? qua l?i truy c?p storage trong giai ?o?n prerender.
        }

        return Anonymous;
    }

    /// <summary>Th?c hi?n ??ng nh?p, tr? v? true n?u thành công.</summary>
    public async Task<bool> LoginAsync(string userName, string password)
    {
        if (!string.Equals(userName?.Trim(), AdminUserName, StringComparison.OrdinalIgnoreCase)
            || password != AdminPassword)
        {
            return false;
        }

        await _localStorage.SetAsync(StorageKey, AdminUserName);
        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(AdminUserName)));
        return true;
    }

    /// <summary>??ng xu?t kh?i h? th?ng.</summary>
    public async Task LogoutAsync()
    {
        await _localStorage.DeleteAsync(StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static AuthenticationState BuildState(string userName)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, "Administrator")
        }, authenticationType: "CnxAuth");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
