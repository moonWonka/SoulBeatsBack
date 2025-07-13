using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists
{
    public class GetSpotifyPlaylistsRequest : IRequest<GetSpotifyPlaylistsResponse>
    {
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
        public string FirebaseUid { get; set; }
    }
}