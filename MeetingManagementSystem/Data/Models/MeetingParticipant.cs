using System.ComponentModel.DataAnnotations;

namespace MeetingManagementSystem.Data.Models
{
    public class MeetingParticipant
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public int ParticipantId { get; set; }
        public User Participant { get; set; }
    }
}
