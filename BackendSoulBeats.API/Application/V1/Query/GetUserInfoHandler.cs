using MediatR;
using Microsoft.ApplicationInsights;
using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Model.Respository;

namespace BackendSoulBeats.API.Application.V1.Query
{
    public class GetUserInfoHandler : IRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ISoulBeatsRepository _soulBeatsRepository;

        public GetUserInfoHandler(TelemetryClient telemetryClient, ISoulBeatsRepository soulBeatsRepository)
        {
            _telemetryClient = telemetryClient;
            _soulBeatsRepository = soulBeatsRepository;
        }

        public async Task<GetUserInfoResponse> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación del request
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    throw new ArgumentException("UserId es requerido", nameof(request.UserId));
                }

                TrackUserInfoRequested(request.UserId);

                DateTime startTime = DateTime.UtcNow;

                // Obtener información del usuario usando el repositorio
                UserInfoReponseModel userInfo = await _soulBeatsRepository.GetUserInfoAsync(request.UserId);

                if (userInfo == null)
                {
                    TrackUserInfoNotFound(request.UserId);

                    throw new KeyNotFoundException($"Usuario con ID '{request.UserId}' no encontrado");
                }

                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Mapear el modelo del repositorio al modelo de respuesta
                var response = new GetUserInfoResponse
                {
                    Id = userInfo.UserId,
                    UserName = userInfo.UserName,
                    Email = userInfo.Email,
                    MoreInformation = $"Usuario: {userInfo.UserName}, Email: {userInfo.Email}, Creado: {userInfo.CreatedAt:yyyy-MM-dd}"
                };

                TrackUserInfoSuccess(request.UserId, userInfo.UserName, duration);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Telemetry Methods

        private void TrackUserInfoRequested(string userId)
        {
            _telemetryClient.TrackEvent("GetUserInfoRequested", new Dictionary<string, string>
            {
                {"Handler", "GetUserInfoHandler"},
                {"UserId", userId},
                {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
            });
        }

        private void TrackUserInfoSuccess(string userId, string userName, double duration)
        {
            _telemetryClient.TrackEvent("GetUserInfoSuccess", new Dictionary<string, string>
            {
                {"Handler", "GetUserInfoHandler"},
                {"UserId", userId},
                {"UserName", userName},
                {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
            });

            _telemetryClient.TrackMetric("UserInfoRequestDuration", duration);
        }

        private void TrackUserInfoNotFound(string userId, string message = "Usuario no encontrado en la base de datos")
        {
            _telemetryClient.TrackEvent("GetUserInfoNotFound", new Dictionary<string, string>
            {
                {"Handler", "GetUserInfoHandler"},
                {"UserId", userId},
                {"Message", message}
            });
        }

        private void TrackUserInfoException(Exception ex, string userId)
        {
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                {"Handler", "GetUserInfoHandler"},
                {"UserId", userId ?? "Unknown"},
                {"ErrorMessage", ex.Message}
            });
        }

        #endregion
    }
}