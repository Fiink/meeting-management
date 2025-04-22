using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Contracts
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public UserDTO Owner { get; set; }
        public int MeetingRoomId { get; set; }
        public string MeetingRoomName { get; set; }
        public IEnumerable<ParticipantDTO> Participants { get; set; }

        public ReservationDTO(Reservation reservation)
        {
            Id = reservation.Id;
            Name = reservation.MeetingName;
            StartTime = reservation.StartTime;
            EndTime = reservation.EndTime;
            Owner = new UserDTO(reservation.ReservationOwner);
            MeetingRoomId = reservation.MeetingRoomId;
            MeetingRoomName = reservation.MeetingRoom.RoomName;
            Participants = reservation.Participants.Select(p => new ParticipantDTO(p));
        }

        public override string ToString()
        {
            return $"ReservationDTO {{ " +
                   $"Id = {Id}, " +
                   $"Name = {Name ?? "null"}, " +
                   $"StartTime = {StartTime.ToString()}, " +
                   $"EndTime = {EndTime.ToString()}, " +
                   $"Owner = {Owner?.ToString() ?? "null"}, " +
                   $"MeetingRoomId = {MeetingRoomId}, " +
                   $"MeetingRoomName = {MeetingRoomName ?? "null"}, " +
                   $"Participants = {(Participants != null ? "[" + string.Join(", ", Participants.Select(p => p.ToString())) + "]" : "null")} " +
                   $"}}";
        }
    }
}
