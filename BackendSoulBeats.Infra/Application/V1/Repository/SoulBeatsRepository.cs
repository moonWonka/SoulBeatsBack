using Microsoft.Extensions.Logging;
using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Model.Respository;
using BackendSoulBeats.Infra.Application.V1.Repository.Querys;

namespace BackendSoulBeats.Infra.Application.V1.Repository
{
    /// <summary>
    /// Repositorio principal para SoulBeats usando variable de entorno SOULBEATSDB
    /// </summary>
    public class SoulBeatsRepository : AbstractRepository, ISoulBeatsRepository
    {
        private static string DB => "SOULBEATSDB";

        // Usa la variable de entorno "SOULBEATSDB" que contiene la cadena de conexión completa
        public SoulBeatsRepository(ILogger<SoulBeatsRepository> logger) :
            base(environmentVariableName: DB, logger: logger)
        {
        }

        public async Task<UserInfoReponseModel> GetUserInfoAsync(string userId)
        {
            _logger?.LogDebug("🔍 Obteniendo información del usuario: {UserId}", userId);

            var userInfo = await QuerySingleAsync<UserInfoReponseModel>(
                QuerysSoulBeats.GetUserInfo,
                new { UserId = userId },
                useTransaction: false
            );

            _logger?.LogDebug("✅ Consulta ejecutada para usuario: {UserId} | Encontrado: {Found}", userId, userInfo != null);
            return userInfo;
        }

        public async Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email)
        {
            _logger?.LogDebug("🔄 Creando usuario en BD: {FirebaseUid}", firebaseUid);

            var affected = await ExecuteAsync(
                QuerysSoulBeats.InsertUser,
                new { FirebaseUid = firebaseUid, DisplayName = displayName, Email = email, RegisteredAt = DateTime.UtcNow },
                useTransaction: true
            );

            var success = affected > 0;

            if (success)
            {
                _logger?.LogDebug("✅ Usuario creado exitosamente en BD: {FirebaseUid}", firebaseUid);
            }
            else
            {
                _logger?.LogWarning("⚠️ No se pudo crear el usuario en BD: {FirebaseUid}", firebaseUid);
            }

            return success;
        }


        // 🔍 Ejemplos de métodos adicionales que demuestran el uso de AbstractRepository        /// <summary>
        /// Obtiene una lista de usuarios activos
        /// </summary>
        public async Task<IEnumerable<UserInfoReponseModel>> GetActiveUsersAsync()
        {
            return await QueryAsync<UserInfoReponseModel>(
                QuerysSoulBeats.GetActiveUsers,
                useTransaction: false
            );
        }

        /// <summary>
        /// Actualiza el último acceso de un usuario
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            var affected = await ExecuteAsync(
                QuerysSoulBeats.UpdateLastLogin,
                new { UserId = userId, LastLogin = DateTime.UtcNow },
                useTransaction: true
            );

            return affected > 0;
        }

        /// <summary>
        /// Obtiene el conteo total de usuarios
        /// </summary>
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await ExecuteScalarAsync<int>(
                QuerysSoulBeats.GetTotalUsersCount,
                useTransaction: false
            );
        }        /// <summary>
                 /// Crea un nuevo usuario con múltiples operaciones en una transacción
                 /// </summary>
        public async Task<bool> CreateUserWithHistoryAsync(string firebaseUid, string displayName, string email)
        {
            var operations = new (string query, object? parameters)[]
            {
                (QuerysSoulBeats.InsertUser,
                 new { FirebaseUid = firebaseUid, DisplayName = displayName, Email = email, RegisteredAt = DateTime.UtcNow }),

                (QuerysSoulBeats.InsertUserHistory,
                 new { FirebaseUid = firebaseUid, Action = "USER_CREATED", ActionDate = DateTime.UtcNow })
            };

            var totalAffected = await ExecuteMultipleAsync(operations);
            return totalAffected == 2; // Ambas operaciones deben haber afectado 1 fila cada una
        }

        /// <summary>
        /// Verifica si un usuario existe por su Firebase UID
        /// </summary>
        public async Task<bool> UserExistsAsync(string userId)
        {
            var count = await ExecuteScalarAsync<int>(
                QuerysSoulBeats.CheckUserExists,
                new { UserId = userId },
                useTransaction: false
            );

            return count > 0;
        }

        /// <summary>
        /// Obtiene usuarios registrados en un rango de fechas
        /// </summary>
        public async Task<IEnumerable<UserInfoReponseModel>> GetUsersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await QueryAsync<UserInfoReponseModel>(
                QuerysSoulBeats.GetUsersByDateRange,
                new { StartDate = startDate, EndDate = endDate },
                useTransaction: false
            );
        }

        /// <summary>
        /// Actualiza toda la información del perfil de usuario
        /// </summary>
        public async Task<bool> UpdateUserProfileAsync(string userId, string? displayName = null, string? email = null, 
            int? age = null, string? bio = null, string? favoriteGenres = null, string? profilePictureUrl = null)
        {
            _logger?.LogDebug("🔄 Actualizando perfil completo del usuario: {UserId}", userId);

            var affected = await ExecuteAsync(
                QuerysSoulBeats.UpdateCompleteUserProfile,
                new { 
                    UserId = userId, 
                    DisplayName = displayName, 
                    Email = email, 
                    Age = age,
                    Bio = bio,
                    FavoriteGenres = favoriteGenres,
                    ProfilePictureUrl = profilePictureUrl
                },
                useTransaction: true
            );

            var success = affected > 0;

            if (success)
            {
                _logger?.LogDebug("✅ Perfil de usuario actualizado exitosamente: {UserId}", userId);
            }
            else
            {
                _logger?.LogWarning("⚠️ No se pudo actualizar el perfil del usuario: {UserId}", userId);
            }

            return success;
        }
    }
}