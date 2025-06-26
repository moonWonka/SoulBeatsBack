namespace BackendSoulBeats.Domain.Application.V1.Model.Respository
{
    public class UserInfoReponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}