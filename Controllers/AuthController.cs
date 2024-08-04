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
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetAsync(request.Name);
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
        public  required string Name { get; set; }
        public required string Password { get; set; }
    }
}
