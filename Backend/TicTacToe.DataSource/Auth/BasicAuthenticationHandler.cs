using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TicTacToe.Application.Interfaces;

namespace TicTacToe.DataSource.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthService _authService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthService authService)
            : base(options, logger, encoder, clock)
        {
            _authService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Логируем путь запроса
            Logger.LogInformation("HandleAuthenticateAsync called for path: {Path}", Request.Path);

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Logger.LogWarning("Missing Authorization Header");
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (authHeader.Scheme != "Basic")
                {
                    Logger.LogWarning("Invalid scheme: {Scheme}", authHeader.Scheme);
                    return AuthenticateResult.Fail("Invalid Scheme");
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

                if (credentials.Length != 2)
                {
                    Logger.LogWarning("Invalid Authorization Header format");
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }

                var login = credentials[0];
                var password = credentials[1];

                Logger.LogInformation("Attempting to authenticate user: {Login}", login);

                var userId = await _authService.AuthenticateAsync(login, password);

                if (userId == null)
                {
                    Logger.LogWarning("Invalid login or password for user: {Login}", login);
                    return AuthenticateResult.Fail("Invalid Login or Password");
                }

                Logger.LogInformation("Authentication successful for user: {Login}, UserId: {UserId}", login, userId);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, login)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception during authentication");
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}