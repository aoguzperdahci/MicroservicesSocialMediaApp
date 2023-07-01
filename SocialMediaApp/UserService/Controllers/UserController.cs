using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Helpers;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userService.GetByUsernameAsync(HttpContext.GetUserId());

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userDTO = new UserDTO { Username = user.Username, Name = user.Name, Email = user.Email };
                return Ok(userDTO);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var isSuccessful = await _userService.DeleteAsync(HttpContext.GetUserId());

            if (isSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
