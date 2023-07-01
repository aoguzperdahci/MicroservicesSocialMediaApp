using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Entities;
using UserService.Helpers;
using UserService.Models;

namespace UserService.Services
{
    public class AuthService : IAuthService
    {
        private UserDataContext _dataContext;
        private IJwtUtils _jwtUtils;

        public AuthService(UserDataContext dataContext, IJwtUtils jwtUtils)
        {
            _dataContext = dataContext;
            _jwtUtils = jwtUtils;
        }

        public async Task<string?> AuthenticateAsync(LoginRequest request)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Username == request.Username);

            // validate
            if (user == null || BCrypt.Net.BCrypt.HashPassword(request.Password, user.PasswordSalt) != user.PasswordHash)
            {
                return null;
            }

            // authentication successful
            var token = _jwtUtils.CreateToken(user);
            return token;
        }
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // validate
            if (await _dataContext.Users.AnyAsync(x => x.Username == request.Username))
                return false;

            // map model to new user object
            var user = new User { Username= request.Username, Email = request.Email, Name = request.Name };

            user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, user.PasswordSalt);

            // save user
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return true;
        }

    }
}
