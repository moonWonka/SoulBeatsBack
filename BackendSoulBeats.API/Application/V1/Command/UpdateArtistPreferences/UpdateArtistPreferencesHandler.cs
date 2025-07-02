using System.Net;
using BackendSoulBeats.Domain.Application.V1.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateArtistPreferences
{
    public class UpdateArtistPreferencesHandler : IRequestHandler<UpdateArtistPreferencesRequest, UpdateArtistPreferencesResponse>
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ILogger<UpdateArtistPreferencesHandler> _logger;

        public UpdateArtistPreferencesHandler(ISoulBeatsRepository repository, ILogger<UpdateArtistPreferencesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<UpdateArtistPreferencesResponse> Handle(UpdateArtistPreferencesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("🔄 Actualizando preferencias de artistas para usuario: {FirebaseUid}, Count: {Count}", 
                request.FirebaseUid, request.Preferences?.Count ?? 0);

            try
            {
                if (string.IsNullOrWhiteSpace(request.FirebaseUid))
                {
                    _logger.LogWarning("⚠️ FirebaseUid vacío o nulo");
                    return new UpdateArtistPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        UserFriendly = "ID de usuario inválido"
                    };
                }

                if (request.Preferences == null || !request.Preferences.Any())
                {
                    _logger.LogWarning("⚠️ Lista de preferencias vacía para usuario: {FirebaseUid}", request.FirebaseUid);
                    return new UpdateArtistPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        UserFriendly = "Debe proporcionar al menos una preferencia de artista"
                    };
                }

                // Validar niveles de preferencia
                var invalidPreferences = request.Preferences.Where(p => p.PreferenceLevel < 1 || p.PreferenceLevel > 5 || p.ArtistId <= 0).ToList();
                if (invalidPreferences.Any())
                {
                    _logger.LogWarning("⚠️ Preferencias inválidas encontradas para usuario: {FirebaseUid}", request.FirebaseUid);
                    return new UpdateArtistPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        UserFriendly = "Algunos artistas tienen valores inválidos. Los niveles deben estar entre 1 y 5."
                    };
                }

                // Convertir a formato esperado por el repositorio
                var preferences = request.Preferences.Select(p => (p.ArtistId, p.PreferenceLevel)).ToList();

                var success = await _repository.UpdateUserArtistPreferencesAsync(request.FirebaseUid, preferences);

                if (success)
                {
                    _logger.LogDebug("✅ Preferencias de artistas actualizadas exitosamente para usuario: {FirebaseUid}", request.FirebaseUid);
                    return new UpdateArtistPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        UserFriendly = "Preferencias de artistas actualizadas exitosamente",
                        UpdatedCount = preferences.Count
                    };
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudieron actualizar las preferencias de artistas para usuario: {FirebaseUid}", request.FirebaseUid);
                    return new UpdateArtistPreferencesResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        UserFriendly = "No se pudieron actualizar las preferencias"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar preferencias de artistas para usuario: {FirebaseUid}", request.FirebaseUid);

                return new UpdateArtistPreferencesResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    UserFriendly = "Error interno del servidor"
                };
            }
        }
    }
}