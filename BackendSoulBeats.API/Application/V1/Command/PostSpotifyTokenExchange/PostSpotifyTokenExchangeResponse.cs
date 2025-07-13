using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeResponse : BaseResponse
    {
        public bool IsConnected { get; set; }
        public string SpotifyUserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}