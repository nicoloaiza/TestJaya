using System;
using System.Diagnostics.CodeAnalysis;

namespace TestJaya.Data.Filters
{
    public class FilterData
    {
        public string Predicate { get; private set; }
        public object[] Values { get; private set; }

        [ExcludeFromCodeCoverage]
        public FilterData(string predicate, object[] values)
        {
            Predicate = predicate;
            Values = values;
        }
    }
}
