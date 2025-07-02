using BackendSoulBeats.API.Application.V1.ViewModel.Common;

namespace BackendSoulBeats.API.Application.V1.Query.GetUserPreferences
{
    public class GetUserPreferencesResponse : BaseResponse
    {
        public List<GenrePreferenceDto> GenrePreferences { get; set; } = new();
        public List<ArtistPreferenceDto> ArtistPreferences { get; set; } = new();
    }

    public class GenrePreferenceDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }

    public class ArtistPreferenceDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }
}