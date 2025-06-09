using System.Net;
using BackendSoulBeats.API.Application.V1.Command.PostRegister;
using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("auth")]
    [Authorize(Policy = "AllowAnonymous")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registro/Autenticación con correo y contraseña.
        /// Este endpoint recibe el correo y la contraseña del cliente para registrar o autenticar al usuario en el sistema.
        /// </summary>
        /// <param name="request">Datos necesarios para la autenticación.</param>
        /// <returns>Devuelve un token JWT y los datos del usuario o la respuesta de error correspondiente.</returns>
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PostRegisterResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostRegister([FromBody] PostRegisterRequest request)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el registro/autenticación");

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }
    }
}
