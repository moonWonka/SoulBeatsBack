using BackendSoulBeats.Domain.Application.V1.Services;
using BackendSoulBeats.Domain.Application.V1.Repository;
using Microsoft.ApplicationInsights;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostRegister
{
    public class PostRegisterHandler : IRequestHandler<PostRegisterRequest, PostRegisterResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ISoulBeatsRepository _soulBeatsRepository;
        private readonly TelemetryClient _telemetryClient;

        public PostRegisterHandler( IGoogleAuthService googleAuthService, ISoulBeatsRepository soulBeatsRepository,
            TelemetryClient telemetryClient)
        {
            _googleAuthService = googleAuthService;
            _soulBeatsRepository = soulBeatsRepository;
            _telemetryClient = telemetryClient;
        }

        public async Task<PostRegisterResponse> Handle(PostRegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación del request
                if (string.IsNullOrWhiteSpace(request.UserEmail))
                {
                    throw new ArgumentException("UserEmail es requerido", nameof(request.UserEmail));
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ArgumentException("Password es requerido", nameof(request.Password));
                }

                // Tracking del evento de solicitud de registro
                _telemetryClient.TrackEvent("UserRegistrationRequested", new Dictionary<string, string>
                {
                    {"Handler", "PostRegisterHandler"},
                    {"UserEmail", request.UserEmail},
                    {"HasDisplayName", (!string.IsNullOrWhiteSpace(request.DisplayName)).ToString()},
                    {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                var startTime = DateTime.UtcNow;
                // 1. Registrar usuario en Firebase
                string firebaseUid = await _googleAuthService.RegisterUserAsync(request.UserEmail, request.Password);

                // 2. Guardar usuario en nuestra base de datos
                string displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? request.UserEmail.Split('@')[0]
                    : request.DisplayName;

                bool dbResult = await _soulBeatsRepository.CreateUserAsync(firebaseUid, displayName, request.UserEmail);

                if (!dbResult)
                {
                    // Si falló la inserción en BD, loggear como advertencia pero no fallar
                    _telemetryClient.TrackEvent("UserRegistrationDbWarning", new Dictionary<string, string>
                    {
                        {"Handler", "PostRegisterHandler"},
                        {"UserEmail", request.UserEmail},
                        {"FirebaseUid", firebaseUid},
                        {"Message", "Usuario creado en Firebase pero falló inserción en BD"}
                    });
                }

                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Tracking del evento de registro exitoso
                _telemetryClient.TrackEvent("UserRegistrationSuccess", new Dictionary<string, string>
                {
                    {"Handler", "PostRegisterHandler"},
                    {"UserEmail", request.UserEmail},
                    {"FirebaseUid", firebaseUid},
                    {"DatabaseSaved", dbResult.ToString()},
                    {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                // Tracking de métricas
                _telemetryClient.TrackMetric("UserRegistrationDuration", duration);

                return new PostRegisterResponse
                {
                    MoreInformation = $"Usuario registrado exitosamente. Firebase UID: {firebaseUid}, BD: {(dbResult ? "OK" : "Error")}"
                };
            }
            catch (Exception ex)
            {
                // Tracking de excepciones
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "PostRegisterHandler"},
                    {"UserEmail", request.UserEmail ?? "Unknown"},
                    {"ErrorMessage", ex.Message}
                });
                throw;
            }
        }
    }
}