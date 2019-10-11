using System;
using System.Collections.Generic;

namespace TestJaya.Data.DTO
{
    [Serializable]
    public class UserDto : IDto<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<UserRoomDto> UserRooms { get; set; }
    }
}
