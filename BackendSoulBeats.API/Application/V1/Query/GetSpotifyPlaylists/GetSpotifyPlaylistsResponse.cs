using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using BackendSoulBeats.Domain.Application.V1.Model.Respository;

namespace BackendSoulBeats.API.Application.V1.Query.GetSpotifyPlaylists
{
    public class GetSpotifyPlaylistsResponse : BaseResponse
    {
        public List<SpotifyPlaylistModel> Playlists { get; set; } = new List<SpotifyPlaylistModel>();
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}