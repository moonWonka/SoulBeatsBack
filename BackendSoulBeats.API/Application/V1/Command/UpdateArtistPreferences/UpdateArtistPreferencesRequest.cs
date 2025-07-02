using MediatR;
using BackendSoulBeats.API.Application.V1.ViewModel.Shared;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateArtistPreferences
{
    public class UpdateArtistPreferencesRequest : IRequest<UpdateArtistPreferencesResponse>
    {
        public string FirebaseUid { get; set; }
        public List<ArtistPreferenceDto> Preferences { get; set; } = new();
    }
}