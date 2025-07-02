using BackendSoulBeats.Domain.Application.V1.Repository;
using Microsoft.ApplicationInsights;
using MediatR;
using System.Net;

namespace BackendSoulBeats.API.Application.V1.Command.PostGoogleRegister
{
    public class PostGoogleRegisterHandler : IRequestHandler<PostGoogleRegisterRequest, PostGoogleRegisterResponse>
    {
        private readonly ISoulBeatsRepository _soulBeatsRepository;
        private readonly TelemetryClient _telemetryClient;

        public PostGoogleRegisterHandler(ISoulBeatsRepository soulBeatsRepository, TelemetryClient telemetryClient)
        {
            _soulBeatsRepository = soulBeatsRepository;
            _telemetryClient = telemetryClient;
        }

        public async Task<PostGoogleRegisterResponse> Handle(PostGoogleRegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación del request
                if (string.IsNullOrWhiteSpace(request.UserEmail))
                {
                    return new PostGoogleRegisterResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Description = "UserEmail es requerido",
                        UserFriendly = "El email del usuario es requerido"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.FirebaseUid))
                {
                    return new PostGoogleRegisterResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Description = "FirebaseUid es requerido",
                        UserFriendly = "El identificador de Firebase es requerido"
                    };
                }

                // Tracking del evento de solicitud
                _telemetryClient.TrackEvent("GoogleUserRegistrationRequested", new Dictionary<string, string>
                {
                    {"Handler", "PostGoogleRegisterHandler"},
                    {"UserEmail", request.UserEmail},
                    {"FirebaseUid", request.FirebaseUid},
                    {"HasDisplayName", (!string.IsNullOrWhiteSpace(request.DisplayName)).ToString()},
                    {"HasPhotoURL", (!string.IsNullOrWhiteSpace(request.PhotoURL)).ToString()},
                    {"RequestTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                var startTime = DateTime.UtcNow;

                // 1. Verificar si el usuario ya existe
                bool userExists = await _soulBeatsRepository.UserExistsAsync(request.FirebaseUid);
                bool isNewUser = !userExists;

                // 2. Preparar datos del usuario
                string displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? request.UserEmail.Split('@')[0]
                    : request.DisplayName;

                bool dbResult;
                if (isNewUser)
                {
                    // Crear nuevo usuario
                    dbResult = await _soulBeatsRepository.CreateUserAsync(
                        request.FirebaseUid, 
                        displayName, 
                        request.UserEmail, 
                        request.PhotoURL);

                    if (dbResult)
                    {
                        // Registrar acción en historial
                        await _soulBeatsRepository.InsertUserHistoryAsync(
                            request.FirebaseUid, 
                            "USER_CREATED_GOOGLE", 
                            "Usuario creado mediante autenticación con Google");
                    }
                }
                else
                {
                    // Actualizar usuario existente (por si hay cambios en nombre o foto)
                    dbResult = await _soulBeatsRepository.UpdateUserProfileAsync(
                        request.FirebaseUid,
                        displayName,
                        request.UserEmail,
                        null, // Age - no se modifica
                        null, // Bio - no se modifica  
                        null, // FavoriteGenres - no se modifica
                        request.PhotoURL);

                    if (dbResult)
                    {
                        // Registrar acción en historial
                        await _soulBeatsRepository.InsertUserHistoryAsync(
                            request.FirebaseUid, 
                            "USER_UPDATED_GOOGLE", 
                            "Información de usuario actualizada desde Google");
                    }
                }

                if (!dbResult)
                {
                    _telemetryClient.TrackEvent("GoogleUserRegistrationDbError", new Dictionary<string, string>
                    {
                        {"Handler", "PostGoogleRegisterHandler"},
                        {"UserEmail", request.UserEmail},
                        {"FirebaseUid", request.FirebaseUid},
                        {"IsNewUser", isNewUser.ToString()},
                        {"Message", "Error al guardar/actualizar usuario en BD"}
                    });

                    return new PostGoogleRegisterResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Description = "Error al procesar usuario en base de datos",
                        UserFriendly = "Error interno del servidor. Intente nuevamente."
                    };
                }

                // 3. Verificar si el perfil está completo
                var userInfo = await _soulBeatsRepository.GetUserInfoAsync(request.FirebaseUid);
                bool profileComplete = userInfo != null && 
                                     !string.IsNullOrWhiteSpace(userInfo.UserName) &&
                                     !string.IsNullOrWhiteSpace(userInfo.Email);

                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Tracking del evento exitoso
                _telemetryClient.TrackEvent("GoogleUserRegistrationSuccess", new Dictionary<string, string>
                {
                    {"Handler", "PostGoogleRegisterHandler"},
                    {"UserEmail", request.UserEmail},
                    {"FirebaseUid", request.FirebaseUid},
                    {"IsNewUser", isNewUser.ToString()},
                    {"ProfileComplete", profileComplete.ToString()},
                    {"ResponseTime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
                });

                // Tracking de métricas
                _telemetryClient.TrackMetric("GoogleUserRegistrationDuration", duration);

                return new PostGoogleRegisterResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Description = isNewUser ? "Usuario creado exitosamente" : "Usuario actualizado exitosamente",
                    UserFriendly = isNewUser ? "¡Bienvenido a SoulBeats!" : "¡Bienvenido de vuelta!",
                    IsNewUser = isNewUser,
                    FirebaseUid = request.FirebaseUid,
                    Email = request.UserEmail,
                    DisplayName = displayName,
                    ProfileComplete = profileComplete
                };
            }
            catch (Exception ex)
            {
                // Tracking de excepciones
                _telemetryClient.TrackException(ex, new Dictionary<string, string>
                {
                    {"Handler", "PostGoogleRegisterHandler"},
                    {"UserEmail", request.UserEmail ?? "Unknown"},
                    {"FirebaseUid", request.FirebaseUid ?? "Unknown"},
                    {"ErrorMessage", ex.Message}
                });

                return new PostGoogleRegisterResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Description = $"Error interno: {ex.Message}",
                    UserFriendly = "Error interno del servidor. Intente nuevamente."
                };
            }
        }
    }
}