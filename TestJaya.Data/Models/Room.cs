using System;
using System.Collections.Generic;

namespace TestJaya.Data.Models
{
    public class Room : AbstractEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IList<UserRoom> UserRooms { get; set; }

        public Room()
        {
        }
    }
}
