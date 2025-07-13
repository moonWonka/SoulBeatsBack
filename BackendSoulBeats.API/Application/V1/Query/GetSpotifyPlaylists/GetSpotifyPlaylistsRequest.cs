using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists
{
    public class GetSpotifyPlaylistsRequest : BaseRequest
    {
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}