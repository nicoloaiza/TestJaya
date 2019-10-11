using System;
namespace TestJaya.Data.DTO
{
    /// <summary>
    /// Common interface for DTOs
    /// </summary>
    /// <typeparam name="TId">Type of ID of the DTO.</typeparam>
    public interface IDto<TId>
    {
        /// <summary>
        /// Gets or sets the ID of this DTO.
        /// </summary>
        TId Id { get; set; }
    }
}
