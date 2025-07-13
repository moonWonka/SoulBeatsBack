using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostSpotifyTokenExchange
{
    public class PostSpotifyTokenExchangeRequest : IRequest<PostSpotifyTokenExchangeResponse>
    {
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        
        // Internal property set by controller, not from request body
        public string FirebaseUid { get; set; }
    }
}