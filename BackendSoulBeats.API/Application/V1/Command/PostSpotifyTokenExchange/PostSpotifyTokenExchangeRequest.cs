using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeRequest : BaseRequest
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }
    }
}