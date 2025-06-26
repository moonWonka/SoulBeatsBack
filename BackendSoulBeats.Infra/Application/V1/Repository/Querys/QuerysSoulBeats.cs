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
        /// Inserta un registro en el historial de acciones del usuario
        /// </summary>
        internal const string InsertUserHistory = @"
            INSERT INTO UserHistory (FirebaseUid, Action, ActionDate) 
            VALUES (@FirebaseUid, @Action, @ActionDate)";

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
    }
}