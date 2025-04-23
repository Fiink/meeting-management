using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services
{
    public class UserService(ILogger<UserService> logger, MeetingDbContext dbContext) : IUserServiceAsync
    {
        private readonly ILogger<UserService> _log = logger;
        private readonly MeetingDbContext _dbContext = dbContext;

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<List<User>> GetUsersByIdsAsync(ICollection<int> participantIds)
        {
            return await (from user in _dbContext.Users
                          where participantIds.Contains(user.Id)
                          select user).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await (from user in _dbContext.Users
                          where id == user.Id
                          select user).SingleOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(string name)
        {
            if (await IsNameInUseAsync(name))
            {
                _log.LogError("User with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "User with provided name already exists");
            }

            var user = new User
            {
                Name = name
            };
            var userEntity = await _dbContext.Users.AddAsync(user);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while saving new user, user={}, e={}", userEntity, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error persisting new user");
            }
            return userEntity.Entity;
        }

        public async Task<User> UpdateUserNameAsync(int id, string newName)
        {
            if (await IsNameInUseAsync(newName))
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
                await _dbContext.SaveChangesAsync();
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

            _dbContext.Remove(user);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while removing user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error removing user");
            }
        }

        private Task<bool> IsNameInUseAsync(string name)
        {
            return _dbContext.Users.AnyAsync(user => user.Name.Contains(name));
        }
    }
}
