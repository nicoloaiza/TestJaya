using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Linq;

namespace TestJaya.Data.DTO
{
    /// <summary>
    /// Wrapper for collections of paginated DTO results.
    /// </summary>
    /// <typeparam name="TDto">Type of DTO to be wrapped.</typeparam>
    public class PagedResult<TDto>
    {
        /// <summary>
        /// Collection of instances included in this result.
        /// </summary>
        public virtual IEnumerable<TDto> Data { get; private set; }

        /// <summary>
        /// Includes information about paging for these results.
        /// </summary>
        public virtual PagingInfo Paging { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this result contains data.
        /// </summary>
        [JsonIgnore]
        public virtual bool HasData { get { return Data != null && Data.Any(); } }

        public PagedResult()
        {

        }

        public PagedResult(IEnumerable<TDto> items, int offset, int limit, long totalRecordCount)
        {
            Data = items;
            Paging = new PagingInfo
            {
                Limit = limit,
                Offset = offset,
                Returned = items != null ? items.Count() : 0,
                Total = totalRecordCount
            };
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{typeof(TDto).Name}: [{Paging}]";
    }
}
