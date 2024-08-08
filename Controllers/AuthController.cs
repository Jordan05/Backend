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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userService.GetAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Crear un nuevo usuario
            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password // Asegúrate de que se maneje la seguridad de la contraseña correctamente
            };

            // Guardar el usuario en la base de datos
            await _userService.CreateAsync(newUser);

            // Generar un token para el nuevo usuario
            var token = _tokenService.GenerateToken(newUser);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetAsync(request.Username);
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
