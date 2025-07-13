using BackendSoulBeats.Domain.Application.V1.Model.SpotifyService;

namespace BackendSoulBeats.Domain.Application.V1.Services
{
    public interface ISpotifyService
    {
        Task<SpotifyTokenModel> ExchangeCodeForTokenAsync(string code, string redirectUri);
        Task<SpotifyTokenModel> RefreshTokenAsync(string refreshToken);
        Task<List<SpotifyPlaylistModel>> GetUserPlaylistsAsync(string accessToken, int limit = 20, int offset = 0);
        Task<bool> ValidateTokenAsync(string accessToken);
    }
}