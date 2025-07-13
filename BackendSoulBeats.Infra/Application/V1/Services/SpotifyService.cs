using BackendSoulBeats.Domain.Application.V1.Model.SpotifyService;
using BackendSoulBeats.Domain.Application.V1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BackendSoulBeats.Infra.Application.V1.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SpotifyService> _logger;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _spotifyApiBaseUrl = "https://api.spotify.com/v1";
        private readonly string _spotifyTokenUrl = "https://accounts.spotify.com/api/token";

        public SpotifyService(HttpClient httpClient, IConfiguration configuration, ILogger<SpotifyService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _clientId = _configuration["Spotify:ClientId"];
            _clientSecret = _configuration["Spotify:ClientSecret"];
        }

        public async Task<SpotifyTokenModel> ExchangeCodeForTokenAsync(string code, string redirectUri)
        {
            // Validar parámetros de entrada
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Authorization code cannot be null or empty", nameof(code));
            if (string.IsNullOrEmpty(redirectUri))
                throw new ArgumentException("Redirect URI cannot be null or empty", nameof(redirectUri));

            // Crear el Basic Authentication header según la documentación de Spotify
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            // Crear el request body según la documentación
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)
            });

            // Configurar headers según la documentación de Spotify
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
            
            // Establecer Content-Type para el request
            tokenRequest.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(_spotifyTokenUrl, tokenRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                // Intentar deserializar el error de Spotify para obtener más detalles
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<SpotifyErrorResponse>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    var errorMessage = errorResponse?.Error ?? "unknown_error";
                    var errorDescription = errorResponse?.ErrorDescription ?? errorContent;
                    
                    throw new Exception($"Spotify API Error: {errorMessage} - {errorDescription}");
                }
                catch (JsonException)
                {
                    // Si no se puede deserializar, usar el contenido raw
                    throw new Exception($"Failed to exchange code for token: {errorContent}");
                }
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
            // Crear el Basic Authentication header
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            });

            // Configurar headers según la documentación de Spotify
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
            
            // Establecer Content-Type para el request
            tokenRequest.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(_spotifyTokenUrl, tokenRequest);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                // Intentar deserializar el error de Spotify para obtener más detalles
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<SpotifyErrorResponse>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    var errorMessage = errorResponse?.Error ?? "unknown_error";
                    var errorDescription = errorResponse?.ErrorDescription ?? errorContent;
                    
                    throw new Exception($"Spotify API Error: {errorMessage} - {errorDescription}");
                }
                catch (JsonException)
                {
                    // Si no se puede deserializar, usar el contenido raw
                    throw new Exception($"Failed to refresh token: {errorContent}");
                }
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

        public async Task<SpotifyUserProfileModel> GetUserProfileAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync($"{_spotifyApiBaseUrl}/me");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get user profile: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<SpotifyUserResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new SpotifyUserProfileModel
            {
                Id = userResponse.Id,
                DisplayName = userResponse.Display_Name,
                Email = userResponse.Email,
                Country = userResponse.Country,
                Product = userResponse.Product,
                Followers = userResponse.Followers?.Total ?? 0,
                ImageUrl = userResponse.Images?.FirstOrDefault()?.Url,
                ExternalUrl = userResponse.External_Urls?.Spotify
            };
        }
    }
}