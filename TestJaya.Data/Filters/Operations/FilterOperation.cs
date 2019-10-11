using System;
using System.Text;

namespace TestJaya.Data.Filters.Operations
{
    public class FilterOperation
    {
        protected readonly FilterField filterField;

        public FilterOperation(FilterField filterField) => this.filterField = filterField;

        public virtual string GenerateExpression<TEntity>(int index)
        {
            var operatorText = MatchOperator(filterField.Operator);
            return $"({ExpressionForCheckingNullObjects()} AND {filterField.FieldName} {operatorText} @{index})";
        }

        protected string MatchOperator(FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.LessThan:
                    return "<";
                case FilterOperator.Equal:
                    return "==";
                case FilterOperator.GreaterThan:
                    return ">";
                case FilterOperator.Contains:
                    return "";
            }
            throw new NotImplementedException();
        }

        protected string ExpressionForCheckingNullObjects()
        {
            var text = filterField.FieldName;
            var outputText = new StringBuilder();
            var expressionText = string.Empty;
            var index = 0;
            foreach (var fieldName in text.Split('.'))
            {
                expressionText += ((index > 0) ? "." : "") + fieldName;
                if (index > 0)
                {
                    outputText.Append(" AND ");
                }
                outputText.Append($"{expressionText} != null ");

                index++;
            }
            return outputText.ToString();
        }
    }
}
