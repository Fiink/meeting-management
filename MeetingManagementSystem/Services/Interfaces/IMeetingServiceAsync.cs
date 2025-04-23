using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Models;

namespace MeetingManagementSystem.Services.Interfaces
{
    public interface IMeetingServiceAsync
    {
        Task<List<Reservation>> GetAllReservationsAsync(bool includeExpired = false);
        Task<List<Reservation>> GetReservationsByOwnerAsync(int ownerId, bool includeExpired = false);
        Task<List<Reservation>> GetReservationsForParticipantAsync(int participantId, bool includeExpired = false);
        Task<Reservation> AddReservationAsync(int ownerId, int meetingRoomId, string? meetingName, TimeRange time, ICollection<int>? participantIds);
        Task<List<MeetingRoom>> GetAllMeetingRoomsAsync();
        Task<MeetingRoom> AddMeetingRoomAsync(string name);
    }
}
