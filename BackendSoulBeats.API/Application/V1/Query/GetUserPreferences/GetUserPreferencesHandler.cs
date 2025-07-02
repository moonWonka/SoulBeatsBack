using System.Net;
using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.API.Application.V1.ViewModel.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendSoulBeats.API.Application.V1.Query.GetUserPreferences
{
    public class GetUserPreferencesHandler : IRequestHandler<GetUserPreferencesRequest, GetUserPreferencesResponse>
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ILogger<GetUserPreferencesHandler> _logger;

        public GetUserPreferencesHandler(ISoulBeatsRepository repository, ILogger<GetUserPreferencesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GetUserPreferencesResponse> Handle(GetUserPreferencesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("🔍 Obteniendo preferencias del usuario: {FirebaseUid}", request.FirebaseUid);

            try
            {
                if (string.IsNullOrWhiteSpace(request.FirebaseUid))
                {
                    _logger.LogWarning("⚠️ FirebaseUid vacío o nulo");
                    return new GetUserPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        UserFriendly = "ID de usuario inválido"
                    };
                }

                // Obtener preferencias de géneros y artistas en paralelo
                var genrePreferencesTask = _repository.GetUserGenrePreferencesAsync(request.FirebaseUid);
                var artistPreferencesTask = _repository.GetUserArtistPreferencesAsync(request.FirebaseUid);

                await Task.WhenAll(genrePreferencesTask, artistPreferencesTask);

                var genrePreferences = await genrePreferencesTask;
                var artistPreferences = await artistPreferencesTask;

                var response = new GetUserPreferencesResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    UserFriendly = "Preferencias obtenidas exitosamente",
                    GenrePreferences = genrePreferences.Select(gp => new GenrePreferenceDto
                    {
                        GenreId = gp.GenreId,
                        GenreName = gp.GenreName ?? string.Empty,
                        PreferenceLevel = gp.PreferenceLevel
                    }).ToList(),
                    ArtistPreferences = artistPreferences.Select(ap => new ArtistPreferenceDto
                    {
                        ArtistId = ap.ArtistId,
                        ArtistName = ap.ArtistName ?? string.Empty,
                        PreferenceLevel = ap.PreferenceLevel
                    }).ToList()
                };

                _logger.LogDebug("✅ Preferencias obtenidas para {FirebaseUid}: {GenreCount} géneros, {ArtistCount} artistas", 
                    request.FirebaseUid, response.GenrePreferences.Count, response.ArtistPreferences.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener preferencias del usuario: {FirebaseUid}", request.FirebaseUid);

                return new GetUserPreferencesResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };
            }
        }
    }
}