using BackendSoulBeats.API.Application.V1.ViewModel.Common;
using BackendSoulBeats.API.Application.V1.ViewModel.PostAuth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.Command.PostAuth
{
    public class PostAuthRequest : IRequest<PostAuthResponse>
    {
        internal HeaderViewModel Header { get; set; }

        public PostAuthRequest() {}

        public PostAuthRequest(HeaderViewModel header)
        {
            Header = header;
        }

        /// <summary>
        /// Cuerpo de la solicitud de autenticación.
        /// </summary>
        [FromBody]
        public PostAuthBody Body { get; set; }
    }
}