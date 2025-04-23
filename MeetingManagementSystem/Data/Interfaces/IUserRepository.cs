using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetByIdsAsync(ICollection<int> participantIds);
        Task<User?> GetByIdAsync(int id);
        Task<User> AddUserAsync(string name);
        Task UpdateUserAsync();
        Task RemoveUserAsync(User user);
        Task<bool> IsNameInUseAsync(string name);
    }
}
