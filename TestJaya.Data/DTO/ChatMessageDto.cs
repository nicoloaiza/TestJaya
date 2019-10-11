using System;
namespace TestJaya.Data.DTO
{
    public class ChatMessageDto : IDto<int>
    {
        public int Id { get; set; }
        public Guid UserID { get; set; }
        public Guid RoomID { get; set; }
        public RoomDto Room { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChatMessageDto()
        {
        }
    }
}
