using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _log;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> log, IUserService userService)
        {
            _log = log;
            _userService = userService;
        }

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
            } catch (ResultException e)
            {
                return MapError(e);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult<UserDTO> UpdateUserName([FromRoute] int id, [FromBody] string newName)
        {
            _log.LogTrace("Received request to update users name, id= {}, newName={}", id, newName);

            try
            {
                var user = _userService.UpdateUserName(id, newName);
                return Ok(new UserDTO(user));
            }
            catch (ResultException e)
            {
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
            catch (ResultException e)
            {
                return MapError(e);
            }
        }

        /// <summary>
        /// Maps exceptions to a corresponding ActionResult
        /// </summary>
        /// <param name="e">Exception that should be mapped</param>
        /// <returns>A matching ActionResult</returns>
        private ActionResult MapError(Exception e)
        {
            if (!(e is ResultException resultException))
            {
                // Rethrow exception to cause an internal server error
                throw e;
            }

            return resultException.Type switch
            {
                ResultException.ExceptionType.NOT_FOUND => NotFound(resultException.ErrorMessage),
                ResultException.ExceptionType.CONFLICT => Conflict(resultException.ErrorMessage),
                ResultException.ExceptionType.PERSISTENCE_ERROR => Conflict(resultException.ErrorMessage),
                _ => throw resultException // Throw an internal server error
            };

        }
    }
}
