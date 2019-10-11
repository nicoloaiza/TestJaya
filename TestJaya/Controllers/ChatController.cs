using System;
using Microsoft.AspNetCore.Mvc;
using TestJaya.Business.Services;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using System.Linq;
using TestJaya.Business.Util;
using TestJaya.Models;

namespace TestJaya.Controllers
{
    public class ChatController : Controller
    {
        private readonly IStoredDataService<ChatMessage, ChatMessageDto, int> _chatService;
        private readonly IStoredDataService<Room, RoomDto, Guid> _roomService;
        private readonly IStoredDataService<UserRoom, UserRoomDto, int> _userRoomService;
        private readonly IStoredDataService<User, UserDto, Guid> _userService;

        public ChatController(IStoredDataService<ChatMessage, ChatMessageDto, int> chatService,
            IStoredDataService<Room, RoomDto, Guid> roomService,
            IStoredDataService<UserRoom, UserRoomDto, int> userRoomService,
            IStoredDataService<User, UserDto, Guid> userService)
        {
            _chatService = chatService;
            _roomService = roomService;
            _userRoomService = userRoomService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index(Guid Id)
        {
            byte[] userArray;
            bool userAvailable = HttpContext.Session.TryGetValue("User", out userArray);
            UserDto user = null;
            if (userAvailable)
            {
                user = (UserDto)userArray.ToObject();
            }
            RoomDto room = _roomService.RetrieveAll().Data.ToList().Where(x => x.Id == Id).FirstOrDefault();
            if (room != null)
            {
                PagedResult<UserRoomDto> userRooms = _userRoomService.RetrieveAll( x => x.UserID == user.Id && x.RoomID == room.Id);
                UserRoomDto userRoom = userRooms != null && userRooms.Paging.Total > 0 ? userRooms.Data.ElementAt(0) : null;
                if (userRooms == null || userRooms.Paging.Total < 1)
                {
                    userRoom = new UserRoomDto()
                    {
                        UserID = user.Id,
                        RoomID = room.Id,
                        JoinedAt = DateTime.Now,
                    };
                    _userRoomService.Create(userRoom);
                }
                PagedResult<ChatMessageDto> roomChats = _chatService.RetrieveAll(x => x.RoomID == room.Id && x.CreatedAt >= userRoom.JoinedAt);
                if(roomChats != null && roomChats.Paging.Total > 0)
                {
                    foreach(ChatMessageDto mess in roomChats.Data)
                    {
                        mess.User = _userService.Retrieve(mess.UserID);
                    }
                }
                HttpContext.Session.Set("Room", room.ToByteArray());
                ChatViewModel vm = new ChatViewModel()
                {
                    UserID = userRoom.UserID,
                    RoomID = room.Id,
                    Chats = roomChats.Data.ToList()
                };
                return View("Index", vm);
            }
            else
            {
                return Redirect("/Rooms/Index");
            }
        }
    }
}
