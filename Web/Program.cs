using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var host = builder.Host;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Enable session support
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Set the session cookie to HttpOnly
    options.Cookie.IsEssential = true; // Required for some cases (GDPR compliance)

});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {

        options.Authority = "http://localhost:8080/realms/myrealm";
        options.RequireHttpsMetadata = false; // Allow HTTP for metadata
        options.ClientId = "myclient";
        options.ClientSecret = "HPae4v67dzPzCJajMW40mYlMpdACV2AI";
        options.ResponseType = OpenIdConnectResponseType.Code; // Use Authorization Code flow
        options.SaveTokens = true; // Save tokens in the authentication ticket
        options.GetClaimsFromUserInfoEndpoint = true; // Fetch claims from the UserInfo endpoint
        options.Scope.Add("openid");
        options.CallbackPath = "/login-callback"; // Update callback path
        options.SignedOutCallbackPath = "/logout-callback"; // Update signout callback path
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "http://localhost:8080/realms/myrealm",
            ValidAudience = "myclient",
        };

        // Add event handlers
        options.Events = new OpenIdConnectEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log or handle errors here
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                // Handle remote failures (like if Keycloak rejects the request)
                context.HandleResponse(); // Don't propagate the exception
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProvider = context =>
            {
                //context.ProtocolMessage.RedirectUri = "https://localhost:7132/"; // Ensure this is correct
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
