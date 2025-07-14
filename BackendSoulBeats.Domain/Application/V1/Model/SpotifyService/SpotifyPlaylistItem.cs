using System.Text.Json.Serialization;

namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyPlaylistItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("public")]
        public bool Public { get; set; }
        
        [JsonPropertyName("collaborative")]
        public bool Collaborative { get; set; }
        
        [JsonPropertyName("snapshot_id")]
        public string SnapshotId { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        [JsonPropertyName("tracks")]
        public SpotifyTracksInfo Tracks { get; set; }
        
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls ExternalUrls { get; set; }
        
        [JsonPropertyName("images")]
        public List<SpotifyImage> Images { get; set; }
        
        [JsonPropertyName("owner")]
        public SpotifyOwner Owner { get; set; }
    }
}
