using BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange;
using BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists;
using BackendSoulBeats.API.Application.V1.Query.GetSpotifyStatus;
using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using BackendSoulBeats.API.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BackendSoulBeats.API.Application.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("Spotify")]
    [Authorize(Policy = "FirebaseAuthenticated")]
    [Produces("application/json")]
    public class SpotifyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SpotifyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Intercambia el código de autorización de Spotify por tokens de acceso
        /// </summary>
        /// <param name="request">Código de autorización y datos de redirección</param>
        /// <returns>Respuesta indicando si la conexión fue exitosa</returns>
        [HttpPost("token-exchange")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PostSpotifyTokenExchangeResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ExchangeSpotifyToken([FromBody] PostSpotifyTokenExchangeRequest request)
        {

            if (request == null)
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Description = "INVALID_REQUEST",
                    UserFriendly = "Invalid request data"
                });
            }
            
            var response = await _mediator.Send(request);
            
            // Devolver la respuesta con el código de estado indicado en la propiedad StatusCode
            return response.StatusCode switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        /// <summary>
        /// Obtiene las playlists del usuario desde Spotify
        /// </summary>
        /// <param name="limit">Número máximo de playlists a obtener (por defecto 20)</param>
        /// <param name="offset">Número de playlists a omitir (por defecto 0)</param>
        /// <returns>Lista de playlists del usuario</returns>
        [HttpGet("playlists")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetSpotifyPlaylistsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSpotifyPlaylists([FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            // Obtener el UID del usuario autenticado desde Firebase
            var firebaseUid = HttpContext.Items["FirebaseUID"]?.ToString();
            
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new GetSpotifyPlaylistsResponse
                {
                    StatusCode = 401,
                    Description = "UNAUTHORIZED",
                    UserFriendly = "User not authenticated"
                });
            }

            var request = new GetSpotifyPlaylistsRequest
            {
                Limit = Math.Max(1, Math.Min(50, limit)), // Limitar entre 1 y 50
                Offset = Math.Max(0, offset),
                FirebaseUid = firebaseUid
            };

            // Enviar la solicitud al handler a través de MediatR
            var response = await _mediator.Send(request);
            
            // Devolver la respuesta con el código de estado indicado en la propiedad StatusCode
            return response.StatusCode switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        /// <summary>
        /// Obtiene el estado de conexión de Spotify del usuario
        /// </summary>
        /// <returns>Estado de la conexión y información del perfil si está conectado</returns>
        [HttpGet("status")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetSpotifyStatusResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSpotifyStatus()
        {
            // Obtener el UID del usuario autenticado desde Firebase
            var firebaseUid = HttpContext.Items["FirebaseUID"]?.ToString();
            
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new GetSpotifyStatusResponse
                {
                    StatusCode = 401,
                    Description = "UNAUTHORIZED",
                    UserFriendly = "User not authenticated",
                    IsConnected = false,
                    TokenValid = false
                });
            }

            var request = new GetSpotifyStatusRequest
            {
                FirebaseUid = firebaseUid
            };

            // Enviar la solicitud al handler a través de MediatR
            var response = await _mediator.Send(request);
            
            // Devolver la respuesta con el código de estado indicado en la propiedad StatusCode
            return response.StatusCode switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        /// <summary>
        /// Desconecta la cuenta de Spotify del usuario
        /// </summary>
        /// <returns>Respuesta indicando si la desconexión fue exitosa</returns>
        [HttpDelete("disconnect")]
        public async Task<ActionResult> DisconnectSpotify()
        {
            // Obtener el UID del usuario autenticado desde Firebase
            var firebaseUid = HttpContext.Items["FirebaseUID"]?.ToString();
            
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new
                {
                    Header = new
                    {
                        Code = "UNAUTHORIZED",
                        Message = "User not authenticated"
                    }
                });
            }

            // TODO: Implementar handler para desconectar Spotify
            // Por ahora retornamos not implemented
            return StatusCode(501, new
            {
                Header = new
                {
                    Code = "NOT_IMPLEMENTED",
                    Message = "Disconnect functionality not yet implemented"
                }
            });
        }
    }
}