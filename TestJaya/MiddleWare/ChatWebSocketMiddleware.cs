using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TestJaya.Business.Services;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using TestJaya.Business.Util;

namespace TestJaya.MiddleWare
{

    public class WebSocketConfig
    {
        public string Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
    }

    public class ChatWebSocketMiddleware
    {

        private static ConcurrentDictionary<WebSocketConfig, WebSocket> _sockets = new ConcurrentDictionary<WebSocketConfig, WebSocket>();
        private readonly IStoredDataService<ChatMessage, ChatMessageDto, int> _chatService;
        private readonly IStoredDataService<Room, RoomDto, Guid> _roomService;
        private readonly IStoredDataService<UserRoom, UserRoomDto, int> _userRoomService;
        private readonly IStoredDataService<User, UserDto, Guid> _userService;

        private readonly RequestDelegate _next;

        public ChatWebSocketMiddleware(RequestDelegate next,
            IStoredDataService<ChatMessage, ChatMessageDto, int> chatService,
            IStoredDataService<Room, RoomDto, Guid> roomService,
            IStoredDataService<UserRoom, UserRoomDto, int> userRoomService,
            IStoredDataService<User, UserDto, Guid> userService)
        {
            _next = next;
            _chatService = chatService;
            _roomService = roomService;
            _userRoomService = userRoomService;
            _userService = userService;
        }

        private UserDto GetUser(Guid userId)
        {
            try
            {
                return this._userService.Retrieve(userId);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private void InsertMessage(ChatMessageDto message)
        {
            try
            {
                _chatService.Create(message);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private UserDto GetUserFromSesion(HttpContext context)
        {
            byte[] array;
            bool objectAvailable = context.Session.TryGetValue("User", out array);
            if (objectAvailable)
            {
                return (UserDto)array.ToObject();
            }
            return null;
        }

        private RoomDto GetRoomFromSesion(HttpContext context)
        {
            byte[] array;
            bool objectAvailable = context.Session.TryGetValue("Room", out array);
            if (objectAvailable)
            {
                return (RoomDto)array.ToObject();
            }
            return null;
        }



        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();
            UserDto userFromSession = GetUserFromSesion(context);
            RoomDto roomFromSession = GetRoomFromSesion(context);


            WebSocketConfig config = new WebSocketConfig() { Id = socketId, RoomId = roomFromSession.Id, UserId = userFromSession.Id };

            _sockets.TryAdd(config, currentSocket);

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var response = await ReceiveStringAsync(currentSocket, ct);

                ChatMessageDto message = JsonConvert.DeserializeObject<ChatMessageDto>(response);
                UserDto user = GetUser(message.UserID);
                message.CreatedAt = DateTime.Now;
                InsertMessage(message);
                if (message.User == null)
                    message.User = new UserDto();
                message.User.Name = user != null ? user.Name : "---";

                if(config.UserId == Guid.Empty || config.RoomId == Guid.Empty)
                {
                    config.UserId = message.UserID;
                    config.RoomId = message.RoomID;
                }

                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }

                    continue;
                }

                foreach (var socket in _sockets)
                {
                    if (socket.Value.State != WebSocketState.Open)
                    {
                        continue;
                    }
                    if(socket.Key.RoomId == config.RoomId)
                    {
                        string messageToSend = JsonConvert.SerializeObject(message, Formatting.Indented);
                        await SendStringAsync(socket.Value, messageToSend, ct);
                    }
                }
            }

            WebSocket dummy;
            _sockets.TryRemove(config, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }

        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
