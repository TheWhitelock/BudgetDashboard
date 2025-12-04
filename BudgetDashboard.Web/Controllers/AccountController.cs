using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BudgetDashboard.Data.Entities;

[Route("Account")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string Email, string Password, string ReturnUrl = "/")
    {
        var result = await _signInManager.PasswordSignInAsync(
            Email,
            Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return LocalRedirect(ReturnUrl ?? "/");
        }

        TempData["LoginError"] = "Invalid login credentials.";
        return LocalRedirect($"/Account/Login?ReturnUrl={Uri.EscapeDataString(ReturnUrl ?? "/")}");
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect("/");
    }
}
