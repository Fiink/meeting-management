using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Contracts
{
    public class ParticipantDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ParticipantDTO(MeetingParticipant participant)
        {
            Id = participant.ParticipantId;
            Name = participant.Participant.Name;
        }

        public override string ToString()
        {
            return $"ParticipantDTO {{ " +
                   $"Id = {Id}, " +
                   $"Name = {Name}" +
                   $"}}";
        }
    }
}
