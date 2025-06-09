using FirebaseAdmin.Auth;
using System.Security.Claims;

namespace BackendSoulBeats.API.Middleware
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirebaseAuthenticationMiddleware> _logger;

        public FirebaseAuthenticationMiddleware(RequestDelegate next, ILogger<FirebaseAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                }

                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);

                await _next(context);
            }
            catch (FirebaseAuthException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido");
            }
        }
    }
}