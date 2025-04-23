using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services.Implementations
{
    public class UserService(ILogger<UserService> logger, IUserRepository userRepository) : IUserServiceAsync
    {
        private readonly ILogger<UserService> _log = logger;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<List<User>> GetUsersByIdsAsync(ICollection<int> participantIds)
        {
            return await _userRepository.GetByIdsAsync(participantIds);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> AddUserAsync(string name)
        {
            if (await _userRepository.IsNameInUseAsync(name))
            {
                _log.LogError("User with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "User with provided name already exists");
            }

            try
            {
                return await _userRepository.AddUserAsync(name);
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while saving new user, name={}, e={}", name, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error persisting new user");
            }
        }

        public async Task<User> UpdateUserNameAsync(int id, string newName)
        {
            if (await _userRepository.IsNameInUseAsync(newName))
            {
                _log.LogError("User with name already exists, newName={}", newName);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "User with provided name already exists");
            }

            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                _log.LogError("User with id {} not found", id);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Could not find user with provided id");
            }

            user.Name = newName;
            try
            {
                await _userRepository.UpdateUserAsync();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while saving updated user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error persisting updated user");
            }
            return user;
        }

        public async Task RemoveUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                _log.LogError("User with id {} not found", id);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Could not find user with provided id");
            }

            try
            {
                await _userRepository.RemoveUserAsync(user);
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while removing user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error removing user");
            }
        }
    }
}
