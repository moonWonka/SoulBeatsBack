using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query
{
    public class GetUserInfoHandler : IRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
    {
        public async Task<GetUserInfoResponse> Handle(GetUserInfoRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new GetUserInfoResponse());
        }
    }
    
}