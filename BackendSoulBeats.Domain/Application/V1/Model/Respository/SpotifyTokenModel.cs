namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    public class SpotifyTokenModel
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}