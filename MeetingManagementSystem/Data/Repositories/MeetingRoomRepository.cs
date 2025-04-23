using MeetingManagementSystem.Data.Db;
using MeetingManagementSystem.Data.Interfaces;
using MeetingManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Data.Repositories
{
    public class MeetingRoomRepository(MeetingDbContext dbContext) : IMeetingRoomRepository
    {
        private readonly MeetingDbContext _dbContext = dbContext;

        public async Task<List<MeetingRoom>> GetAllAsync()
        {
            return await _dbContext.MeetingRooms.ToListAsync();
        }

        public async Task<MeetingRoom?> GetMeetingRoomByIdAsync(int id)
        {
            return await (from meetingRooms in _dbContext.MeetingRooms
                    where meetingRooms.Id == id
                    select meetingRooms).SingleOrDefaultAsync();
        }

        public async Task<bool> IsRoomNameInUseAsync(string name)
        {
            var nameLowercase = name.ToLower();
            return await (from room in _dbContext.MeetingRooms
                    where !String.IsNullOrEmpty(room.RoomName) && room.RoomName.ToLower().Equals(nameLowercase)
                    select room).AnyAsync();
        }

        public async Task<MeetingRoom> AddMeetingRoom(string name)
        {
            var meetingRoom = new MeetingRoom { RoomName = name };
            var meetingRoomEntity = await _dbContext.AddAsync(meetingRoom);
            await _dbContext.SaveChangesAsync();
            return meetingRoomEntity.Entity;
        }
    }
}
