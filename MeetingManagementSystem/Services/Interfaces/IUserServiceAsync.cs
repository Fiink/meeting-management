using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Services.Interfaces
{
    public interface IUserServiceAsync
    {
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByIdsAsync(ICollection<int> participantIds);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(string name);
        Task<User> UpdateUserNameAsync(int id, string newName);
        Task RemoveUserAsync(int id);
    }
}
