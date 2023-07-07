using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Entities;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private UserDataContext _dataContext;

        public UserService(UserDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> DeleteAsync(string username)
        {
            var user = await GetByUsernameAsync(username);

            if (user == null)
            {
                return false;
            }

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dataContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task UpdateUserProfilePictureAsync(string username, string imageUrl)
        {
            var user = await GetByUsernameAsync(username);

            if (user == null)
            {
                return;
            }

            user.ProfilePicture = imageUrl;
            await _dataContext.SaveChangesAsync();
        }
    }
}
