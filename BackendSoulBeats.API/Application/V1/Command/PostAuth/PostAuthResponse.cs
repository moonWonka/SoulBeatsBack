using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using System.Security.Claims;

namespace BackendSoulBeats.API.Application.V1.Command.PostAuth
{
    public class PostAuthResponse : BaseResponse
    {
        /// <summary>
        /// Indica si la autenticación fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado de la autenticación.
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Token de acceso generado tras la autenticación (si aplica).
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Tiempo de expiración del token (en segundos).
        /// </summary>
        public int? ExpiresIn { get; set; }
    }
}