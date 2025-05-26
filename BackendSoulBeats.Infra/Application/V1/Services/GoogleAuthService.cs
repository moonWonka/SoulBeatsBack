using BackendSoulBeats.Domain.Application.V1.Services;

namespace BackendSoulBeats.Infra.Application.V1.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public GoogleAuthService()
        {
        }

        public Task<bool> IsUserRegisteredAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterUserAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}