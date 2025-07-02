using BackendSoulBeats.Domain.Application.V1.Model.Respository;

namespace BackendSoulBeats.Domain.Application.V1.Repository
{
    public interface ISoulBeatsRepository
    {
        // User management
        Task<UserInfoReponseModel> GetUserInfoAsync(string userId);
        Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email);
        Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email, string? profilePictureUrl);
        Task<bool> UpdateUserProfileAsync(string userId, string displayName, string email, int? age, string? bio, string? favoriteGenres, string? profilePictureUrl);
        Task<bool> UserExistsAsync(string firebaseUid);
        Task<bool> InsertUserHistoryAsync(string firebaseUid, string action, string? details = null);

        // Music data
        Task<IEnumerable<GenreModel>> GetActiveGenresAsync();
        Task<IEnumerable<ArtistModel>> GetArtistsByGenreAsync(int genreId);
        Task<IEnumerable<ArtistModel>> GetAllActiveArtistsAsync();

        // User preferences
        Task<IEnumerable<UserGenrePreferenceModel>> GetUserGenrePreferencesAsync(string firebaseUid);
        Task<IEnumerable<UserArtistPreferenceModel>> GetUserArtistPreferencesAsync(string firebaseUid);
        Task<bool> UpdateUserGenrePreferencesAsync(string firebaseUid, List<(int genreId, int preferenceLevel)> preferences);
        Task<bool> UpdateUserArtistPreferencesAsync(string firebaseUid, List<(int artistId, int preferenceLevel)> preferences);
    }
}