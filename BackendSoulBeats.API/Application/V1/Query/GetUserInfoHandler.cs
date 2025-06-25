using MediatR;
using Microsoft.ApplicationInsights;

namespace BackendSoulBeats.API.Application.V1.Query
{
    public class GetUserInfoHandler : IRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
    {
        private readonly TelemetryClient _telemetryClient;

        public GetUserInfoHandler(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public async Task<GetUserInfoResponse> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Tracking del evento de solicitud de información de usuario
                _telemetryClient.TrackEvent("GetUserInfoRequested", new Dictionary<string, string>
                {
                    {"Handler", "GetUserInfoHandler"},
                    {"UserId", request.UserId?.ToString() ?? "Unknown"},
                    {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                // Simular algún procesamiento
                await Task.Delay(100, cancellationToken);
                var response = new GetUserInfoResponse
                {
                    Id = request.UserId ?? string.Empty,
                    MoreInformation = "User information retrieved successfully."
                };

                // Tracking del evento de respuesta exitosa
                _telemetryClient.TrackEvent("GetUserInfoSuccess", new Dictionary<string, string>
                {
                    {"Handler", "GetUserInfoHandler"},
                    {"UserId", request.UserId?.ToString() ?? "Unknown"},
                    {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                // Tracking de métricas personalizadas
                _telemetryClient.TrackMetric("UserInfoRequestDuration", 100);

                return response;
            }
            catch (Exception ex)
            {
                // Tracking de excepciones
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "GetUserInfoHandler"},
                    {"UserId", request.UserId?.ToString() ?? "Unknown"},
                    {"ErrorMessage", ex.Message}
                });

                throw;
            }
        }
    }
}