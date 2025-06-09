namespace BackendSoulBeats.Domain.Application.V1.Services
{
    public interface IGoogleAuthService
    {
        /// <summary>
        /// Registra un nuevo usuario utilizando Firebase Authentication.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <returns>El UID del usuario registrado en Firebase.</returns>
        Task<string> RegisterUserAsync(string email, string password);

        /// <summary>
        /// Verifica si un usuario ya está registrado en el sistema de Google.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <returns>Un valor booleano que indica si el usuario ya está registrado.</returns>
        Task<bool> IsUserRegisteredAsync(string email);
    }

}