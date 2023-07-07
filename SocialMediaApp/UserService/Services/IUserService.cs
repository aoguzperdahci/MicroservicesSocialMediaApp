using UserService.Entities;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> DeleteAsync(string username);
        Task UpdateUserProfilePictureAsync(string username, string imageUrl);
    }
}
