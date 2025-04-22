using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Services
{
    public interface IUserService
    {
        ICollection<User> GetAllUsers();
        User? GetUserById(int id);
        User AddUser(string name);
        User UpdateUserName(int id, string newName);
        void RemoveUser(int id);
    }
}
