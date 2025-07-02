using MediatR;
using BackendSoulBeats.API.Application.V1.ViewModel.Shared;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateGenrePreferences
{
    public class UpdateGenrePreferencesRequest : IRequest<UpdateGenrePreferencesResponse>
    {
        public string FirebaseUid { get; set; }
        public List<GenrePreferenceDto> Preferences { get; set; } = new();
    }
}