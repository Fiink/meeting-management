using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Contracts
{
    public class MeetingRoomDTO(MeetingRoom meetingRoom)
    {
        public int Id { get; set; } = meetingRoom.Id;
        public string Name { get; set; } = meetingRoom.RoomName;

        public override string ToString()
        {
            return $"MeetingRoomDTO {{ " +
                   $"Id = {Id}, " +
                   $"Name = {Name}" +
                   $"}}";
        }
    }
}
