using System;
namespace TestJaya.Data.Models
{
    public class ChatMessage : AbstractEntity<int>
    { 
        public Guid UserID { get; set; }
        public Guid RoomID { get; set; }
        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public ChatMessage()
        {
        }
    }
}
