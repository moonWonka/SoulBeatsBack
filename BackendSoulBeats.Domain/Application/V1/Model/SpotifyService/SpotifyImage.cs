using System.Text.Json.Serialization;

namespace BackendSoulBeats.Domain.Application.V1.Model.SpotifyService
{
    public class SpotifyImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        
        [JsonPropertyName("width")]
        public int? Width { get; set; }
    }
}
