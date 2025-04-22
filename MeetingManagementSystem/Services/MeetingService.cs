using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services
{
    public class MeetingService(ILogger<MeetingService> log, MeetingDbContext dbContext, IUserService userService) : IMeetingService
    {
        private readonly ILogger<MeetingService> _log = log;
        private readonly MeetingDbContext _dbContext = dbContext;
        private readonly IUserService _userService = userService;

        public IEnumerable<Reservation> GetAllReservations(bool includeExpired = false)
        {
            return from reservation in _dbContext.Reservations
                   where FilterRemoveExpiredReservations(reservation, includeExpired)
                   orderby reservation.StartTime
                   select reservation;
        }

        public IEnumerable<Reservation> GetReservationsByOwner(int ownerId, bool includeExpired = false)
        {
            return from reservation in _dbContext.Reservations
                   where ownerId == reservation.ReservationOwnerId && FilterRemoveExpiredReservations(reservation, includeExpired)
                   orderby reservation.StartTime
                   select reservation;
        }

        public IEnumerable<Reservation> GetReservationsForParticipant(int participantId, bool includeExpired = false) {
            return from user in _dbContext.Users
                   join participant in _dbContext.MeetingParticipants
                        on user.Id equals participant.ParticipantId
                   join reservation in _dbContext.Reservations
                        on participant.ReservationId equals reservation.Id
                   where user.Id == participantId && FilterRemoveExpiredReservations(reservation, includeExpired)
                   select reservation;
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
            var reservationEntry = _dbContext.Add(reservation);

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
            return reservationEntry.Entity;
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
        /// If includeExpired is set to false, will filter reservations that have expired w.r.t. the current system time.
        /// </summary>
        /// <returns>Always true if includeExpired is set to true. Otherwise, returns true if the provided reservation has not expired.</returns>
        private static bool FilterRemoveExpiredReservations(Reservation reservation, bool includeExpired)
        {
            return includeExpired ? true : reservation.EndTime < DateTimeOffset.Now;
        }
    }
}
