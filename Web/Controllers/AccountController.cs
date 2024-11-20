using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action(nameof(LoginCallback), "Account");
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
            };

            return Challenge(authenticationProperties);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> LoginCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (authResult?.Succeeded != true)
            {
                // Handle failed authentication
                return RedirectToAction("Login");
            }

            //User.Identity.Name = "";

            // Get the access token and refresh token
            var accessToken = authResult.Properties.GetTokenValue("access_token");
            var refreshToken = authResult.Properties.GetTokenValue("refresh_token");

            // Save the tokens to the user's session or database
            HttpContext.Session.SetString("access_token", accessToken);
            HttpContext.Session.SetString("refresh_token", refreshToken);

            // Redirect the user to the desired page
            return RedirectToAction("LoginSuccess", "Claims");
        }
        
        

    }
}
