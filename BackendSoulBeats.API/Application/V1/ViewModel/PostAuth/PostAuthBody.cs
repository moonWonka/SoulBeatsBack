using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendSoulBeats.API.Application.V1.ViewModel.PostAuth
{
    public class PostAuthBody
    {
        /// <summary>
        /// Proveedor social a utilizar.
        /// Valores permitidos: gmail, facebook.
        /// </summary>
        [Required]
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// Código de autorización recibido tras el login.
        /// </summary>
        [Required]
        [JsonPropertyName("authCode")]
        public string AuthCode { get; set; }
    }
}