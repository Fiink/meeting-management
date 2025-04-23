using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Data.Interfaces
{
    public interface IMeetingRoomRepository
    {
        Task<List<MeetingRoom>> GetAllAsync();
        Task<MeetingRoom?> GetMeetingRoomByIdAsync(int id);
        Task<bool> IsRoomNameInUseAsync(string name);
        Task<MeetingRoom> AddMeetingRoom(string name);
    }
}
