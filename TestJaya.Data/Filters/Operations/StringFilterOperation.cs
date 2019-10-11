using System;
namespace TestJaya.Data.Filters.Operations
{

    public class StringFilterOperation : FilterOperation
    {
        public StringFilterOperation(FilterField filterField) : base(filterField)
        {
            filterField.Value = filterField.Value == null ?
                                    string.Empty :
                                    filterField.Value.ToString().ToUpper();
        }

        public override string GenerateExpression<TEntity>(int index)
        {
            return $"({ExpressionForCheckingNullObjects()} AND {filterField.FieldName}.ToUpper().Contains(@{index}))";
        }
    }
}
