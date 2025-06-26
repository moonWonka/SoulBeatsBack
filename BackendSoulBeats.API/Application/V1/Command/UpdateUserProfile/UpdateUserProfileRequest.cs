using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.UpdateUserProfile
{
    public class UpdateUserProfileRequest : IRequest<UpdateUserProfileResponse>
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string Bio { get; set; }
        public string FavoriteGenres { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
