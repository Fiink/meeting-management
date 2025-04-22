using System.ComponentModel.DataAnnotations;

namespace MeetingManagementSystem.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<MeetingParticipant> ParticipatingInMeetings { get; set; }
    }
}
