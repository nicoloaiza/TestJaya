using System;
namespace TestJaya.Data.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}
