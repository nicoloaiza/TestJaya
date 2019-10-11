using System;
using System.Collections.Generic;
using System.Text;
using TestJaya.Data.Filters.Operations;

namespace TestJaya.Data.Filters
{
    public static class FiltersExtensions
    {
        public static FilterData ToWhereClause<TEntity>(this PagedSearchAndFilterInfo pagedSearchAndFilterInfo)
        {
            var predicate = new StringBuilder();
            var listOfExpressions = new List<Object>();
            if (pagedSearchAndFilterInfo != null && pagedSearchAndFilterInfo.Filters != null)
            {
                var index = 0;
                foreach (var filterField in pagedSearchAndFilterInfo.Filters)
                {
                    if (index > 0) predicate.Append(" AND ");

                    var propertyType = getPropertyType<TEntity>(filterField);
                    var filterOperation = FilterOperationFactory.Create(filterField, propertyType);

                    predicate.Append(filterOperation.GenerateExpression<TEntity>(index));
                    listOfExpressions.Add(filterField.Value);
                    index++;
                }
            }
            return new FilterData(predicate.ToString(), listOfExpressions.ToArray());
        }

        private static Type getPropertyType<TClass>(FilterField filterField) => getPropertyType(filterField.FieldName, typeof(TClass), 0, filterField.FieldName.Split('.').Length);

        private static Type getPropertyType(string fieldName, Type classType, int index, int maximumSize)
        {
            var partialFieldName = maximumSize > 1 ? fieldName.Split('.')[index] : fieldName;
            var propertyInfo = classType.GetProperty(partialFieldName);

            if (propertyInfo == null) throw new Exception($"There isn't a property named: {partialFieldName} inside the class {classType}.");

            index++;

            return (maximumSize > index ?
                    getPropertyType(fieldName, propertyInfo.PropertyType, index, maximumSize) :
                    propertyInfo.PropertyType);
        }
    }
}
