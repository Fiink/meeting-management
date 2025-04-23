using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> log, IUserServiceAsync userService) : AbstractController
    {
        private readonly ILogger<UsersController> _log = log;
        private readonly IUserServiceAsync _userService = userService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            _log.LogTrace("Received request for all users");
            var users = await _userService.GetAllUsersAsync();
            var dtos = users.Select(u => new UserDTO(u));
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById([FromRoute] int id)
        {
            _log.LogTrace("Received request for user by id, id={}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new UserDTO(user));
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> AddUser([FromBody] string userName)
        {
            _log.LogTrace("Received request to add user, userName={}", userName);

            try
            {
                var user = await _userService.AddUserAsync(userName);
                return Ok(new UserDTO(user));
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while adding user, userName={}, e={}", userName, e);
                return MapError(e);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUserName([FromRoute] int id, [FromBody] string newName)
        {
            _log.LogTrace("Received request to update users name, id={}, newName={}", id, newName);

            try
            {
                var user = await _userService.UpdateUserNameAsync(id, newName);
                return Ok(new UserDTO(user));
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while updating users name, id={}, newName={}, e={}", id, newName, e);
                return MapError(e);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int id)
        {
            _log.LogTrace("Received request to remove user, id= {}", id);

            try
            {
                await _userService.RemoveUserAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while removing user, id={}, e={}", id, e);
                return MapError(e);
            }
        }
    }
}
