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

            // Si no hay header de autorización, continuar sin autenticar
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                // Construir Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    new Claim("firebase_uid", decodedToken.Uid)
                };

                if (decodedToken.Claims.TryGetValue("email", out var email) && email != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? string.Empty));
                }

                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);

                await _next(context);
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning("Token de Firebase inválido: {Error}", ex.Message);

                // Crear un objeto de respuesta simple
                var errorResponse = new
                {
                    error = "unauthorized",
                    message = "Token de Firebase inválido",
                    details = ex.Message
                };

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                // Configurar opciones para evitar referencias cíclicas
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse, options));
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en autenticación de Firebase");

                var errorResponse = new
                {
                    error = "internal_server_error",
                    message = "Error interno del servidor"
                };

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse, options));
                return;
            }
        }
    }

    // Extensión para facilitar el registro del middleware
    public static class FirebaseAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseFirebaseAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FirebaseAuthenticationMiddleware>();
        }
    }
}