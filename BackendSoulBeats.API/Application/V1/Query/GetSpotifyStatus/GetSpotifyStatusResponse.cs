using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyStatus
{
    public class GetSpotifyStatusResponse : BaseResponse
    {
        public bool IsConnected { get; set; }
        public string SpotifyUserId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Product { get; set; }
        public int Followers { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalUrl { get; set; }
        public bool TokenValid { get; set; }
    }
}
