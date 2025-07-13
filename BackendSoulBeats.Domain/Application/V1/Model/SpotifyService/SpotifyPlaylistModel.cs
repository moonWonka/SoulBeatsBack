namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyPlaylistModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Public { get; set; }
        public bool Collaborative { get; set; }
        public string SnapshotId { get; set; }
        public int TracksTotal { get; set; }
        public string ExternalUrl { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerDisplayName { get; set; }
        public string OwnerId { get; set; }
    }
}
