using System.Text.Json.Serialization;

namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyPlaylistsResponse
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        
        [JsonPropertyName("next")]
        public string Next { get; set; }
        
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        
        [JsonPropertyName("previous")]
        public string Previous { get; set; }
        
        [JsonPropertyName("total")]
        public int Total { get; set; }
        
        [JsonPropertyName("items")]
        public List<SpotifyPlaylistItem> Items { get; set; } = new List<SpotifyPlaylistItem>();
    }
}
