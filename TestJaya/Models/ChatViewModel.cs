using System;
using System.Collections.Generic;
using TestJaya.Data.DTO;

namespace TestJaya.Models
{
    public class ChatViewModel
    {
        public Guid UserID { get; set; }
        public Guid RoomID { get; set; }
        public List<ChatMessageDto> Chats { get; set; }
        public ChatViewModel()
        {
        }
    }
}
