using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        List<User> GetUsersByIds(ICollection<int> participantIds);
        User? GetUserById(int id);
        User AddUser(string name);
        User UpdateUserName(int id, string newName);
        void RemoveUser(int id);
    }
}
