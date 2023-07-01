using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.MessageBus;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private IMessageBusClient _messageBusClient;

        public AuthController(IAuthService authService, IMessageBusClient messageBusClient)
        {
            _authService = authService;
            _messageBusClient = messageBusClient;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.AuthenticateAsync(request);
            return response == null ? NotFound() : Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            bool isSuccessful = await _authService.RegisterAsync(request);
            if (isSuccessful)
            {
                _messageBusClient.PublishCreateUserEvent(request.Username);
                return StatusCode(201);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
