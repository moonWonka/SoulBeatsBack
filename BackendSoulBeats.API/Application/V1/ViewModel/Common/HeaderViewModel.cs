using Microsoft.AspNetCore.Mvc;

namespace BackendSoulBeats.API.Application.V1.ViewModel.Common
{
    public class HeaderViewModel
    {
        [FromHeader(Name = "Authorization")]
        public string? Authorization { get; set; }
    
        [FromHeader(Name = "api-version")]
        public string? XApiVersion { get; set; }
    }
}