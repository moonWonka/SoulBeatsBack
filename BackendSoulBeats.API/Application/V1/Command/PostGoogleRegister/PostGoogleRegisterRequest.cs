using System.ComponentModel.DataAnnotations;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostGoogleRegister
{
    /// <summary>
    /// Representa la solicitud para registrar/actualizar un usuario autenticado con Google.
    /// El usuario ya está autenticado con Firebase, solo necesitamos crear/actualizar el registro en nuestra BD.
    /// </summary>
    public class PostGoogleRegisterRequest : IRequest<PostGoogleRegisterResponse>
    {
        /// <summary>
        /// Email del usuario obtenido desde Google
        /// </summary>
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        /// <summary>
        /// Firebase UID del usuario autenticado
        /// </summary>
        [Required]
        public string FirebaseUid { get; set; }

        /// <summary>
        /// Nombre para mostrar del usuario obtenido desde Google
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// URL de la foto de perfil obtenida desde Google
        /// </summary>
        public string? PhotoURL { get; set; }
    }
}