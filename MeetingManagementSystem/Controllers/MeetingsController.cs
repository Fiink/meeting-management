using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Data.Models;
using MeetingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController(ILogger<MeetingsController> log, IMeetingService meetingService) : AbstractController
    {
        private readonly ILogger<MeetingsController> _log = log;
        private readonly IMeetingService _meetingService = meetingService;

        [HttpGet]
        public ActionResult<IEnumerable<ReservationDTO>> GetAllReservations([FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations, includeExpired={}", includeExpired);
            var reservations = _meetingService.GetAllReservations(includeExpired);
            var dtos = MapToDTO(reservations);
            return Ok(dtos);
        }

        [HttpGet("owner/{id}")]
        public ActionResult<IEnumerable<ReservationDTO>> GetAllReservationsForOwner([FromRoute] int id, [FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations for owner, id={}, includeExpired={}", id, includeExpired);
            try
            {
                var reservations = _meetingService.GetReservationsByOwner(id, includeExpired);
                var dtos = MapToDTO(reservations);
                return Ok(dtos);
            } catch (Exception e)
            {
                _log.LogError("Exception occurred while getting all reservations for owner, id={}, includeExpired={}, e={}", id, includeExpired, e);
                return MapError(e);
            }
        }

        [HttpGet("participant/{id}")]
        public ActionResult<IEnumerable<ReservationDTO>> GetAllReservationsForParticipant([FromRoute] int id, [FromQuery] bool includeExpired = false)
        {
            _log.LogTrace("Received request for all reservations for participant, id={}, includeExpired={}", id, includeExpired);
            try
            {
                var reservations = _meetingService.GetReservationsForParticipant(id, includeExpired);
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
        public ActionResult<ReservationDTO> AddReservation([FromBody] AddReservationDTO addReservationDTO)
        {
            _log.LogTrace("Received request to add reservation, addReservationDTO={}", addReservationDTO);
            try
            {
                Reservation reservation = _meetingService.AddReservation(
                    addReservationDTO.OwnerId,
                    addReservationDTO.MeetingRoomId,
                    addReservationDTO.MeetingName,
                    addReservationDTO.StartTime,
                    addReservationDTO.EndTime,
                    addReservationDTO.ParticipantIds
                );
                return new ReservationDTO(reservation);
            } catch (Exception e)
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
