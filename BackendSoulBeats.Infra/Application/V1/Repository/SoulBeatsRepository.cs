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
            return await CreateUserAsync(firebaseUid, displayName, email, null);
        }

        public async Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email, string? profilePictureUrl)
        {
            _logger?.LogDebug("🔄 Creando usuario en BD: {FirebaseUid}", firebaseUid);

            var affected = await ExecuteAsync(
                QuerysSoulBeats.InsertUserWithProfilePicture,
                new { 
                    FirebaseUid = firebaseUid, 
                    DisplayName = displayName, 
                    Email = email, 
                    ProfilePictureUrl = profilePictureUrl,
                    RegisteredAt = DateTime.UtcNow 
                },
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
        public async Task<bool> UserExistsAsync(string firebaseUid)
        {
            var count = await ExecuteScalarAsync<int>(
                QuerysSoulBeats.CheckUserExists,
                new { UserId = firebaseUid },
                useTransaction: false
            );

            return count > 0;
        }

        /// <summary>
        /// Inserta un registro en el historial de acciones del usuario
        /// </summary>
        public async Task<bool> InsertUserHistoryAsync(string firebaseUid, string action, string? details = null)
        {
            _logger?.LogDebug("🔄 Insertando historial para usuario: {FirebaseUid}, Acción: {Action}", firebaseUid, action);

            var affected = await ExecuteAsync(
                QuerysSoulBeats.InsertUserHistory,
                new { 
                    FirebaseUid = firebaseUid, 
                    Action = action, 
                    ActionDate = DateTime.UtcNow,
                    Details = details
                },
                useTransaction: true
            );

            var success = affected > 0;

            if (success)
            {
                _logger?.LogDebug("✅ Historial insertado exitosamente para usuario: {FirebaseUid}", firebaseUid);
            }
            else
            {
                _logger?.LogWarning("⚠️ No se pudo insertar historial para usuario: {FirebaseUid}", firebaseUid);
            }

            return success;
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

        // =============================================
        // IMPLEMENTACIONES PARA MÚSICA Y GÉNEROS
        // =============================================

        /// <summary>
        /// Obtiene todos los géneros musicales activos
        /// </summary>
        public async Task<IEnumerable<GenreModel>> GetActiveGenresAsync()
        {
            _logger?.LogDebug("🔍 Obteniendo géneros musicales activos");

            var genres = await QueryAsync<GenreModel>(
                QuerysSoulBeats.GetActiveGenres,
                useTransaction: false
            );

            _logger?.LogDebug("✅ Géneros obtenidos: {Count}", genres?.Count() ?? 0);
            return genres ?? Enumerable.Empty<GenreModel>();
        }

        /// <summary>
        /// Obtiene artistas por género específico
        /// </summary>
        public async Task<IEnumerable<ArtistModel>> GetArtistsByGenreAsync(int genreId)
        {
            _logger?.LogDebug("🔍 Obteniendo artistas del género: {GenreId}", genreId);

            var artists = await QueryAsync<ArtistModel>(
                QuerysSoulBeats.GetArtistsByGenre,
                new { GenreId = genreId },
                useTransaction: false
            );

            _logger?.LogDebug("✅ Artistas obtenidos para género {GenreId}: {Count}", genreId, artists?.Count() ?? 0);
            return artists ?? Enumerable.Empty<ArtistModel>();
        }

        /// <summary>
        /// Obtiene todos los artistas activos
        /// </summary>
        public async Task<IEnumerable<ArtistModel>> GetAllActiveArtistsAsync()
        {
            _logger?.LogDebug("🔍 Obteniendo todos los artistas activos");

            var artists = await QueryAsync<ArtistModel>(
                QuerysSoulBeats.GetAllActiveArtists,
                useTransaction: false
            );

            _logger?.LogDebug("✅ Artistas activos obtenidos: {Count}", artists?.Count() ?? 0);
            return artists ?? Enumerable.Empty<ArtistModel>();
        }

        // =============================================
        // IMPLEMENTACIONES PARA PREFERENCIAS DE USUARIO
        // =============================================

        /// <summary>
        /// Obtiene las preferencias de géneros del usuario
        /// </summary>
        public async Task<IEnumerable<UserGenrePreferenceModel>> GetUserGenrePreferencesAsync(string firebaseUid)
        {
            _logger?.LogDebug("🔍 Obteniendo preferencias de géneros para usuario: {FirebaseUid}", firebaseUid);

            var preferences = await QueryAsync<UserGenrePreferenceModel>(
                QuerysSoulBeats.GetUserGenrePreferences,
                new { FirebaseUid = firebaseUid },
                useTransaction: false
            );

            _logger?.LogDebug("✅ Preferencias de géneros obtenidas para {FirebaseUid}: {Count}", firebaseUid, preferences?.Count() ?? 0);
            return preferences ?? Enumerable.Empty<UserGenrePreferenceModel>();
        }

        /// <summary>
        /// Obtiene las preferencias de artistas del usuario
        /// </summary>
        public async Task<IEnumerable<UserArtistPreferenceModel>> GetUserArtistPreferencesAsync(string firebaseUid)
        {
            _logger?.LogDebug("🔍 Obteniendo preferencias de artistas para usuario: {FirebaseUid}", firebaseUid);

            var preferences = await QueryAsync<UserArtistPreferenceModel>(
                QuerysSoulBeats.GetUserArtistPreferences,
                new { FirebaseUid = firebaseUid },
                useTransaction: false
            );

            _logger?.LogDebug("✅ Preferencias de artistas obtenidas para {FirebaseUid}: {Count}", firebaseUid, preferences?.Count() ?? 0);
            return preferences ?? Enumerable.Empty<UserArtistPreferenceModel>();
        }

        /// <summary>
        /// Actualiza las preferencias de géneros del usuario
        /// </summary>
        public async Task<bool> UpdateUserGenrePreferencesAsync(string firebaseUid, List<(int genreId, int preferenceLevel)> preferences)
        {
            _logger?.LogDebug("🔄 Actualizando preferencias de géneros para usuario: {FirebaseUid}, Items: {Count}", firebaseUid, preferences?.Count ?? 0);

            if (preferences == null || !preferences.Any())
            {
                _logger?.LogWarning("⚠️ No hay preferencias de géneros para actualizar");
                return true; // No es un error, simplemente no hay nada que hacer
            }

            try
            {
                var operations = preferences.Select(p => (
                    query: QuerysSoulBeats.UpsertUserGenrePreference,
                    parameters: (object?)new { 
                        FirebaseUid = firebaseUid, 
                        GenreId = p.genreId, 
                        PreferenceLevel = p.preferenceLevel 
                    }
                )).ToArray();

                var totalAffected = await ExecuteMultipleAsync(operations);
                var success = totalAffected > 0;

                if (success)
                {
                    _logger?.LogDebug("✅ Preferencias de géneros actualizadas exitosamente para {FirebaseUid}", firebaseUid);
                }
                else
                {
                    _logger?.LogWarning("⚠️ No se actualizaron preferencias de géneros para {FirebaseUid}", firebaseUid);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al actualizar preferencias de géneros para {FirebaseUid}", firebaseUid);
                throw;
            }
        }

        /// <summary>
        /// Actualiza las preferencias de artistas del usuario
        /// </summary>
        public async Task<bool> UpdateUserArtistPreferencesAsync(string firebaseUid, List<(int artistId, int preferenceLevel)> preferences)
        {
            _logger?.LogDebug("🔄 Actualizando preferencias de artistas para usuario: {FirebaseUid}, Items: {Count}", firebaseUid, preferences?.Count ?? 0);

            if (preferences == null || !preferences.Any())
            {
                _logger?.LogWarning("⚠️ No hay preferencias de artistas para actualizar");
                return true; // No es un error, simplemente no hay nada que hacer
            }

            try
            {
                var operations = preferences.Select(p => (
                    query: QuerysSoulBeats.UpsertUserArtistPreference,
                    parameters: (object?)new { 
                        FirebaseUid = firebaseUid, 
                        ArtistId = p.artistId, 
                        PreferenceLevel = p.preferenceLevel 
                    }
                )).ToArray();

                var totalAffected = await ExecuteMultipleAsync(operations);
                var success = totalAffected > 0;

                if (success)
                {
                    _logger?.LogDebug("✅ Preferencias de artistas actualizadas exitosamente para {FirebaseUid}", firebaseUid);
                }
                else
                {
                    _logger?.LogWarning("⚠️ No se actualizaron preferencias de artistas para {FirebaseUid}", firebaseUid);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error al actualizar preferencias de artistas para {FirebaseUid}", firebaseUid);
                throw;
            }
        }
    }
}