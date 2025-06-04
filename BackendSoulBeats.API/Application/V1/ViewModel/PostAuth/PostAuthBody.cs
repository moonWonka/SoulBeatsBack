using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendSoulBeats.API.Application.V1.ViewModel.PostAuth
{
    public class PostAuthBody
    {
        /// <summary>

        /// </summary>
        [Required]
        public string UserEmail { get; set; }

        /// <summary>
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}