using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Data.Repositories
{
    public class ReservationRepository(MeetingDbContext dbContext) : IReservationRepository
    {
        private readonly MeetingDbContext _dbContext = dbContext;

        public async Task<List<Reservation>> GetAllAsync(bool includeExpired = false)
        {
            IQueryable<Reservation> query = _dbContext.Reservations;
            if (!includeExpired)
            {
                query = FilterRemoveExpiredReservations(query);
            }
            return await query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByOwnerAsync(int ownerId, bool includeExpired = false)
        {
            IQueryable<Reservation> query = _dbContext.Reservations
                .Where(r => ownerId == r.ReservationOwnerId);
            if (!includeExpired)
            {
                query = FilterRemoveExpiredReservations(query);
            }
            return await query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsForParticipantAsync(int participantId, bool includeExpired = false)
        {
            var query = _dbContext.Users
                .Where(user => user.Id == participantId)
                .Join(
                    _dbContext.MeetingParticipants,
                    user => user.Id,
                    participant => participant.ParticipantId,
                    (user, participant) => participant
                )
                .Join(
                    _dbContext.Reservations,
                    participant => participant.ReservationId,
                    reservation => reservation.Id,
                    (participant, reservation) => reservation
                );
            if (!includeExpired)
            {
                query = FilterRemoveExpiredReservations(query);
            }
            return await query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .ToListAsync();
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            var reservationEntry = await _dbContext.AddAsync(reservation);
            foreach (var participant in reservationEntry.Entity.Participants)
            {
                await _dbContext.AddAsync(participant);
            }
            await _dbContext.SaveChangesAsync();
            return reservationEntry.Entity;
        }

        public async Task<bool> IsRoomOccupiedInTimeslotAsync(MeetingRoom room, TimeRange time)
        {
            return await _dbContext.Reservations
                .Where(r => r.MeetingRoomId == room.Id)
                // Does time range overlap with the reservation?
                .Where(reservation => reservation.StartTime < time.EndTime && time.StartTime < reservation.EndTime)
                .AnyAsync();
        }

        /// <summary>
        /// Will filter reservations that have expired w.r.t. the current system time.
        /// </summary>
        /// <returns>Query with filter applied.</returns>
        private static IQueryable<Reservation> FilterRemoveExpiredReservations(IQueryable<Reservation> query)
        {
            return query.Where(reservation => DateTimeOffset.Now < reservation.EndTime);
        }
    }
}
