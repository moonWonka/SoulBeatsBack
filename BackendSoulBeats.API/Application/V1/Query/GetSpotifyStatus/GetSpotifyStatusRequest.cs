using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyStatus
{
    public class GetSpotifyStatusRequest : IRequest<GetSpotifyStatusResponse>
    {
        // Internal property set by controller, not from request body
        public string FirebaseUid { get; set; }
    }
}
