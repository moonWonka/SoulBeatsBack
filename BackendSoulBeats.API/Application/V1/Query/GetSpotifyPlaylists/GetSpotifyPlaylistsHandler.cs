using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Services;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists
{
    public class GetSpotifyPlaylistsHandler
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ISpotifyService _spotifyService;

        public GetSpotifyPlaylistsHandler(ISoulBeatsRepository repository, ISpotifyService spotifyService)
        {
            _repository = repository;
            _spotifyService = spotifyService;
        }

        public async Task<GetSpotifyPlaylistsResponse> Handle(GetSpotifyPlaylistsRequest request, string firebaseUid)
        {
            try
            {
                // Get stored Spotify token
                var tokenModel = await _repository.GetSpotifyTokenAsync(firebaseUid);

                if (tokenModel == null)
                {
                    return new GetSpotifyPlaylistsResponse
                    {
                        Header = new ViewModel.Common.HeaderViewModel
                        {
                            Code = "SPOTIFY_NOT_CONNECTED",
                            Message = "Spotify account not connected"
                        }
                    };
                }

                // Check if token is expired and refresh if needed
                if (tokenModel.ExpiresAt <= DateTime.UtcNow && !string.IsNullOrEmpty(tokenModel.RefreshToken))
                {
                    try
                    {
                        var refreshedToken = await _spotifyService.RefreshTokenAsync(tokenModel.RefreshToken);
                        refreshedToken.UserId = firebaseUid;
                        refreshedToken.CreatedAt = tokenModel.CreatedAt;

                        await _repository.UpdateSpotifyTokenAsync(firebaseUid, refreshedToken);
                        tokenModel = refreshedToken;
                    }
                    catch (Exception ex)
                    {
                        return new GetSpotifyPlaylistsResponse
                        {
                            Header = new ViewModel.Common.HeaderViewModel
                            {
                                Code = "SPOTIFY_TOKEN_REFRESH_FAILED",
                                Message = $"Failed to refresh Spotify token: {ex.Message}"
                            }
                        };
                    }
                }

                // Get user playlists from Spotify
                var playlists = await _spotifyService.GetUserPlaylistsAsync(tokenModel.AccessToken, request.Limit, request.Offset);

                return new GetSpotifyPlaylistsResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "SUCCESS",
                        Message = "Playlists retrieved successfully"
                    },
                    Playlists = playlists,
                    Total = playlists.Count,
                    Limit = request.Limit,
                    Offset = request.Offset
                };
            }
            catch (Exception ex)
            {
                return new GetSpotifyPlaylistsResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "SPOTIFY_API_ERROR",
                        Message = $"Failed to retrieve playlists: {ex.Message}"
                    }
                };
            }
        }
    }
}