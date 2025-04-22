using System.ComponentModel.DataAnnotations;

namespace MeetingManagementSystem.Data.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        public string? MeetingName { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        public int ReservationOwnerId { get; set; }
    
        public User ReservationOwner { get; set; }

        public int MeetingRoomId { get; set; }

        public MeetingRoom MeetingRoom { get; set; }

        public ICollection<MeetingParticipant> Participants { get; set; }
    }
}
