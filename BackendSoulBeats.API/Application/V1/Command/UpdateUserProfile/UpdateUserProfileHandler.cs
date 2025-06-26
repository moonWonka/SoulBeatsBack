using MediatR;
using Microsoft.ApplicationInsights;
using BackendSoulBeats.Domain.Application.V1.Repository;
using System.Net;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateUserProfile
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileRequest, UpdateUserProfileResponse>
    {
        private readonly ISoulBeatsRepository _soulBeatsRepository;
        private readonly TelemetryClient _telemetryClient;

        public UpdateUserProfileHandler(ISoulBeatsRepository soulBeatsRepository, TelemetryClient telemetryClient)
        {
            _soulBeatsRepository = soulBeatsRepository;
            _telemetryClient = telemetryClient;
        }

        public async Task<UpdateUserProfileResponse> Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Tracking del evento de solicitud de actualización
                TrackUpdateProfileRequested(request.UserId);

                var startTime = DateTime.UtcNow;

                // Verificar si el usuario existe antes de actualizar
                var existingUser = await _soulBeatsRepository.GetUserInfoAsync(request.UserId);
                if (existingUser == null)
                {
                    TrackUpdateProfileNotFound(request.UserId);

                    return new UpdateUserProfileResponse();

                }

                // Actualizar el perfil del usuario
                var updateResult = await _soulBeatsRepository.UpdateUserProfileAsync(
                    request.UserId,
                    request.DisplayName,
                    request.Email,
                    request.Age,
                    request.Bio,
                    request.FavoriteGenres,
                    request.ProfilePictureUrl
                );

                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                if (updateResult) return new UpdateUserProfileResponse();

                throw new Exception("Error al actualizar el perfil del usuario. Verifique los datos proporcionados.");
    
            }
            catch (Exception ex)
            {
                TrackUpdateProfileException(ex, request.UserId);

                return new UpdateUserProfileResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    MoreInformation = ex.Message
                };
            }
        }

        #region Telemetry Methods

        private void TrackUpdateProfileRequested(string userId)
        {
            _telemetryClient.TrackEvent("UpdateUserProfileRequested", new Dictionary<string, string>
            {
                {"Handler", "UpdateUserProfileHandler"},
                {"UserId", userId},
                {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
            });
        }

        private void TrackUpdateProfileSuccess(string userId, double duration)
        {
            _telemetryClient.TrackEvent("UpdateUserProfileSuccess", new Dictionary<string, string>
            {
                {"Handler", "UpdateUserProfileHandler"},
                {"UserId", userId},
                {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
            });

            _telemetryClient.TrackMetric("UpdateUserProfileDuration", duration);
        }

        private void TrackUpdateProfileNotFound(string userId)
        {
            _telemetryClient.TrackEvent("UpdateUserProfileNotFound", new Dictionary<string, string>
            {
                {"Handler", "UpdateUserProfileHandler"},
                {"UserId", userId},
                {"Message", "Usuario no encontrado"}
            });
        }

        private void TrackUpdateProfileFailed(string userId, string reason)
        {
            _telemetryClient.TrackEvent("UpdateUserProfileFailed", new Dictionary<string, string>
            {
                {"Handler", "UpdateUserProfileHandler"},
                {"UserId", userId},
                {"Reason", reason}
            });
        }

        private void TrackUpdateProfileException(Exception ex, string userId)
        {
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                {"Handler", "UpdateUserProfileHandler"},
                {"UserId", userId ?? "Unknown"},
                {"ErrorMessage", ex.Message}
            });
        }

        #endregion
    }
}
