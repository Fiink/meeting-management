using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingManagementSystem.Data.Models
{
    public class MeetingParticipant
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public int ParticipantId { get; set; }
        public User Participant { get; set; }

        public MeetingParticipant(Reservation reservation, User participant)
        {
            ReservationId = reservation.Id;
            Reservation = reservation;
            ParticipantId = participant.Id;
            Participant = participant;
        }

        public MeetingParticipant()
        {
        }
    }
}
