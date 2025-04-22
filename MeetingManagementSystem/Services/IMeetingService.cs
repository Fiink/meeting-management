using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Services
{
    public interface IMeetingService
    {
        List<Reservation> GetAllReservations(bool includeExpired = false);
        List<Reservation> GetReservationsByOwner(int ownerId, bool includeExpired = false);
        List<Reservation> GetReservationsForParticipant(int participantId, bool includeExpired = false);
        Reservation AddReservation(int ownerId, int meetingRoomId, string? meetingName, DateTimeOffset startTime,
            DateTimeOffset endTime, ICollection<int>? participantIds);
        List<MeetingRoom> GetAllMeetingRooms();
        MeetingRoom AddMeetingRoom(string name);
    }
}
