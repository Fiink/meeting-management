using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Exceptions;
using MeetingManagementSystem.Models;
using MeetingManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Services.Implementations
{
    public class MeetingService(ILogger<MeetingService> log, IReservationRepository reservationRepository, IMeetingRoomRepository meetingRoomRepository, IUserServiceAsync userService) : IMeetingServiceAsync
    {
        private readonly ILogger<MeetingService> _log = log;
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IMeetingRoomRepository _meetingRoomRepository = meetingRoomRepository;
        private readonly IUserServiceAsync _userService = userService;

        public async Task<List<Reservation>> GetAllReservationsAsync(bool includeExpired = false)
        {
            return await _reservationRepository.GetAllAsync(includeExpired);
        }

        public async Task<List<Reservation>> GetReservationsByOwnerAsync(int ownerId, bool includeExpired = false)
        {
            return await _reservationRepository.GetReservationsByOwnerAsync(ownerId, includeExpired);
        }

        public async Task<List<Reservation>> GetReservationsForParticipantAsync(int participantId, bool includeExpired = false)
        {
            return await _reservationRepository.GetReservationsForParticipantAsync(participantId, includeExpired);
        }

        public async Task<Reservation> AddReservationAsync(int ownerId, int meetingRoomId, string? meetingName, TimeRange time, ICollection<int>? participantIds)
        {
            var room = await _meetingRoomRepository.GetMeetingRoomByIdAsync(meetingRoomId);
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

            if (await _reservationRepository.IsRoomOccupiedInTimeslotAsync(room, time))
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
                }
            }

            try
            {
                return await _reservationRepository.AddReservationAsync(reservation);
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding reservation, reservation={}, e={}", reservation, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving reservation");
            }
        }

        public Task<List<MeetingRoom>> GetAllMeetingRoomsAsync()
        {
            return _meetingRoomRepository.GetAllAsync();
        }

        public async Task<MeetingRoom> AddMeetingRoomAsync(string name)
        {
            if (await _meetingRoomRepository.IsRoomNameInUseAsync(name.ToLower()))
            {
                _log.LogError("Meeting room with name already exists, name={}", name);
                throw new ResultException(ResultException.ExceptionType.CONFLICT, "Room with provided name already exists");
            }

            try
            {
                return await _meetingRoomRepository.AddMeetingRoom(name);
            }
            catch (DbUpdateException e)
            {
                _log.LogError("Error occurred while adding meeting room, name={}, e={}", name, e);
                throw new ResultException(ResultException.ExceptionType.PERSISTENCE_ERROR, "Error saving meeting room");
            }
        }
    }
}
