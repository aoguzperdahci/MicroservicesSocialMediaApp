using UserService.Models;

namespace UserService.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);

    }
}
