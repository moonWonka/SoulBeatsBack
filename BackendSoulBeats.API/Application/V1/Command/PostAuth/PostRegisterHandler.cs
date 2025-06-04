using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostRegister
{
    public class PostRegisterHandler : IRequestHandler<PostRegisterRequest, PostRegisterResponse>
    {
        public async Task<PostRegisterResponse> Handle(PostRegisterRequest request, CancellationToken cancellationToken)
        {
            // Registrar a new user
            return await Task.FromResult(new PostRegisterResponse());
        }
    }
}