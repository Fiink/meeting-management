using MeetingManagementSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

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

        public UserDTO()
        {
        }
    }
}
