using MediatR;

namespace BackendSoulBeats.API.Application.V1.Command.PostAuth
{
    public class PostAuthHandler : IRequestHandler<PostAuthRequest, PostAuthResponse>
    {
        public async Task<PostAuthResponse> Handle(PostAuthRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new PostAuthResponse());
        }
    }
}