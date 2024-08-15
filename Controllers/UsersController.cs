using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;
using MongoDB.Bson;

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
        [Authorize]
        public async Task<List<User>> Get() => await _userService.GetAsync();

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> Get(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userService.GetAsync(objectId);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(User newUser)
        {
            await _userService.CreateAsync(newUser);
            var createdUser = newUser;

            return CreatedAtAction(nameof(Get), new { id = createdUser.Id.ToString() }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, User updatedUser)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userService.GetAsync(objectId);

            if (user is null)
            {
                return NotFound();
            }

            updatedUser.Id = objectId;

            await _userService.UpdateAsync(objectId, updatedUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userService.GetAsync(objectId);

            if (user is null)
            {
                return NotFound();
            }

            await _userService.RemoveAsync(objectId);

            return NoContent();
        }
    }
}
