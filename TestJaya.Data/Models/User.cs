using System;
using System.Collections.Generic;

namespace TestJaya.Data.Models
{
    public class User : AbstractEntity<Guid>
    {
        public string Name { get; set; }
        public virtual IList<UserRoom> UserRooms { get; set; }

        public User()
        {
        }
    }
}
