using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using FirebaseAdmin.Auth;
using System.Security.Claims;
using Microsoft.ApplicationInsights;

namespace BackendSoulBeats.API.Middleware
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly TelemetryClient _telemetryClient;

        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            TelemetryClient telemetryClient)
            : base(options, logger, encoder, clock)
        {
            _telemetryClient = telemetryClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.NoResult();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    new Claim("firebase_uid", decodedToken.Uid)
                };

                if (decodedToken.Claims.TryGetValue("email", out var email) && email != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? string.Empty));
                }

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                // Tracking de autenticación exitosa
                _telemetryClient.TrackEvent("AuthenticationSuccess", new Dictionary<string, string>
                {
                    {"Handler", "FirebaseAuthenticationHandler"},
                    {"UserId", decodedToken.Uid},
                    {"UserAgent", Request.Headers["User-Agent"].ToString()}
                });

                return AuthenticateResult.Success(ticket);
            }
            catch (FirebaseAuthException ex)
            {
                // Tracking de excepciones de autenticación
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "FirebaseAuthenticationHandler"},
                    {"AuthToken", token?.Substring(0, Math.Min(10, token.Length)) + "..."},
                    {"ErrorCode", ex.AuthErrorCode?.ToString() ?? "Unknown"},
                    {"ErrorMessage", ex.Message}
                });

                return AuthenticateResult.Fail($"Firebase authentication failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Tracking de excepciones generales
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "FirebaseAuthenticationHandler"},
                    {"ExceptionType", ex.GetType().Name},
                    {"ErrorMessage", ex.Message}
                });

                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}