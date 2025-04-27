using System.Text.Json.Serialization;

namespace BackendSoulBeats.API.Application.V1.ViewModel.Common
{
    public class BaseResponse
    {
        /// <summary>
        /// Código de estado HTTP.
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Descripción de la respuesta.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Mensaje amigable para el usuario.
        /// </summary>
        [JsonPropertyName("userFriendly")]
        public string UserFriendly { get; set; }

        /// <summary>
        /// Información adicional para detalles.
        /// </summary>
        [JsonPropertyName("moreInformation")]
        public string MoreInformation { get; set; }


        public BaseResponse()
        {
            StatusCode = 200; // Código de estado predeterminado (OK).
            Description = string.Empty; // Descripción inicial vacía.
            UserFriendly = string.Empty; // Mensaje amigable inicial vacío.
            MoreInformation = string.Empty; // Información adicional inicial vacía.
        }
    }
}
