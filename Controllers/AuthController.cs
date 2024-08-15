using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;
using System.Threading.Tasks;
using BCrypt.Net;
using MongoDB.Bson;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUserByEmail = await _userService.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return Conflict("El email de usuario ya está en uso.");
            }

            var newUser = new User
            {
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                // Asigna otros campos necesarios, como Name y LastName
                
            };

            await _userService.CreateAsync(newUser);

            return Ok(new { Message = "Usuario registrado exitosamente" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }

    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
