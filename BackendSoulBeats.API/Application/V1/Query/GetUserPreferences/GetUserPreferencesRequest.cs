using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query.GetUserPreferences
{
    public class GetUserPreferencesRequest : IRequest<GetUserPreferencesResponse>
    {
        public string FirebaseUid { get; set; }
    }
}