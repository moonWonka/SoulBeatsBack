using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateArtistPreferences
{
    public class UpdateArtistPreferencesRequest : IRequest<UpdateArtistPreferencesResponse>
    {
        public string FirebaseUid { get; set; }
        public List<ArtistPreferenceDto> Preferences { get; set; } = new();
    }

    public class ArtistPreferenceDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
    }
}