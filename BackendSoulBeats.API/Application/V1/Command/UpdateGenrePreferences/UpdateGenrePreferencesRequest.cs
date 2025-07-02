using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateGenrePreferences
{
    public class UpdateGenrePreferencesRequest : IRequest<UpdateGenrePreferencesResponse>
    {
        public string FirebaseUid { get; set; }
        public List<GenrePreferenceDto> Preferences { get; set; } = new();
    }

    public class GenrePreferenceDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }
}