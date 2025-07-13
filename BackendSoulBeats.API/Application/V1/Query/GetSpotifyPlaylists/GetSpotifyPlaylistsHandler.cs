using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Services;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists
{
    public class GetSpotifyPlaylistsHandler : IRequestHandler<GetSpotifyPlaylistsRequest, GetSpotifyPlaylistsResponse>
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ISpotifyService _spotifyService;

        public GetSpotifyPlaylistsHandler(ISoulBeatsRepository repository, ISpotifyService spotifyService)
        {
            _repository = repository;
            _spotifyService = spotifyService;
        }

        public async Task<GetSpotifyPlaylistsResponse> Handle(GetSpotifyPlaylistsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Get stored Spotify token
                var tokenModel = await _repository.GetSpotifyTokenAsync(request.FirebaseUid);

                if (tokenModel == null)
                {
                    return new GetSpotifyPlaylistsResponse
                    {
                        StatusCode = 400,
                        Description = "SPOTIFY_NOT_CONNECTED",
                        UserFriendly = "Spotify account not connected"
                    };
                }

                // Check if token is expired and refresh if needed
                if (tokenModel.ExpiresAt <= DateTime.UtcNow && !string.IsNullOrEmpty(tokenModel.RefreshToken))
                {
                    try
                    {
                        var refreshedToken = await _spotifyService.RefreshTokenAsync(tokenModel.RefreshToken);
                        refreshedToken.UserId = request.FirebaseUid;
                        refreshedToken.CreatedAt = tokenModel.CreatedAt;

                        await _repository.UpdateSpotifyTokenAsync(request.FirebaseUid, refreshedToken);
                        tokenModel = refreshedToken;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                // Get user playlists from Spotify
                var playlists = await _spotifyService.GetUserPlaylistsAsync(tokenModel.AccessToken, request.Limit, request.Offset);

                return new GetSpotifyPlaylistsResponse
                {
                    StatusCode = 200,
                    Description = "SUCCESS",
                    UserFriendly = "Playlists retrieved successfully",
                    Playlists = playlists,
                    Total = playlists.Count,
                    Limit = request.Limit,
                    Offset = request.Offset
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}