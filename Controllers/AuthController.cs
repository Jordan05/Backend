using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyApi.Models;
using MyApi.Services;
using System.Threading.Tasks;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserService _userService;

        public AuthController(UserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetAsync(request.UserName);
            if (user == null || user.Password != request.Password) // Assuming you have a password property
            {
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
