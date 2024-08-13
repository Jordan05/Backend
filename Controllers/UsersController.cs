using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService) =>
            _userService = userService;

        [HttpGet]
        [Authorize] // Requiere autenticación
        public async Task<List<User>> Get() => await _userService.GetAsync();

        [HttpGet("{id}")]
        [Authorize] // Requiere autenticación
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _userService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Authorize] // Requiere autenticación
        public async Task<IActionResult> Post(User newUser)
        {
            await _userService.CreateAsync(newUser);
            var createdUser = newUser;

            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize] // Requiere autenticación
        public async Task<IActionResult> Update(int id, User updatedUser)
        {
            var user = await _userService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            updatedUser.Id = user.Id;

            await _userService.UpdateAsync(id, updatedUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize] // Requiere autenticación
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            await _userService.RemoveAsync(id);

            return NoContent();
        }
    }
}
