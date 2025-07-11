using System.Net;
using BackendSoulBeats.API.Application.V1.Query;
using BackendSoulBeats.API.Application.V1.Command.UpdateUserProfile;
using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Controllers{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("User")]
    [Authorize(Policy = "FirebaseAuthenticated")] // Aplica la política de autorización
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene el perfil detallado del usuario incluyendo sus preferencias musicales y estadísticas.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <param name="header">Metadatos de control para la petición.</param>
        /// <remarks>
        /// Example request:
        /// ```json
        /// {
        ///   "userId": "123456789",
        ///   "profile": {
        ///     "username": "MusicLover2024",
        ///     "fullName": "John Doe",
        ///     "age": 25,
        ///     "location": "Temuco, Chile",
        ///     "bio": "Amante del rock alternativo y jazz experimental"
        ///   },
        ///   "musicPreferences": {
        ///     "favoriteGenres": ["Rock Alternativo", "Jazz", "Indie"],
        ///     "favoriteArtists": [
        ///       {
        ///         "name": "Arctic Monkeys",
        ///         "genre": "Rock Alternativo",
        ///         "matchScore": 95
        ///       }
        ///     ],
        ///     "topTracks": [
        ///       {
        ///         "title": "Do I Wanna Know?",
        ///         "artist": "Arctic Monkeys",
        ///         "playCount": 150
        ///       }
        ///     ]
        ///   },
        ///   "statistics": {
        ///     "totalListeningTime": "350h",
        ///     "favoriteTimeToListen": "21:00-23:00",
        ///     "matchingPreference": {
        ///       "genreImportance": "high",
        ///       "artistOverlap": "medium"
        ///     }
        ///   },
        ///   "status": "success",
        ///   "timestamp": "2024-05-04T15:30:00Z"
        /// }
        /// ```
        /// </remarks>
        /// <returns>
        /// Códigos de estado posibles:
        /// - 200: Información del perfil recuperada exitosamente
        /// - 400: Solicitud inválida o parámetros incorrectos
        /// - 401: Usuario no autenticado
        /// - 500: Error interno del servidor
        /// </returns>
        [HttpGet("{id}/info")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetUserInfoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetIUser([FromRoute] string id)
        {
            GetUserInfoRequest request = new() { UserId = id };            
            // Puedes obtener el UID del usuario autenticado así:
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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

        /// <summary>
        /// Actualiza la información completa del perfil del usuario.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <param name="request">Datos del perfil a actualizar.</param>
        /// <remarks>
        /// Example request:
        /// ```json
        /// {
        ///   "userId": "firebase-uid-123",
        ///   "displayName": "Juan Pérez",
        ///   "email": "juan.perez@email.com",
        ///   "age": 25,
        ///   "bio": "Amante del rock y jazz experimental",
        ///   "favoriteGenres": "Rock, Jazz, Indie",
        ///   "profilePictureUrl": "https://example.com/profile.jpg"
        /// }
        /// ```
        /// </remarks>
        /// <returns>
        /// Códigos de estado posibles:
        /// - 200: Perfil actualizado exitosamente
        /// - 400: Solicitud inválida o parámetros incorrectos
        /// - 401: Usuario no autenticado
        /// - 404: Usuario no encontrado
        /// - 500: Error interno del servidor
        /// </returns>
        [HttpPut("{id}/profile")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateUserProfileResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(UpdateUserProfileResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(UpdateUserProfileResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(UpdateUserProfileResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(UpdateUserProfileResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile([FromRoute] string id, [FromBody] UpdateUserProfileRequest request)
        {

            // Se envía la solicitud al handler a través de MediatR
            var response = await _mediator.Send(request);

            // Se devuelve la respuesta con el código de estado indicado en la propiedad StatusCode
            return response.StatusCode switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.NotFound => NotFound(response),
                (int)HttpStatusCode.Unauthorized => Unauthorized(response),
                (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}