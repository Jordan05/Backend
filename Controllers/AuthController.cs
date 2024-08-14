using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;
using System.Threading.Tasks;
using BCrypt.Net; // Asegúrate de haber instalado BCrypt.Net-Next

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
            // Verifica si el usuario ya existe por ID
            var existingUserById = await _userService.GetAsync(request.Id);
            if (existingUserById != null)
            {
                return Conflict("El ID de usuario ya está en uso.");
            }

            // Verifica si el usuario ya existe por nombre
            var existingUserByName = await _userService.GetByNameAsync(request.Email);
            if (existingUserByName != null)
            {
                return Conflict("El email de usuario ya está en uso.");
            }

            // Crea un nuevo usuario
            var newUser = new User
            {
                Id = request.Id,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password) // Encripta la contraseña
            };

            await _userService.CreateAsync(newUser);

            return Ok(new { Message = "Usuario registrado exitosamente" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetAsync(request.Id);
            if (user == null || user.Email != request.Email || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }

    public class RegisterRequest
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
