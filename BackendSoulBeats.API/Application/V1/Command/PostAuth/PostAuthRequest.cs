using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostAuth
{
    public class PostAuthRequest : IRequest<PostAuthResponse>
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