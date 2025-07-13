namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    // Response model for Spotify /me API
    public class SpotifyUserResponse
    {
        public string Id { get; set; }
        public string Display_Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Product { get; set; }
        public SpotifyFollowers Followers { get; set; }
        public List<SpotifyImage> Images { get; set; }
        public SpotifyExternalUrls External_Urls { get; set; }
    }

    public class SpotifyFollowers
    {
        public int Total { get; set; }
    }
}
