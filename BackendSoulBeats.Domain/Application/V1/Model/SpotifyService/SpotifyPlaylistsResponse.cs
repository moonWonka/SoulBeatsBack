namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyPlaylistsResponse
    {
        public List<SpotifyPlaylistItem> Items { get; set; } = new List<SpotifyPlaylistItem>();
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
