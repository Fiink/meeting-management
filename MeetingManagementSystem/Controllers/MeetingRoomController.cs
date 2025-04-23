using MeetingManagementSystem.Contracts;
using MeetingManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingManagementSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MeetingRoomController(ILogger<MeetingRoomController> log, IMeetingServiceAsync meetingService) : AbstractController
    {
        private readonly ILogger<MeetingRoomController> _log = log;
        private readonly IMeetingServiceAsync _meetingService = meetingService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingRoomDTO>>> GetAllMeetingRooms()
        {
            _log.LogTrace("Received request for all meeting rooms");
            var rooms = await _meetingService.GetAllMeetingRoomsAsync();
            var dtos = rooms.Select(r => new MeetingRoomDTO(r));
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<MeetingRoomDTO>> AddMeetingRoom([FromBody] string roomName)
        {
            _log.LogTrace("Received request for all meeting rooms");
            try
            {
                var room = await _meetingService.AddMeetingRoomAsync(roomName);
                var dto = new MeetingRoomDTO(room);
                return Ok(dto);
            }
            catch (Exception e)
            {
                _log.LogError("Exception occurred while adding meeting room, roomName={}, e={}", roomName, e);
                return MapError(e);
            }
        }
    }
}
