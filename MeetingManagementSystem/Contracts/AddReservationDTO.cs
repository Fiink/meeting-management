namespace MeetingManagementSystem.Contracts
{
    public class AddReservationDTO
    {
        public int OwnerId { get; set; }
        public int MeetingRoomId { get; set; }
        public string? MeetingName { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public ICollection<int>? ParticipantIds { get; set; }

        public override string ToString()
        {
            return $"AddReservationDTO {{ " +
                   $"OwnerId = {OwnerId}, " +
                   $"MeetingRoomId = {MeetingRoomId}, " +
                   $"MeetingName = {MeetingName ?? "null"}, " +
                   $"StartTime = {StartTime}, " +
                   $"EndTime = {EndTime}, " +
                   $"ParticipantIds = {(ParticipantIds != null ? "[" + string.Join(", ", ParticipantIds) + "]" : "null")} " +
                   $"}}";
        }
    }
}
