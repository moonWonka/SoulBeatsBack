using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using MediatR;

namespace BackendSoulBeats.API.Application.V1.Query
{
    public class GetUserInfoRequest : IRequest<GetUserInfoResponse>
    {
        internal HeaderViewModel Header { get; set; }
        public GetUserInfoRequest() { }
        public GetUserInfoRequest(HeaderViewModel header)
        {
            Header = header;
        }
        public string UserId { get; set; } = string.Empty;
    }
}