using System;
using System.Diagnostics.CodeAnalysis;

namespace TestJaya.Data.Filters
{
    /// <summary>
    /// Utility DTO with information to search and filter API data queries.
    /// </summary>
    public class PagedSearchAndFilterInfo
    {
        /// <summary>
        /// Text to search inside all the fields of the entities being searched.
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// Maximum number of entities to return.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Number of entities to skip from the results.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Name of the field of the entity to sort the results.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Indicates wether the order of sorting should be ascendant or descendant.
        /// </summary>
        public bool OrderAsc { get; set; }

        /// <summary>
        /// Filters to be applied to the search.
        /// </summary>
        public FilterField[] Filters { get; set; }

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{nameof(SearchString)} = {SearchString}; {nameof(Limit)} = {Limit}; {nameof(Offset)} = {Offset}; {nameof(OrderBy)} = {OrderBy}; {nameof(OrderAsc)} = {OrderAsc}; ";
    }
}
