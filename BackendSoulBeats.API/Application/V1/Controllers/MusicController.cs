using System.Net;
using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using BackendSoulBeats.API.Application.V1.Query.GetGenres;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("music")]
    [Authorize(Policy = "FirebaseAuthenticated")]
    [Produces("application/json")]
    public class MusicController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MusicController> _logger;

        public MusicController(IMediator mediator, ILogger<MusicController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los géneros musicales disponibles para selección.
        /// </summary>
        /// <returns>Lista de géneros musicales activos.</returns>
        [HttpGet("genres")]
        [ProducesResponseType(typeof(GetGenresResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetGenres()
        {
            try
            {
                var request = new GetGenresRequest();
                var response = await _mediator.Send(request);

                return response.StatusCode switch
                {
                    (int)HttpStatusCode.OK => Ok(response),
                    (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener géneros musicales");

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Obtiene artistas por género específico.
        /// </summary>
        /// <param name="genreId">ID del género musical.</param>
        /// <returns>Lista de artistas del género especificado.</returns>
        [HttpGet("genres/{genreId}/artists")]
        [ProducesResponseType(typeof(GetArtistsByGenreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArtistsByGenre([FromRoute] int genreId)
        {
            try
            {
                var request = new GetArtistsByGenreRequest { GenreId = genreId };
                var response = await _mediator.Send(request);

                return response.StatusCode switch
                {
                    (int)HttpStatusCode.OK => Ok(response),
                    (int)HttpStatusCode.BadRequest => BadRequest(response),
                    (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artistas por género: {GenreId}", genreId);

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Obtiene las preferencias musicales del usuario autenticado.
        /// </summary>
        /// <returns>Preferencias de géneros y artistas del usuario.</returns>
        [HttpGet("preferences")]
        [ProducesResponseType(typeof(GetUserPreferencesResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var firebaseUid = User.FindFirst("firebase_uid")?.Value;
                
                if (string.IsNullOrWhiteSpace(firebaseUid))
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        UserFriendly = "Sesión expirada. Por favor, inicie sesión nuevamente."
                    });
                }

                var request = new GetUserPreferencesRequest { FirebaseUid = firebaseUid };
                var response = await _mediator.Send(request);

                return response.StatusCode switch
                {
                    (int)HttpStatusCode.OK => Ok(response),
                    (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preferencias del usuario");

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Actualiza las preferencias de géneros musicales del usuario.
        /// </summary>
        /// <param name="request">Nuevas preferencias de géneros.</param>
        /// <returns>Confirmación de actualización.</returns>
        [HttpPut("preferences/genres")]
        [ProducesResponseType(typeof(UpdateGenrePreferencesResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateGenrePreferences([FromBody] UpdateGenrePreferencesRequest request)
        {
            try
            {
                var firebaseUid = User.FindFirst("firebase_uid")?.Value;
                
                if (string.IsNullOrWhiteSpace(firebaseUid))
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        UserFriendly = "Sesión expirada. Por favor, inicie sesión nuevamente."
                    });
                }

                request.FirebaseUid = firebaseUid;
                var response = await _mediator.Send(request);

                return response.StatusCode switch
                {
                    (int)HttpStatusCode.OK => Ok(response),
                    (int)HttpStatusCode.BadRequest => BadRequest(response),
                    (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar preferencias de géneros");

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// Actualiza las preferencias de artistas musicales del usuario.
        /// </summary>
        /// <param name="request">Nuevas preferencias de artistas.</param>
        /// <returns>Confirmación de actualización.</returns>
        [HttpPut("preferences/artists")]
        [ProducesResponseType(typeof(UpdateArtistPreferencesResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateArtistPreferences([FromBody] UpdateArtistPreferencesRequest request)
        {
            try
            {
                var firebaseUid = User.FindFirst("firebase_uid")?.Value;
                
                if (string.IsNullOrWhiteSpace(firebaseUid))
                {
                    return Unauthorized(new BaseResponse
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        UserFriendly = "Sesión expirada. Por favor, inicie sesión nuevamente."
                    });
                }

                request.FirebaseUid = firebaseUid;
                var response = await _mediator.Send(request);

                return response.StatusCode switch
                {
                    (int)HttpStatusCode.OK => Ok(response),
                    (int)HttpStatusCode.BadRequest => BadRequest(response),
                    (int)HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar preferencias de artistas");

                BaseResponse errorResponse = new()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }
    }

    // Temporary placeholder classes to make the code compile
    // These will be moved to separate files in the next commit
    public class GetArtistsByGenreRequest : IRequest<GetArtistsByGenreResponse> 
    { 
        public int GenreId { get; set; }
    }
    
    public class GetArtistsByGenreResponse : BaseResponse 
    { 
        public List<ArtistDto> Artists { get; set; } = new();
    }

    public class GetUserPreferencesRequest : IRequest<GetUserPreferencesResponse> 
    { 
        public string FirebaseUid { get; set; }
    }
    
    public class GetUserPreferencesResponse : BaseResponse 
    { 
        public List<GenrePreferenceDto> GenrePreferences { get; set; } = new();
        public List<ArtistPreferenceDto> ArtistPreferences { get; set; } = new();
    }

    public class UpdateGenrePreferencesRequest : IRequest<UpdateGenrePreferencesResponse> 
    { 
        public string FirebaseUid { get; set; }
        public List<GenrePreferenceDto> Preferences { get; set; } = new();
    }
    
    public class UpdateGenrePreferencesResponse : BaseResponse { }

    public class UpdateArtistPreferencesRequest : IRequest<UpdateArtistPreferencesResponse> 
    { 
        public string FirebaseUid { get; set; }
        public List<ArtistPreferenceDto> Preferences { get; set; } = new();
    }
    
    public class UpdateArtistPreferencesResponse : BaseResponse { }

    // DTOs
    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int Popularity { get; set; }
    }

    public class GenrePreferenceDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }

    public class ArtistPreferenceDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }
}