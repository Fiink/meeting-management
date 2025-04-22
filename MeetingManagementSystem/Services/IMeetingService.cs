using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Services
{
    public interface IMeetingService
    {
        IEnumerable<Reservation> GetAllReservations(bool includeExpired = false);
        IEnumerable<Reservation> GetReservationsByOwner(int ownerId, bool includeExpired = false);
        IEnumerable<Reservation> GetReservationsForParticipant(int participantId, bool includeExpired = false);
        Reservation AddReservation(int ownerId, int meetingRoomId, string? meetingName, DateTimeOffset startTime,
            DateTimeOffset endTime, ICollection<int>? participantIds);
    }
}
