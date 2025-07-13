using BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange;
using BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists;
using BackendSoulBeats.API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class SpotifyController : ControllerBase
    {
        private readonly PostSpotifyTokenExchangeHandler _tokenExchangeHandler;
        private readonly GetSpotifyPlaylistsHandler _playlistsHandler;

        public SpotifyController(
            PostSpotifyTokenExchangeHandler tokenExchangeHandler,
            GetSpotifyPlaylistsHandler playlistsHandler)
        {
            _tokenExchangeHandler = tokenExchangeHandler;
            _playlistsHandler = playlistsHandler;
        }

        /// <summary>
        /// Intercambia el código de autorización de Spotify por tokens de acceso
        /// </summary>
        /// <param name="request">Código de autorización y datos de redirección</param>
        /// <returns>Respuesta indicando si la conexión fue exitosa</returns>
        [HttpPost("token-exchange")]
        public async Task<ActionResult<PostSpotifyTokenExchangeResponse>> ExchangeSpotifyToken(
            [FromBody] PostSpotifyTokenExchangeRequest request)
        {
            // Obtener el UID del usuario autenticado desde Firebase
            var firebaseUid = HttpContext.Items["FirebaseUID"]?.ToString();
            
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new PostSpotifyTokenExchangeResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "UNAUTHORIZED",
                        Message = "User not authenticated"
                    }
                });
            }

            var response = await _tokenExchangeHandler.Handle(request, firebaseUid);
            
            if (response.Header.Code == "SUCCESS")
            {
                return Ok(response);
            }
            
            return BadRequest(response);
        }

        /// <summary>
        /// Obtiene las playlists del usuario desde Spotify
        /// </summary>
        /// <param name="limit">Número máximo de playlists a obtener (por defecto 20)</param>
        /// <param name="offset">Número de playlists a omitir (por defecto 0)</param>
        /// <returns>Lista de playlists del usuario</returns>
        [HttpGet("playlists")]
        public async Task<ActionResult<GetSpotifyPlaylistsResponse>> GetSpotifyPlaylists(
            [FromQuery] int limit = 20, 
            [FromQuery] int offset = 0)
        {
            // Obtener el UID del usuario autenticado desde Firebase
            var firebaseUid = HttpContext.Items["FirebaseUID"]?.ToString();
            
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new GetSpotifyPlaylistsResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "UNAUTHORIZED",
                        Message = "User not authenticated"
                    }
                });
            }

            var request = new GetSpotifyPlaylistsRequest
            {
                Limit = Math.Max(1, Math.Min(50, limit)), // Limitar entre 1 y 50
                Offset = Math.Max(0, offset)
            };

            var response = await _playlistsHandler.Handle(request, firebaseUid);
            
            if (response.Header.Code == "SUCCESS")
            {
                return Ok(response);
            }
            
            if (response.Header.Code == "SPOTIFY_NOT_CONNECTED")
            {
                return BadRequest(response);
            }
            
            return StatusCode(500, response);
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