using BackendSoulBeats.Domain.Application.V1.Services;
using Microsoft.Extensions.Configuration;
using FirebaseAdmin.Auth;

namespace BackendSoulBeats.Infra.Application.V1.Services
{
    /// <summary>
    /// Implementación del servicio de autenticación de Google.
    /// </summary>
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
 
        public async Task<string> RegisterUserAsync(string email, string password)
        {
            // UserRecordArgs userArgs = new()
            // {
            //     Email = email,
            //     Password = password
            // };

            // UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
            var userRecord = "123456789"; // Simulación de creación de usuario
            return userRecord;
        }

        /// <summary>
        /// Verifica si un usuario ya está registrado en el sistema de Google.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <returns>Un valor booleano que indica si el usuario ya está registrado.</returns>
        public async Task<bool> IsUserRegisteredAsync(string email)
        {
            // Simulación de que el usuario no existe.
            return await Task.FromResult(false);
        }
    }
}