namespace BudgetDashboard.Web.Constants;

/// <summary>
/// Contains centralized route constants used throughout the application.
/// This prevents hardcoded strings and ensures consistency across the codebase.
/// </summary>
public static class RouteConstants
{
    /// <summary>
    /// Authentication and account routes.
    /// </summary>
    public static class Account
    {
        public const string LoginPath = "/Account/Login";
        public const string RegisterPath = "/Account/Register";
        public const string LogoutPath = "/Account/Logout";
    }

    /// <summary>
    /// API endpoint routes for authentication.
    /// </summary>
    public static class Api
    {
        public const string AuthLoginEndpoint = "/api/auth/login";
        public const string AuthLogoutEndpoint = "/api/auth/logout";
    }

    /// <summary>
    /// Main application routes.
    /// </summary>
    public static class Pages
    {
        public const string Dashboard = "/dashboard";
        public const string Home = "/";
    }
}
