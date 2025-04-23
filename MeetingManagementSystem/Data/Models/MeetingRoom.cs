using System.ComponentModel.DataAnnotations;

namespace MeetingManagementSystem.Data.Models
{
    public class MeetingRoom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoomName { get; set; }

        public ICollection<Reservation> Reservations { get; set; }


    }
}
