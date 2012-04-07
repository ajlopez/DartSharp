namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DartSharp.Language;

    using Microsoft.VisualBasic.CompilerServices;

    [Serializable]
    public class CompareExpression : BinaryExpression
    {
        private Func<object, object, bool, object> function;
        private ComparisonOperator operation;

        public CompareExpression(ComparisonOperator operation, IExpression left, IExpression right)
            : base(left, right)
        {
            this.operation = operation;

            switch (operation)
            {
                case ComparisonOperator.Equal:
                    this.function = Operators.CompareObjectEqual;
                    break;
                case ComparisonOperator.NotEqual:
                    this.function = Operators.CompareObjectNotEqual;
                    break;
                case ComparisonOperator.Less:
                    this.function = Operators.CompareObjectLess;
                    break;
                case ComparisonOperator.LessEqual:
                    this.function = Operators.CompareObjectLessEqual;
                    break;
                case ComparisonOperator.Greater:
                    this.function = Operators.CompareObjectGreater;
                    break;
                case ComparisonOperator.GreaterEqual:
                    this.function = Operators.CompareObjectGreaterEqual;
                    break;
            }
        }

        public ComparisonOperator Operation { get { return this.operation; } }

        public override object Apply(object leftValue, object rightValue)
        {
            return this.function(leftValue, rightValue, false);
        }
    }
}
