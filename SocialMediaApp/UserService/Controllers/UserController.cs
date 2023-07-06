using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using UserService.Helpers;
using UserService.MessageBus;
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
        private IMessageBusClient _messageBusClient;
        string apiGatawayUri = "http://localhost:7000";

        public UserController(IUserService userService, IMessageBusClient messageBusClient)
        {
            _userService = userService;
            _messageBusClient = messageBusClient;
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
                var userDTO = new UserDTO { Username = user.Username, Name = user.Name, Email = user.Email, ProfilePicture = user.ProfilePicture };
                return Ok(userDTO);
            }
        }

        [HttpGet("/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userDTO = new UserDTO { Username = user.Username, Name = user.Name, Email = user.Email, ProfilePicture = user.ProfilePicture };
                return Ok(userDTO);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var username = HttpContext.GetUserId();
            var isSuccessful = await _userService.DeleteAsync(username);

            if (isSuccessful)
            {
                _messageBusClient.PublishDeleteUserEvent(username);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("/profile-picture")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePhotoRequest request)
        {
            var username = HttpContext.GetUserId();
            var filename = FileHelper.GetUniqueFileName(request.File.FileName);

            using MultipartFormDataContent multipartContent = new();
            multipartContent.Add(new StringContent(username, Encoding.UTF8, MediaTypeNames.Text.Plain), "Username");
            multipartContent.Add(new StringContent(filename, Encoding.UTF8, MediaTypeNames.Text.Plain), "Filename");

            byte[] data;
            using (var br = new BinaryReader(request.File.OpenReadStream()))
                data = br.ReadBytes((int)request.File.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);
            multipartContent.Add(bytes, "file", "File");

            string mediaServiceUri = apiGatawayUri + "/MediaService/api/Media";

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync(mediaServiceUri, multipartContent);

            return Ok();

        }

    }
}
