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
// API: REGISTER (JS handles redirect)
// ---------------------------------------------------------------------------

app.MapPost("/api/auth/register", async (
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    [FromForm] string email,
    [FromForm] string password,
    [FromForm] string confirmPassword
) =>
{
    // Validation
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        return Results.Json(new { success = false, error = "EmailAndPasswordRequired" });

    if (password != confirmPassword)
        return Results.Json(new { success = false, error = "PasswordsDoNotMatch" });

    if (password.Length < 6)
        return Results.Json(new { success = false, error = "PasswordTooShort" });

    // Check if user already exists
    var existingUser = await userManager.FindByEmailAsync(email);
    if (existingUser is not null)
        return Results.Json(new { success = false, error = "EmailAlreadyExists" });

    // Create user
    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);

    if (!result.Succeeded)
    {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Results.Json(new { success = false, error = errors });
    }

    // Auto sign in after registration
    await signInManager.SignInAsync(user, isPersistent: false);

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
