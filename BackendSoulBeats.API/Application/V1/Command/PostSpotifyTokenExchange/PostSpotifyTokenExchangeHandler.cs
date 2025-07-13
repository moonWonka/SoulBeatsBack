using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Services;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeHandler : IRequestHandler<PostSpotifyTokenExchangeRequest, PostSpotifyTokenExchangeResponse> 
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ISpotifyService _spotifyService;

        public PostSpotifyTokenExchangeHandler(ISoulBeatsRepository repository, ISpotifyService spotifyService)
        {
            _repository = repository;
            _spotifyService = spotifyService;
        }

        public async Task<PostSpotifyTokenExchangeResponse> Handle(PostSpotifyTokenExchangeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Exchange authorization code for access token
                var tokenModel = await _spotifyService.ExchangeCodeForTokenAsync(request.Code, request.RedirectUri);
                tokenModel.UserId = request.FirebaseUid;

                // Get user profile information
                var userProfile = await _spotifyService.GetUserProfileAsync(tokenModel.AccessToken);

                // Save token to database
                // var saved = await _repository.SaveSpotifyTokenAsync(request.FirebaseUid, tokenModel);

                // if (!saved)
                // {
                //     return new PostSpotifyTokenExchangeResponse
                //     {
                //         StatusCode = 500,
                //         Description = "SPOTIFY_TOKEN_SAVE_FAILED",
                //         UserFriendly = "Failed to save Spotify token"
                //     };
                // }

                return new PostSpotifyTokenExchangeResponse
                {
                    StatusCode = 200,
                    Description = "SUCCESS",
                    UserFriendly = "Spotify account connected successfully",
                    IsConnected = true,
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}