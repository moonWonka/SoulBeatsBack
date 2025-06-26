using System.ComponentModel.DataAnnotations;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostRegister
{
    /// <summary>
    /// Representa la solicitud para registrar un nuevo usuario en el sistema.
    /// Contiene los datos necesarios para el registro, como el encabezado y el cuerpo de la solicitud.
    /// </summary>
    public class PostRegisterRequest : IRequest<PostRegisterResponse>
    {
        // /// <summary>
        // /// Encabezado de la solicitud, que puede incluir información adicional como tokens o metadatos.
        // /// </summary>
        // internal HeaderViewModel Header { get; set; }

        // /// <summary>
        // /// Constructor vacío para inicializar la solicitud sin parámetros.
        // /// </summary>
        // public PostRegisterRequest() { }

        // /// <summary>
        // /// Constructor que inicializa la solicitud con un encabezado.
        // /// </summary>
        // /// <param name="header">Encabezado de la solicitud.</param>
        // public PostRegisterRequest(HeaderViewModel header)
        // {
        //     Header = header;
        // }        /// <summary>
        /// Email del usuario para el registro
        /// </summary>
        [Required]
        public string UserEmail { get; set; }

        /// <summary>
        /// Contraseña del usuario para el registro
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Nombre para mostrar del usuario (opcional)
        /// </summary>
        public string? DisplayName { get; set; }
    }
}