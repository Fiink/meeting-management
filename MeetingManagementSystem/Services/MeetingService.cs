using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services
{
    public class MeetingService(ILogger<MeetingService> log, MeetingDbContext dbContext, IUserServiceAsync userService) : IMeetingServiceAsync
    {
        private readonly ILogger<MeetingService> _log = log;
        private readonly MeetingDbContext _dbContext = dbContext;
        private readonly IUserServiceAsync _userService = userService;

        public async Task<List<Reservation>> GetAllReservationsAsync(bool includeExpired = false)
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

        public async Task<Reservation> AddReservationAsync(int ownerId, int meetingRoomId, string? meetingName, TimeRange time, ICollection<int>? participantIds)
        {
            var room = await GetMeetingRoomByIdAsync(meetingRoomId);
            if (room == null)
            {
                _log.LogError("Failed to add reservation: meeting room not found, meetingRoomId={}", meetingRoomId);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Meeting room not found");
            }

            var owner = await _userService.GetUserByIdAsync(ownerId);
            if (owner == null)
            {
                _log.LogError("Failed to add reservation: owner not found, meetingRoomId={}", meetingRoomId);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Owner not found");
            }

            if (await IsRoomOccupiedInTimeslotAsync(room, time))
            {
                _log.LogError("Failed to add reservation: Room is already occupied");
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "Meeting room is occupied in provided timeslot");
            }

            var reservation = new Reservation
            {
                MeetingName = meetingName,
                MeetingRoom = room,
                MeetingRoomId = room.Id,
                ReservationOwner = owner,
                ReservationOwnerId = owner.Id,
                StartTime = time.StartTime,
                EndTime = time.EndTime,
                Participants = []
            };
            var reservationEntity = await _dbContext.AddAsync(reservation);

            if (participantIds != null)
            {
                var participants = await _userService.GetUsersByIdsAsync(participantIds);
                if (participants.Count != participantIds.Count)
                {
                    _log.LogError("Could not find one more participants, expected {}, found {}, found users={}",
                        participantIds.Count, participants.Count, participants);
                    throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Could not find one more provided participants");
                }
                foreach (var participant in participants)
                {
                    var meetingParticipant = new MeetingParticipant(reservation, participant);
                    reservation.Participants.Add(meetingParticipant);

                    _dbContext.Add(meetingParticipant);
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding reservation, reservation={}, e={}", reservation, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving reservation");
            }
            return reservationEntity.Entity;
        }

        public Task<List<MeetingRoom>> GetAllMeetingRoomsAsync()
        {
            return _dbContext.MeetingRooms.ToListAsync();
        }

        public async Task<MeetingRoom> AddMeetingRoomAsync(string name)
        {
            if (await IsRoomNameInUseAsync(name.ToLower()))
            {
                _log.LogError("Meeting room with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "Room with provided name already exists");
            }

            var meetingRoom = new MeetingRoom { RoomName = name };
            var meetingRoomEntity = await _dbContext.AddAsync(meetingRoom);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding meeting room, meetingRoom={}, e={}", meetingRoom, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving meeting room");
            }
            return meetingRoomEntity.Entity;
        }

        private Task<MeetingRoom?> GetMeetingRoomByIdAsync(int id)
        {
            return (from meetingRooms in _dbContext.MeetingRooms
                    where meetingRooms.Id == id
                    select meetingRooms).SingleOrDefaultAsync();
        }

        private Task<bool> IsRoomOccupiedInTimeslotAsync(MeetingRoom room, TimeRange time)
        {
            return _dbContext.Reservations
                .Where(r => r.MeetingRoomId == room.Id)
                .Where(r => !time.DoesOverlapWith(r.StartTime, r.EndTime))
                .AnyAsync();
        }

        private Task<bool> IsRoomNameInUseAsync(string name)
        {
            var nameLowercase = name.ToLower();
            return (from room in _dbContext.MeetingRooms
                    where !String.IsNullOrEmpty(room.RoomName) && room.RoomName.Equals(nameLowercase)
                    select room).AnyAsync();
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
