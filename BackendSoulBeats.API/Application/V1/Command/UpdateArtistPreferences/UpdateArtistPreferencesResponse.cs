using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateArtistPreferences
{
    public class UpdateArtistPreferencesResponse : BaseResponse
    {
        public int UpdatedCount { get; set; }
    }
}