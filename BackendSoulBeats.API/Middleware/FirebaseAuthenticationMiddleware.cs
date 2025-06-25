using FirebaseAdmin.Auth;
using System.Security.Claims;
using Microsoft.ApplicationInsights;

namespace BackendSoulBeats.API.Middleware
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;

        public FirebaseAuthenticationMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
        {
            _next = next;
            _telemetryClient = telemetryClient;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
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

                if (decodedToken.Claims.TryGetValue("email", out var email) && email != null && email.ToString() != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? string.Empty));
                }                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);

                // Tracking de autenticación exitosa
                _telemetryClient.TrackEvent("AuthenticationSuccess", new Dictionary<string, string>
                {
                    {"Middleware", "FirebaseAuthenticationMiddleware"},
                    {"UserId", decodedToken.Uid},
                    {"UserAgent", context.Request.Headers["User-Agent"].ToString()}
                });

                await _next(context);
            }
            catch (FirebaseAuthException ex)
            {
                // Tracking de excepciones de autenticación con Application Insights
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Middleware", "FirebaseAuthenticationMiddleware"},
                    {"AuthToken", token?.Substring(0, Math.Min(10, token.Length)) + "..."},
                    {"ErrorCode", ex.AuthErrorCode?.ToString() ?? "Unknown"},
                    {"ErrorMessage", ex.Message}
                });

                // Tracking del evento de autenticación fallida
                _telemetryClient.TrackEvent("AuthenticationFailed", new Dictionary<string, string>
                {
                    {"Middleware", "FirebaseAuthenticationMiddleware"},
                    {"ErrorCode", ex.AuthErrorCode?.ToString() ?? "Unknown"},
                    {"UserAgent", context.Request.Headers["User-Agent"].ToString()}
                });

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido");
            }
            catch (Exception ex)
            {
                // Tracking de excepciones generales
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Middleware", "FirebaseAuthenticationMiddleware"},
                    {"ExceptionType", ex.GetType().Name},
                    {"ErrorMessage", ex.Message}
                });

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error interno del servidor");
            }
        }
    }
}