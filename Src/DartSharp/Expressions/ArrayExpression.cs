namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;

    public class ArrayExpression : IExpression
    {
        private IEnumerable<IExpression> expressions;

        public ArrayExpression(IEnumerable<IExpression> expressions)
        {
            this.expressions = expressions;
        }

        public IEnumerable<IExpression> Expressions { get { return this.expressions; } }

        public object Evaluate(Context context)
        {
            IList<object> values = new List<object>();

            if (expressions != null)
                foreach (var argument in expressions)
                    values.Add(argument.Evaluate(context));

            return values;
        }
    }
}

