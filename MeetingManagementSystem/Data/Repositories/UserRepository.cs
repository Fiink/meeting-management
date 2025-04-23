using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Data.Repositories
{
    public class UserRepository(MeetingDbContext dbContext) : IUserRepository
    {
        private readonly MeetingDbContext _dbContext = dbContext;

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<List<User>> GetByIdsAsync(ICollection<int> participantIds)
        {
            return await (from user in _dbContext.Users
                          where participantIds.Contains(user.Id)
                          select user).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await (from user in _dbContext.Users
                          where id == user.Id
                          select user).SingleOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(string name)
        {
            var user = new User { Name = name };
            var userEntity = await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return userEntity.Entity;
        }

        public async Task UpdateUserAsync()
        {
            // This might not be the correct way to update users, this will likely update everything (e.g. rooms if we tracked those using this _dbContext)
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(User user)
        {
            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsNameInUseAsync(string name)
        {
            var lowercaseName = name.ToLower();
            return await _dbContext.Users.AnyAsync(user => user.Name.ToLower().Equals(name));
        }
    }
}
