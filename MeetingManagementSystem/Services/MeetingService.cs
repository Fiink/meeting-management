using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MeetingManagementSystem.Services
{
    public class MeetingService(ILogger<MeetingService> log, MeetingDbContext dbContext, IUserService userService) : IMeetingService
    {
        private readonly ILogger<MeetingService> _log = log;
        private readonly MeetingDbContext _dbContext = dbContext;
        private readonly IUserService _userService = userService;

        public List<Reservation> GetAllReservations(bool includeExpired = false)
        {
            IQueryable<Reservation> query = _dbContext.Reservations;
            if (!includeExpired)
            {
                query = FilterRemoveExpiredReservations(query);
            }
            return query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .OrderBy(r => r.StartTime).ToList();
        }

        public List<Reservation> GetReservationsByOwner(int ownerId, bool includeExpired = false)
        {
            IQueryable<Reservation> query = _dbContext.Reservations
                .Where(r => ownerId == r.ReservationOwnerId);
            if (!includeExpired)
            {
                query = FilterRemoveExpiredReservations(query);
            }
            return query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .OrderBy(r => r.StartTime).ToList();
        }

        public List<Reservation> GetReservationsForParticipant(int participantId, bool includeExpired = false) {
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
            return query
                .Include(r => r.MeetingRoom)
                .Include(r => r.Participants)
                .ThenInclude(p => p.Participant)
                .Include(r => r.ReservationOwner)
                .ToList();
        }

        public Reservation AddReservation(int ownerId, int meetingRoomId, string? meetingName, DateTimeOffset startTime,
            DateTimeOffset endTime, ICollection<int>? participantIds)
        {
            if (startTime > endTime)
            {
                _log.LogError("Invalid timeslots, reservation start time is after end time, start={}, end={}", startTime, endTime);
                throw new ResultException(ResultException.ExceptionType.UNPROCESSABLE_ENTITY, "Invalid start- and end-time");
            }

            var room = GetMeetingRoomById(meetingRoomId);
            if (room == null)
            {
                _log.LogError("Failed to add reservation: meeting room not found, meetingRoomId={}", meetingRoomId);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Meeting room not found");
            }

            var owner = _userService.GetUserById(ownerId);
            if (owner == null)
            {
                _log.LogError("Failed to add reservation: owner not found, meetingRoomId={}", meetingRoomId);
                throw new ResultException(ResultException.ExceptionType.NOT_FOUND, "Owner not found");
            }

            if (IsRoomOccupiedInTimeslot(room, startTime, endTime))
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
                StartTime = startTime,
                EndTime = endTime,
                Participants = new List<MeetingParticipant>()
            };
            var reservationEntity = _dbContext.Add(reservation);

            if (participantIds != null)
            {
                var participants = _userService.GetUsersByIds(participantIds);
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
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding reservation, reservation={}, e={}", reservation, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving reservation");
            }
            return reservationEntity.Entity;
        }

        public List<MeetingRoom> GetAllMeetingRooms()
        {
            return [.. _dbContext.MeetingRooms];
        }
        public MeetingRoom AddMeetingRoom(string name)
        {
            // Check if room name is already used
            var nameLowercase = name.ToLower();
            if ((from room in _dbContext.MeetingRooms
                    where !String.IsNullOrEmpty(room.RoomName) && room.RoomName.Equals(nameLowercase)
                    select room).Any())
            {
                _log.LogError("Meeting room with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "Room with provided name already exists");
            }

            var meetingRoom = new MeetingRoom
            {
                RoomName = name
            };
            var meetingRoomEntity = _dbContext.Add(meetingRoom);

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding meeting room, meetingRoom={}, e={}", meetingRoom, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving meeting room");
            }
            return meetingRoomEntity.Entity;
        }

        private MeetingRoom? GetMeetingRoomById(int id)
        {
            return (from meetingRooms in _dbContext.MeetingRooms
                    where meetingRooms.Id == id
                    select meetingRooms).SingleOrDefault();
        }

        private bool IsRoomOccupiedInTimeslot(MeetingRoom room, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return (from reservations in _dbContext.Reservations
                    where reservations.StartTime < endTime && startTime < reservations.EndTime
                    select reservations).Any();
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
