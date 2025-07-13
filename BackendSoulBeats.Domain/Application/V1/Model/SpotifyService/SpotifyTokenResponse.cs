namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyTokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
