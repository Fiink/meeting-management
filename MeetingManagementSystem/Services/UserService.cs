using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services
{
    public class UserService(ILogger<UserService> logger, MeetingDbContext dbContext) : IUserService
    {
        private readonly ILogger<UserService> _log = logger;
        private readonly MeetingDbContext _dbContext = dbContext;

        public ICollection<User> GetAllUsers()
        {
            return [.. _dbContext.Users];
        }

        public User? GetUserById(int id)
        {
            return (from user in _dbContext.Users
                    where id == user.Id
                    select user).SingleOrDefault();
        }

        public User AddUser(string name)
        {
            if (IsNameInUse(name))
            {
                _log.LogError("User with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "User with provided name already exists");
            }

            var newUser = new User
            {
                Name = name
            };
            var user = _dbContext.Users.Add(newUser);
            
            try
            {
                _dbContext.SaveChanges();
            } catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while saving new user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error persisting new user");
            }
            return user.Entity;
        }

        public User UpdateUserName(int id, string newName)
        {
            if (IsNameInUse(newName))
            {
                _log.LogError("User with name already exists, newName={}", newName);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "User with provided name already exists");
            }

            var user = GetUserById(id);
            if (user == null)
            {
                _log.LogError("User with id {} not found", id);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Could not find user with provided id");
            }

            user.Name = newName;
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while saving updated user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error persisting updated user");
            }
            return user;
        }

        public void RemoveUser(int id)
        {
            var user = GetUserById(id);
            if (user == null)
            {
                _log.LogError("User with id {} not found", id);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Could not find user with provided id");
            }

            _dbContext.Remove(user);
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while removing user, user={}, e={}", user, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error removing user");
            }
        }

        public ICollection<User> GetUsersByIds(ICollection<int> participantIds)
        {
            return [.. (from user in _dbContext.Users
                   where participantIds.Contains(user.Id)
                   select user)];
        }

        private bool IsNameInUse(string name)
        {
            return _dbContext.Users.Any(user => user.Name.Contains(name));
        }
    }
}
