using UserService.Entities;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> DeleteAsync(string username);
    }
}
