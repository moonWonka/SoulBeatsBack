using BackendSoulBeats.Domain.Application.V1.Model.Respository;

namespace BackendSoulBeats.Domain.Application.V1.Repository
{
    public interface ISoulBeatsRepository
    {
        Task<UserInfoReponseModel> GetUserInfoAsync(string userId);
        Task<bool> CreateUserAsync(string firebaseUid, string displayName, string email);
        Task<bool> UpdateUserProfileAsync(string userId, string displayName, string email, int? age, string? bio, string? favoriteGenres, string? profilePictureUrl);
    }
}