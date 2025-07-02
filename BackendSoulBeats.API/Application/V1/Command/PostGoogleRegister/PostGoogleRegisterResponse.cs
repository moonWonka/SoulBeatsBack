using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Command.PostGoogleRegister
{
    /// <summary>
    /// Respuesta del endpoint de registro/actualización de usuario con Google.
    /// </summary>
    public class PostGoogleRegisterResponse : BaseResponse
    {
        /// <summary>
        /// Indica si el usuario era nuevo (true) o ya existía (false)
        /// </summary>
        public bool IsNewUser { get; set; }

        /// <summary>
        /// Firebase UID del usuario
        /// </summary>
        public string FirebaseUid { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nombre para mostrar del usuario
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Indica si el perfil del usuario está completo
        /// </summary>
        public bool ProfileComplete { get; set; }
    }
}