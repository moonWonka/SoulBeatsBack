using BackendSoulBeats.Domain.Application.V1.Model.Respository;

namespace BackendSoulBeats.Domain.Application.V1.Repository
{
    public interface ISoulBeatsRepository
    {
        Task<UserInfoReponseModel> GetUserInfoAsync(string userId);
        Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email);
        Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email, string? profilePictureUrl);
        Task<bool> UpdateUserProfileAsync(string userId, string displayName, string email, int? age, string? bio, string? favoriteGenres, string? profilePictureUrl);
        Task<bool> UserExistsAsync(string firebaseUid);
        Task<bool> InsertUserHistoryAsync(string firebaseUid, string action, string? details = null);
    }
}