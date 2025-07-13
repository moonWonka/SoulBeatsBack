namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyPlaylistItem
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
}
