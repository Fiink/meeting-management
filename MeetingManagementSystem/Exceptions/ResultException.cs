using Microsoft.AspNetCore.Http.HttpResults;

namespace MeetingManagementSystem.Exceptions
{
    /// <summary>
    /// Represents exceptions occurring when performing certain actions using services, e.g. when 
    /// retrieving elements or updating them
    /// </summary>
    public class ResultException : Exception
    {
        public enum ExceptionType
        {
            CONFLICT, NOT_FOUND, PERSISTENCE_ERROR, UNKNOWN
        };

        public ExceptionType Type { get; }
        public string? ErrorMessage { get; }

        public ResultException(ExceptionType type, string? errorMessage)
        {
            Type = type;
            ErrorMessage = errorMessage;
        }
    }
}
