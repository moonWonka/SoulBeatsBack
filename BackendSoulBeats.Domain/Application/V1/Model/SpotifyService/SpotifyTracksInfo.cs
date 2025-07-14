using System.Text.Json.Serialization;

namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyTracksInfo
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
