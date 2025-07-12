using BackendSoulBeats.Domain.Application.V1.Repository;
using BackendSoulBeats.Domain.Application.V1.Services;
using Microsoft.AspNetCore.Authorization;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeHandler
    {
        private readonly ISoulBeatsRepository _repository;
        private readonly ISpotifyService _spotifyService;

        public PostSpotifyTokenExchangeHandler(ISoulBeatsRepository repository, ISpotifyService spotifyService)
        {
            _repository = repository;
            _spotifyService = spotifyService;
        }

        public async Task<PostSpotifyTokenExchangeResponse> Handle(PostSpotifyTokenExchangeRequest request, string firebaseUid)
        {
            try
            {
                // Exchange authorization code for access token
                var tokenModel = await _spotifyService.ExchangeCodeForTokenAsync(request.Code, request.RedirectUri);
                tokenModel.UserId = firebaseUid;

                // Save token to database
                var saved = await _repository.SaveSpotifyTokenAsync(firebaseUid, tokenModel);

                if (!saved)
                {
                    return new PostSpotifyTokenExchangeResponse
                    {
                        Header = new ViewModel.Common.HeaderViewModel
                        {
                            Code = "SPOTIFY_TOKEN_SAVE_FAILED",
                            Message = "Failed to save Spotify token"
                        }
                    };
                }

                return new PostSpotifyTokenExchangeResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "SUCCESS",
                        Message = "Spotify account connected successfully"
                    },
                    IsConnected = true,
                    ExpiresAt = tokenModel.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                return new PostSpotifyTokenExchangeResponse
                {
                    Header = new ViewModel.Common.HeaderViewModel
                    {
                        Code = "SPOTIFY_OAUTH_ERROR",
                        Message = $"Failed to connect Spotify account: {ex.Message}"
                    }
                };
            }
        }
    }
}