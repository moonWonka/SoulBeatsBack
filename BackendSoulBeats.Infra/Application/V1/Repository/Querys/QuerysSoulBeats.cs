namespace BackendSoulBeats.Infra.Application.V1.Repository.Querys
{
    public static class QuerysSoulBeats
    {
        /// <summary>
        /// Obtiene información de un usuario por su Firebase UID
        /// </summary>
        internal const string GetUserInfo = @"
            SELECT 
                Id AS UserId,
                DisplayName AS UserName, 
                Email AS Email, 
                RegisteredAt AS CreatedAt
            FROM 
                Users 
            WHERE 
                FirebaseUid = @UserId";

        /// <summary>
        /// Obtiene lista de usuarios activos (que tienen edad definida)
        /// </summary>
        internal const string GetActiveUsers = @"
            SELECT 
                Id AS UserId,
                DisplayName AS UserName, 
                Email AS Email, 
                RegisteredAt AS CreatedAt
            FROM Users 
            WHERE Age IS NOT NULL 
            ORDER BY RegisteredAt DESC";

        /// <summary>
        /// Actualiza el último acceso de un usuario
        /// </summary>
        internal const string UpdateLastLogin = @"
            UPDATE Users 
            SET RegisteredAt = @LastLogin 
            WHERE FirebaseUid = @UserId";

        /// <summary>
        /// Obtiene el conteo total de usuarios registrados
        /// </summary>
        internal const string GetTotalUsersCount = @"
            SELECT COUNT(*) FROM Users";

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos
        /// </summary>
        internal const string InsertUser = @"
            INSERT INTO Users (FirebaseUid, DisplayName, Email, RegisteredAt) 
            VALUES (@FirebaseUid, @DisplayName, @Email, @RegisteredAt)";

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos con foto de perfil
        /// </summary>
        internal const string InsertUserWithProfilePicture = @"
            INSERT INTO Users (FirebaseUid, DisplayName, Email, ProfilePictureUrl, RegisteredAt) 
            VALUES (@FirebaseUid, @DisplayName, @Email, @ProfilePictureUrl, @RegisteredAt)";

        /// <summary>
        /// Inserta un registro en el historial de acciones del usuario
        /// </summary>
        internal const string InsertUserHistory = @"
            INSERT INTO UserHistory (FirebaseUid, Action, ActionDate, Details) 
            VALUES (@FirebaseUid, @Action, @ActionDate, @Details)";

        /// <summary>
        /// Verifica si un usuario existe por su Firebase UID
        /// </summary>
        internal const string CheckUserExists = @"
            SELECT COUNT(*) 
            FROM Users 
            WHERE FirebaseUid = @UserId";

        /// <summary>
        /// Obtiene usuarios registrados en un rango de fechas
        /// </summary>
        internal const string GetUsersByDateRange = @"
            SELECT 
                Id AS UserId,
                DisplayName AS UserName, 
                Email AS Email, 
                RegisteredAt AS CreatedAt
            FROM Users 
            WHERE RegisteredAt BETWEEN @StartDate AND @EndDate
            ORDER BY RegisteredAt DESC";

        /// <summary>
        /// Actualiza el perfil completo de un usuario
        /// </summary>
        internal const string UpdateUserProfile = @"
            UPDATE Users 
            SET DisplayName = @DisplayName, 
                Email = @Email,
                Age = @Age
            WHERE FirebaseUid = @UserId";

        /// <summary>
        /// Actualiza el perfil completo del usuario con todos los campos disponibles
        /// </summary>
        internal const string UpdateCompleteUserProfile = @"
            UPDATE Users 
            SET 
                DisplayName = COALESCE(@DisplayName, DisplayName),
                Email = COALESCE(@Email, Email),
                Age = COALESCE(@Age, Age),
                Bio = COALESCE(@Bio, Bio),
                FavoriteGenres = COALESCE(@FavoriteGenres, FavoriteGenres),
                ProfilePictureUrl = COALESCE(@ProfilePictureUrl, ProfilePictureUrl)
            WHERE FirebaseUid = @UserId";

        // =============================================
        // QUERIES PARA MÚSICA Y GÉNEROS
        // =============================================

        /// <summary>
        /// Obtiene todos los géneros musicales activos
        /// </summary>
        internal const string GetActiveGenres = @"
            SELECT 
                Id, Name, Description, IconUrl, DisplayOrder, IsActive, CreatedAt
            FROM Genres 
            WHERE IsActive = 1 
            ORDER BY DisplayOrder, Name";

        /// <summary>
        /// Obtiene artistas por género específico
        /// </summary>
        internal const string GetArtistsByGenre = @"
            SELECT 
                a.Id, a.Name, a.SpotifyId, a.ImageUrl, 
                a.GenreId, g.Name as GenreName, 
                a.Popularity, a.IsActive, a.CreatedAt
            FROM Artists a
            INNER JOIN Genres g ON a.GenreId = g.Id
            WHERE a.GenreId = @GenreId 
            AND a.IsActive = 1 
            AND g.IsActive = 1
            ORDER BY a.Popularity DESC, a.Name";

        /// <summary>
        /// Obtiene todos los artistas activos
        /// </summary>
        internal const string GetAllActiveArtists = @"
            SELECT 
                a.Id, a.Name, a.SpotifyId, a.ImageUrl, 
                a.GenreId, g.Name as GenreName, 
                a.Popularity, a.IsActive, a.CreatedAt
            FROM Artists a
            INNER JOIN Genres g ON a.GenreId = g.Id
            WHERE a.IsActive = 1 
            AND g.IsActive = 1
            ORDER BY a.Popularity DESC, a.Name";

        // =============================================
        // QUERIES PARA PREFERENCIAS DE USUARIO
        // =============================================

        /// <summary>
        /// Obtiene las preferencias de géneros del usuario
        /// </summary>
        internal const string GetUserGenrePreferences = @"
            SELECT 
                ugp.Id, ugp.FirebaseUid, ugp.GenreId, 
                g.Name as GenreName, ugp.PreferenceLevel, 
                ugp.CreatedAt, ugp.UpdatedAt
            FROM UserMusicPreferences ugp
            INNER JOIN Genres g ON ugp.GenreId = g.Id
            WHERE ugp.FirebaseUid = @FirebaseUid 
            AND g.IsActive = 1
            ORDER BY ugp.PreferenceLevel DESC, g.DisplayOrder";

        /// <summary>
        /// Obtiene las preferencias de artistas del usuario
        /// </summary>
        internal const string GetUserArtistPreferences = @"
            SELECT 
                uap.Id, uap.FirebaseUid, uap.ArtistId, 
                a.Name as ArtistName, uap.PreferenceLevel, 
                uap.CreatedAt, uap.UpdatedAt
            FROM UserArtistPreferences uap
            INNER JOIN Artists a ON uap.ArtistId = a.Id
            WHERE uap.FirebaseUid = @FirebaseUid 
            AND a.IsActive = 1
            ORDER BY uap.PreferenceLevel DESC, a.Name";

        /// <summary>
        /// Inserta o actualiza una preferencia de género del usuario
        /// </summary>
        internal const string UpsertUserGenrePreference = @"
            MERGE UserMusicPreferences AS target
            USING (SELECT @FirebaseUid as FirebaseUid, @GenreId as GenreId, @PreferenceLevel as PreferenceLevel) AS source
            ON (target.FirebaseUid = source.FirebaseUid AND target.GenreId = source.GenreId)
            WHEN MATCHED THEN
                UPDATE SET PreferenceLevel = source.PreferenceLevel, UpdatedAt = GETDATE()
            WHEN NOT MATCHED THEN
                INSERT (FirebaseUid, GenreId, PreferenceLevel, CreatedAt, UpdatedAt)
                VALUES (source.FirebaseUid, source.GenreId, source.PreferenceLevel, GETDATE(), GETDATE());";

        /// <summary>
        /// Inserta o actualiza una preferencia de artista del usuario
        /// </summary>
        internal const string UpsertUserArtistPreference = @"
            MERGE UserArtistPreferences AS target
            USING (SELECT @FirebaseUid as FirebaseUid, @ArtistId as ArtistId, @PreferenceLevel as PreferenceLevel) AS source
            ON (target.FirebaseUid = source.FirebaseUid AND target.ArtistId = source.ArtistId)
            WHEN MATCHED THEN
                UPDATE SET PreferenceLevel = source.PreferenceLevel, UpdatedAt = GETDATE()
            WHEN NOT MATCHED THEN
                INSERT (FirebaseUid, ArtistId, PreferenceLevel, CreatedAt, UpdatedAt)
                VALUES (source.FirebaseUid, source.ArtistId, source.PreferenceLevel, GETDATE(), GETDATE());";

        /// <summary>
        /// Elimina una preferencia de género del usuario
        /// </summary>
        internal const string DeleteUserGenrePreference = @"
            DELETE FROM UserMusicPreferences 
            WHERE FirebaseUid = @FirebaseUid AND GenreId = @GenreId";

        /// <summary>
        /// Elimina una preferencia de artista del usuario
        /// </summary>
        internal const string DeleteUserArtistPreference = @"
            DELETE FROM UserArtistPreferences 
            WHERE FirebaseUid = @FirebaseUid AND ArtistId = @ArtistId";
    }
}