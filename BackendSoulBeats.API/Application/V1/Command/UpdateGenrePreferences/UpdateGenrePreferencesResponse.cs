using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateGenrePreferences
{
    public class UpdateGenrePreferencesResponse : BaseResponse
    {
        public int UpdatedCount { get; set; }
    }
}