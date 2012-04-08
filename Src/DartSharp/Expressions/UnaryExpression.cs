namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class UnaryExpression : IExpression
    {
        private IExpression expression;

        public UnaryExpression(IExpression expression)
        {
            this.expression = expression;
        }

        public IExpression Expression { get { return this.expression; } }

        public abstract object Apply(object value);

        public virtual object Evaluate(Context context)
        {
            return this.Apply(this.expression.Evaluate(context));
        }
    }
}
