using System;
using System.Diagnostics.CodeAnalysis;

namespace TestJaya.Data.DTO
{
    public class PagingInfo
    {
        /// <summary>
        /// Requested max number of entities to return.
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Requested number of entities to bypass from the collection.
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Number of entities returned.
        /// </summary>
        public int Returned { get; set; }
        /// <summary>
        /// Total number of entities in the collection.
        /// </summary>
        public long Total { get; set; }

        public PagingInfo()
        {

        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"[Offset: {Offset}; Limit: {Limit}; Returned: {Returned}; Total: {Total}]";
        }
    }
}
