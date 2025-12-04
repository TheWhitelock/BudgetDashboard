using BudgetDashboard.Web.Components;
using BudgetDashboard.Web.Constants;
using BudgetDashboard.Data;
using BudgetDashboard.Data.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Services
// ---------------------------------------------------------------------------

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

// EF
builder.Services.AddDbContext<BudgetContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<BudgetContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

// Cookie paths (still valid)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = RouteConstants.Account.LoginPath;
    options.AccessDeniedPath = RouteConstants.Account.LoginPath;
    options.LogoutPath = RouteConstants.Account.LogoutPath;
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// DB Migration
// ---------------------------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BudgetContext>();
    await db.Database.MigrateAsync();
}

// ---------------------------------------------------------------------------
// Middleware
// ---------------------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// ---------------------------------------------------------------------------
// API: LOGIN (JS handles redirect)
// ---------------------------------------------------------------------------

app.MapPost("/api/auth/login", async (
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    HttpContext http,
    [FromForm] string email,
    [FromForm] string password
) =>
{
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
        return Results.Json(new { success = false, error = "InvalidCredentials" });

    var result = await signInManager.PasswordSignInAsync(user, password, false, false);
    if (!result.Succeeded)
        return Results.Json(new { success = false, error = "InvalidCredentials" });

    return Results.Json(new { success = true, redirect = "/" });
})
.DisableAntiforgery();

// ---------------------------------------------------------------------------
// API: LOGOUT (JS handles redirect)
// ---------------------------------------------------------------------------

app.MapPost("/api/auth/logout", async (
    SignInManager<ApplicationUser> signInManager
) =>
{
    await signInManager.SignOutAsync();
    return Results.Json(new { success = true, redirect = "/Account/Login" });
})
.DisableAntiforgery();

// ---------------------------------------------------------------------------
// Blazor Root (ONLY ONE ENDPOINT)
// ---------------------------------------------------------------------------

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// DO NOT MAP ANYTHING ELSE
// - NO MapBlazorHub()
// - NO _Host
// - NO fallback files
// - NO controllers

app.Run();
