using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Models;

namespace MeetingManagementSystem.Data.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAllAsync(bool includeExpired);
        Task<List<Reservation>> GetReservationsByOwnerAsync(int ownerId, bool includeExpired = false);
        Task<List<Reservation>> GetReservationsForParticipantAsync(int participantId, bool includeExpired = false);
        Task<Reservation> AddReservationAsync(Reservation reservation);
        Task<bool> IsRoomOccupiedInTimeslotAsync(MeetingRoom room, TimeRange time);
    }
}
