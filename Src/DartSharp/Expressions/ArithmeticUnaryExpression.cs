namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DartSharp.Language;

    using Microsoft.VisualBasic.CompilerServices;

    public class ArithmeticUnaryExpression : UnaryExpression
    {
        private Func<object, object> function;
        private ArithmeticOperator operation;

        public ArithmeticUnaryExpression(ArithmeticOperator operation, IExpression expression)
            : base(expression)
        {
            this.operation = operation;

            switch (operation)
            {
                case ArithmeticOperator.Minus:
                    this.function = Operators.NegateObject;
                    break;
                case ArithmeticOperator.Plus:
                    this.function = Operators.PlusObject;
                    break;
                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        public ArithmeticOperator Operation { get { return this.operation; } }

        public override object Apply(object value)
        {
            if (value == null)
                return value;

            return this.function(value);
        }
    }
}
