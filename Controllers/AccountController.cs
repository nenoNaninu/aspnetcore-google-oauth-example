using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OAuthSample.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Login()
    {
        var prop = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(LoginCallback))
        };

        return this.Challenge(prop);
    }

    public async Task<IActionResult> LoginCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return this.BadRequest();
        }

        var principal = authenticateResult.Principal;

        if (principal is null)
        {
            return this.BadRequest();
        }

        var message = $"name: {principal.FindFirst(ClaimTypes.Name)?.Value}, e-main: {principal.FindFirst(ClaimTypes.Email)?.Value}";
        _logger.Log(LogLevel.Information, message);

        return this.RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return this.RedirectToAction(nameof(HomeController.Index), "Home");
    }
}