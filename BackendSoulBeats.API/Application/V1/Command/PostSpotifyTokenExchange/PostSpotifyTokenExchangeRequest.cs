using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeRequest : IRequest<PostSpotifyTokenExchangeResponse>
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }
        public string FirebaseUid { get; set; }
    }
}