using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestJaya.Data.Models
{
    public class UserRoom : AbstractEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        public Guid UserID { get; set; }
        public virtual User User { get; set; }
        public Guid RoomID { get; set; }
        public virtual Room Room { get; set; }
        public DateTime JoinedAt { get; set; }

        public UserRoom()
        {
        }
    }
}
