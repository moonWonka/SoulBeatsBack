using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using BackendSoulBeats.API.Application.V1.ViewModel.Shared;

namespace BackendSoulBeats.API.Application.V1.Query.GetUserPreferences
{
    public class GetUserPreferencesResponse : BaseResponse
    {
        public List<GenrePreferenceDto> GenrePreferences { get; set; } = new();
        public List<ArtistPreferenceDto> ArtistPreferences { get; set; } = new();
    }
}