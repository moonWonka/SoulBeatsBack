using System.Text.Json.Serialization;

namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyOwner
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls ExternalUrls { get; set; }
    }
}
