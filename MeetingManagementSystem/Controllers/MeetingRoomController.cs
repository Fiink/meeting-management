using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MeetingRoomController(ILogger<MeetingRoomController> log, IMeetingService meetingService) : AbstractController
    {
        private readonly ILogger<MeetingRoomController> _log = log;
        private readonly IMeetingService _meetingService = meetingService;

        [HttpGet]
        public ActionResult<IEnumerable<MeetingRoomDTO>> GetAllMeetingRooms()
        {
            _log.LogTrace("Received request for all meeting rooms");
            var rooms = _meetingService.GetAllMeetingRooms();
            var dtos = rooms.Select(r => new MeetingRoomDTO(r));
            return Ok(dtos);
        }

        [HttpPost]
        public ActionResult<MeetingRoomDTO> AddMeetingRoom([FromBody] string roomName)
        {
            _log.LogTrace("Received request for all meeting rooms");
            try
            {
                var room = _meetingService.AddMeetingRoom(roomName);
                var dto = new MeetingRoomDTO(room);
                return Ok(dto);
            } catch (Exception e)
            {
                _log.LogError("Exception occurred while adding meeting room, roomName={}, e={}", roomName, e);
                return MapError(e);
            }
        }
    }
}
