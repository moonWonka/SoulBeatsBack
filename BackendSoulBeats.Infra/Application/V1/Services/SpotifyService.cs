using BackendSoulBeats.Domain.Application.V1.Model.Respository;
using BackendSoulBeats.Domain.Application.V1.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace BackendSoulBeats.Infra.Application.V1.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _spotifyApiBaseUrl = "https://api.spotify.com/v1";
        private readonly string _spotifyTokenUrl = "https://accounts.spotify.com/api/token";

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _clientId = _configuration["Spotify:ClientId"];
            _clientSecret = _configuration["Spotify:ClientSecret"];
        }

        public async Task<SpotifyTokenModel> ExchangeCodeForTokenAsync(string code, string redirectUri)
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            var response = await _httpClient.PostAsync(_spotifyTokenUrl, tokenRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to exchange code for token: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new SpotifyTokenModel
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<SpotifyTokenModel> RefreshTokenAsync(string refreshToken)
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            var response = await _httpClient.PostAsync(_spotifyTokenUrl, tokenRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to refresh token: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new SpotifyTokenModel
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken ?? refreshToken, // Refresh token might not be returned
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<List<SpotifyPlaylistModel>> GetUserPlaylistsAsync(string accessToken, int limit = 20, int offset = 0)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var url = $"{_spotifyApiBaseUrl}/me/playlists?limit={limit}&offset={offset}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get user playlists: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var playlistsResponse = JsonSerializer.Deserialize<SpotifyPlaylistsResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return playlistsResponse.Items.Select(item => new SpotifyPlaylistModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Public = item.Public,
                Collaborative = item.Collaborative,
                SnapshotId = item.SnapshotId,
                TracksTotal = item.Tracks?.Total ?? 0,
                ExternalUrl = item.ExternalUrls?.Spotify,
                ImageUrl = item.Images?.FirstOrDefault()?.Url,
                OwnerDisplayName = item.Owner?.DisplayName,
                OwnerId = item.Owner?.Id
            }).ToList();
        }

        public async Task<bool> ValidateTokenAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync($"{_spotifyApiBaseUrl}/me");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private class SpotifyTokenResponse
        {
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public string Scope { get; set; }
            public int ExpiresIn { get; set; }
            public string RefreshToken { get; set; }
        }

        private class SpotifyPlaylistsResponse
        {
            public List<SpotifyPlaylistItem> Items { get; set; } = new List<SpotifyPlaylistItem>();
            public int Total { get; set; }
            public int Limit { get; set; }
            public int Offset { get; set; }
        }

        private class SpotifyPlaylistItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Public { get; set; }
            public bool Collaborative { get; set; }
            public string SnapshotId { get; set; }
            public SpotifyTracksInfo Tracks { get; set; }
            public SpotifyExternalUrls ExternalUrls { get; set; }
            public List<SpotifyImage> Images { get; set; }
            public SpotifyOwner Owner { get; set; }
        }

        private class SpotifyTracksInfo
        {
            public int Total { get; set; }
        }

        private class SpotifyExternalUrls
        {
            public string Spotify { get; set; }
        }

        private class SpotifyImage
        {
            public string Url { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
        }

        private class SpotifyOwner
        {
            public string DisplayName { get; set; }
            public string Id { get; set; }
        }
    }
}