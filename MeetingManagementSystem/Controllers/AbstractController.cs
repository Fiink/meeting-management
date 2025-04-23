using MeetingManagementSystem.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    public class AbstractController : ControllerBase
    {

        /// <summary>
        /// Maps exceptions to a corresponding ActionResult
        /// </summary>
        /// <param name="e">Exception that should be mapped</param>
        /// <returns>A matching ActionResult</returns>
        protected ActionResult MapError(Exception e)
        {
            if (e is not ResultException resultException)
            {
                // Rethrow exception to cause an internal server error
                throw e;
            }

            string? message = resultException.ErrorMessage;
            return resultException.Type switch
            {
                ResultException.ExceptionType.NOT_FOUND => NotFound(message),
                ResultException.ExceptionType.CONFLICT => Conflict(message),
                ResultException.ExceptionType.PERSISTENCE_ERROR => Conflict(message),
                ResultException.ExceptionType.UNPROCESSABLE_ENTITY => UnprocessableEntity(message),
                _ => throw resultException // Throw an internal server error
            };
        }
    }
}
