using System;
namespace TestJaya.Data.DTO
{
    public class UserRoomDto : IDto<int>
    {
        public int Id { get; set; }
        public Guid UserID { get; set; }
        //public UserDto User { get; set; }
        public Guid RoomID { get; set; }
        //public RoomDto Room { get; set; }
        public DateTime JoinedAt { get; set; }
        public UserRoomDto()
        {
        }
    }
}
