using System;
namespace TestJaya.Data.Filters
{
    public class FilterField
    {
        public string FieldName { get; set; }
        public FilterOperator Operator { get; set; }
        public object Value { get; set; }
    }
}
