using System.Net;
using BackendSoulBeats.API.Application.V1.Command.PostRegister;
using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registro de un nuevo usuario con email y contraseña.
        /// Este endpoint permite registrar un usuario en el sistema proporcionando un email y una contraseña.
        /// </summary>
        /// <param name="request">Datos necesarios para el registro del usuario (email y contraseña).</param>
        /// <returns>Devuelve un token JWT y los datos del usuario o la respuesta de error correspondiente.</returns>
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PostRegisterResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RegisterUserWithEmailAndPassword(
            [FromBody] PostRegisterRequest request) // Cambiar a [FromBody] ya que los datos de email y contraseña suelen enviarse en el cuerpo
        {
            // Se envía la solicitud al handler a través de MediatR
            var response = await _mediator.Send(request);

            // Se devuelve la respuesta con el código de estado indicado en la propiedad StatusCode
            return response.StatusCode switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
