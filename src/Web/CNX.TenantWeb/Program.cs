using Radzen;
using CNX.TenantWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Radzen UI services (Dialog, Notification, Tooltip, ContextMenu)
builder.Services.AddRadzenComponents();

// Dịch vụ menu & tab control.
builder.Services.AddSingleton<CNX.TenantWeb.Services.MenuService>();
builder.Services.AddScoped<CNX.TenantWeb.Services.TabService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
