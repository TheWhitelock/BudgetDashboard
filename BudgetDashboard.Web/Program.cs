/// <summary>
/// Application startup and entry configuration for the ASP.NET Core Blazor Server application.
/// </summary>
using BudgetDashboard.Web.Components;
using BudgetDashboard.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Build the WebApplication with default configuration and logging.
// Register framework and application services:
// - Razor Components with interactive server component support.
// - Razor Pages and Server-side Blazor.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register Entity Framework Core DbContext (BudgetContext) configured to use SQLite
// with the connection string named "DefaultConnection".
builder.Services.AddDbContext<BudgetContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply any pending EF Core migrations at startup by creating a scoped service provider,
// resolving the BudgetContext, and calling Database.MigrateAsync().
// Migration application is awaited to ensure the database schema is up-to-date before handling requests.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BudgetContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline:
// - In non-development environments, use an exception handler with scoped error pages and HSTS.
// - Enforce HTTPS redirection and serve static files.
// - Enable antiforgery protection and routing.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
// app.UseStaticFiles();

app.UseRouting();

// Enable antiforgery when using forms/components that require it.
app.UseAntiforgery();

// Map static assets and Razor component endpoints and enable interactive server render mode for the root component (App).
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Notes:
// - Service registrations and middleware order are important for correct security and behavior
//   (e.g., static files before routing, antiforgery when using forms/components that require it).
// - Connection strings and environment settings are read from the application's configuration.
app.Run();
