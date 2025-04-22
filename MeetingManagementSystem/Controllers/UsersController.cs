using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> log, IUserService userService) : AbstractController
    {
        private readonly ILogger<UsersController> _log = log;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetUsers()
        {
            _log.LogTrace("Received request for all users");
            var users = _userService.GetAllUsers();
            var dtos = users.Select(u => new UserDTO(u));
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetById([FromRoute] int id)
        {
            _log.LogTrace("Received request for user by id, id={}", id);
            
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            } else
            {
                return Ok(new UserDTO(user));
            }
        }

        [HttpPost]
        public ActionResult<UserDTO> AddUser([FromBody] string userName)
        {
            _log.LogTrace("Received request to add user, userName={}", userName);
            
            try
            {
                var user = _userService.AddUser(userName);
                return Ok(new UserDTO(user));
            } catch (Exception e)
            {
                _log.LogError("Exception occurred while adding user, userName={}, e={}", userName, e);
                return MapError(e);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult<UserDTO> UpdateUserName([FromRoute] int id, [FromBody] string newName)
        {
            _log.LogTrace("Received request to update users name, id={}, newName={}", id, newName);

            try
            {
                var user = _userService.UpdateUserName(id, newName);
                return Ok(new UserDTO(user));
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while updating users name, id={}, newName={}, e={}", id, newName, e);
                return MapError(e);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveUser([FromRoute] int id)
        {
            _log.LogTrace("Received request to remove user, id= {}", id);

            try
            {
                _userService.RemoveUser(id);
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
