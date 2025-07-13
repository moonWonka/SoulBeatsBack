using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Services;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyStatus
{
    public class GetSpotifyStatusHandler : IRequestHandler<GetSpotifyStatusRequest, GetSpotifyStatusResponse>
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ISpotifyService _spotifyService;

        public GetSpotifyStatusHandler(ISoulBeatsRepository repository, ISpotifyService spotifyService)
        {
            _repository = repository;
            _spotifyService = spotifyService;
        }

        public async Task<GetSpotifyStatusResponse> Handle(GetSpotifyStatusRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Get stored Spotify token
                var tokenModel = await _repository.GetSpotifyTokenAsync(request.FirebaseUid);

                if (tokenModel == null)
                {
                    return new GetSpotifyStatusResponse
                    {
                        StatusCode = 200,
                        Description = "SPOTIFY_NOT_CONNECTED",
                        UserFriendly = "Spotify account not connected",
                        IsConnected = false,
                        TokenValid = false
                    };
                }

                // Check if token is expired
                if (tokenModel.ExpiresAt <= DateTime.UtcNow)
                {
                    try
                    {
                        // Try to refresh the token
                        var refreshedToken = await _spotifyService.RefreshTokenAsync(tokenModel.RefreshToken);
                        refreshedToken.UserId = request.FirebaseUid;

                        // Update token in database
                        await _repository.UpdateSpotifyTokenAsync(request.FirebaseUid, refreshedToken);
                        tokenModel = refreshedToken;
                    }
                    catch
                    {
                        return new GetSpotifyStatusResponse
                        {
                            StatusCode = 200,
                            Description = "SPOTIFY_TOKEN_EXPIRED",
                            UserFriendly = "Spotify token expired and could not be refreshed",
                            IsConnected = false,
                            TokenValid = false
                        };
                    }
                }

                // Validate token and get user profile
                var isTokenValid = await _spotifyService.ValidateTokenAsync(tokenModel.AccessToken);
                if (!isTokenValid)
                {
                    return new GetSpotifyStatusResponse
                    {
                        StatusCode = 200,
                        Description = "SPOTIFY_TOKEN_INVALID",
                        UserFriendly = "Spotify token is invalid",
                        IsConnected = false,
                        TokenValid = false
                    };
                }

                // Get user profile
                var userProfile = await _spotifyService.GetUserProfileAsync(tokenModel.AccessToken);

                return new GetSpotifyStatusResponse
                {
                    StatusCode = 200,
                    Description = "SUCCESS",
                    UserFriendly = "Spotify account connected",
                    IsConnected = true,
                    TokenValid = true,
                    SpotifyUserId = userProfile.Id,
                    ExpiresAt = tokenModel.ExpiresAt,
                    DisplayName = userProfile.DisplayName,
                    Email = userProfile.Email,
                    Country = userProfile.Country,
                    Product = userProfile.Product,
                    Followers = userProfile.Followers,
                    ImageUrl = userProfile.ImageUrl,
                    ExternalUrl = userProfile.ExternalUrl
                };
            }
            catch (Exception ex)
            {
                return new GetSpotifyStatusResponse
                {
                    StatusCode = 500,
                    Description = "INTERNAL_ERROR",
                    UserFriendly = "An error occurred while checking Spotify status",
                    IsConnected = false,
                    TokenValid = false
                };
            }
        }
    }
}
