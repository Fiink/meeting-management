using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Models;
using MeetingManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController(ILogger<MeetingsController> log, IMeetingServiceAsync meetingService) : AbstractController
    {
        private readonly ILogger<MeetingsController> _log = log;
        private readonly IMeetingServiceAsync _meetingService = meetingService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations([FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations, includeExpired={}", includeExpired);
            var reservations = await _meetingService.GetAllReservationsAsync(includeExpired);
            var dtos = MapToDTO(reservations);
            return Ok(dtos);
        }

        [HttpGet("owner/{id}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservationsForOwner([FromRoute] int id, [FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations for owner, id={}, includeExpired={}", id, includeExpired);
            try
            {
                var reservations = await _meetingService.GetReservationsByOwnerAsync(id, includeExpired);
                var dtos = MapToDTO(reservations);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while getting all reservations for owner, id={}, includeExpired={}, e={}", id, includeExpired, e);
                return MapError(e);
            }
        }

        [HttpGet("participant/{id}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservationsForParticipant([FromRoute] int id, [FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations for participant, id={}, includeExpired={}", id, includeExpired);
            try
            {
                var reservations = await _meetingService.GetReservationsForParticipantAsync(id, includeExpired);
                var dtos = MapToDTO(reservations);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while getting all reservations for participant, id={}, includeExpired={}, e={}", id, includeExpired, e);
                return MapError(e);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> AddReservation([FromBody] AddReservationDTO addReservationDTO)
        {
            _log.LogTrace("Received request to add reservation, addReservationDTO={}", addReservationDTO);

            TimeRange time;
            try
            {
                time = new TimeRange(addReservationDTO.StartTime, addReservationDTO.EndTime);
            }
            catch (ArgumentException)
            {
                _log.LogError("Invalid time provided in request, addReservationDTO={}", addReservationDTO);
                return UnprocessableEntity();
            }

            try
            {
                Reservation reservation = await _meetingService.AddReservationAsync(
                    addReservationDTO.OwnerId,
                    addReservationDTO.MeetingRoomId,
                    addReservationDTO.MeetingName,
                    time,
                    addReservationDTO.ParticipantIds
                );
                return new ReservationDTO(reservation);
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while adding new reservation, addReservationDTO={}, e={}", addReservationDTO, e);
                return MapError(e);
            }
        }

        private static IEnumerable<ReservationDTO> MapToDTO(IEnumerable<Reservation> reservations)
        {
            return reservations.Select(r => new ReservationDTO(r));
        }
    }
}
