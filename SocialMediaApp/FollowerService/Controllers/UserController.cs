using Microsoft.AspNetCore.Mvc;
using FollowerService.Services;
using Microsoft.AspNetCore.Authorization;
using FollowerService.Helpers;

namespace FollowerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IFollowerService followerService;

        public UserController(IFollowerService followerService)
        {
            this.followerService = followerService;
        }

        [HttpPost("unfollow/{followeeUsername}")]
        public async Task<IActionResult> Unfollow(string followeeUsername)
        {
            await followerService.UnfollowUser(HttpContext.GetUserId(), followeeUsername);
            return Ok();
        }

        [HttpPost("follow/{followeeUsername}")]
        public async Task<IActionResult> FollowUser(string followeeUsername)
        {
            await followerService.FollowUser(HttpContext.GetUserId(), followeeUsername);
            return Ok();
        }

        [HttpGet("following/{username}")]
        [AllowAnonymous]
        
        public async Task<ActionResult> GetFollowing(string username)
        {
            var results = await followerService.GetFollowing(username);
            return Ok(results);
        }

        [HttpGet("followers/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFollowers(string username)
        {
            var results = await followerService.GetFollowers(username);
            return Ok(results);
        }

    }
}



