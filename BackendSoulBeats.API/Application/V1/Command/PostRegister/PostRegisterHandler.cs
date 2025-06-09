using BackendSoulBeats.Domain.Application.V1.Services;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostRegister
{
    public class PostRegisterHandler : IRequestHandler<PostRegisterRequest, PostRegisterResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        
        public PostRegisterHandler(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public async Task<PostRegisterResponse> Handle(PostRegisterRequest request, CancellationToken cancellationToken)
        {
            // Registrar a new user
            string newUser = await _googleAuthService.RegisterUserAsync(request.UserEmail, request.Password);

            return await Task.FromResult(new PostRegisterResponse
            {
                 MoreInformation = newUser
            });
        }
    }
}