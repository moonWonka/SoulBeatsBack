namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyUserProfileModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Product { get; set; } // premium, free, etc.
        public int Followers { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalUrl { get; set; }
    }
}
