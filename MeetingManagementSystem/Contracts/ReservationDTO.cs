using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Contracts
{
    public class ReservationDTO(Reservation reservation)
    {
        public int Id { get; set; } = reservation.Id;
        public string? Name { get; set; } = reservation.MeetingName;
        public DateTimeOffset StartTime { get; set; } = reservation.StartTime;
        public DateTimeOffset EndTime { get; set; } = reservation.EndTime;
        public UserDTO Owner { get; set; } = new UserDTO(reservation.ReservationOwner);
        public int MeetingRoomId { get; set; } = reservation.MeetingRoomId;
        public string MeetingRoomName { get; set; } = reservation.MeetingRoom.RoomName;
        public IEnumerable<ParticipantDTO> Participants { get; set; } = reservation.Participants.Select(p => new ParticipantDTO(p));

        public override string ToString()
        {
            return $"ReservationDTO {{ " +
                   $"Id = {Id}, " +
                   $"Name = {Name ?? "null"}, " +
                   $"StartTime = {StartTime}, " +
                   $"EndTime = {EndTime}, " +
                   $"Owner = {Owner?.ToString() ?? "null"}, " +
                   $"MeetingRoomId = {MeetingRoomId}, " +
                   $"MeetingRoomName = {MeetingRoomName ?? "null"}, " +
                   $"Participants = {(Participants != null ? "[" + string.Join(", ", Participants.Select(p => p.ToString())) + "]" : "null")} " +
                   $"}}";
        }
    }
}
