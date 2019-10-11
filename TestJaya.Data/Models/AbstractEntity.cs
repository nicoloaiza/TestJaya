using System;
using System.ComponentModel.DataAnnotations;

namespace TestJaya.Data.Models
{
    public abstract class AbstractEntity<TId> : IEntity<TId>
    {
        [Key]
        public virtual TId Id { get; set; }
    }
}
