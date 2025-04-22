using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Contracts
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public UserDTO(User user)
        {
            Id = user.Id;
            Name = user.Name;
        }
    }
}
