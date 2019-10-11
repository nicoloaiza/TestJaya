using System;
namespace TestJaya.Data.Filters.Operations
{
    public static class FilterOperationFactory
    {
        /// <summary>
        /// Creates a <see cref="FilterOperation"/> that depends on the type of the filter.
        /// </summary>
        /// <param name="filterField">Field to be filtered.</param>
        /// <param name="propertyType">Type of the field.</param>
        /// <returns></returns>
        public static FilterOperation Create(FilterField filterField, Type propertyType)
        {
            var propertyTypeName = propertyType.IsGenericType ? propertyType.GenericTypeArguments[0].Name : propertyType.Name;

            switch (propertyTypeName)
            {
                case "String":
                    return new StringFilterOperation(filterField);
                case "Int32":
                case "Int64":
                case "DateTime":
                case "Boolean":
                    return new FilterOperation(filterField);
            }

            throw new NotImplementedException();
        }
    }
}
