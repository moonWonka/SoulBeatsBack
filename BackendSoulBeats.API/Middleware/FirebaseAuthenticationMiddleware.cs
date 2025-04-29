using FirebaseAdmin.Auth;
using System.Security.Claims;

namespace BackendSoulBeats.API.Middleware
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public FirebaseAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                    // Construir Claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid)
                    };

                    if (decodedToken.Claims.TryGetValue("email", out var email) && email != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Email, email?.ToString() ?? string.Empty)); // Manejo de null
                    }

                    // Puedes agregar más claims según tus necesidades

                    var identity = new ClaimsIdentity(claims, "Firebase");
                    context.User = new ClaimsPrincipal(identity);

                    await _next(context);
                    return;
                }
                catch
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid Firebase token.");
                    return;
                }
            }
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("No Firebase token provided.");
        }
    }
}