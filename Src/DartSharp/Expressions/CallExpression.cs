namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;

    public class CallExpression : IExpression
    {
        private IExpression expression;
        private IEnumerable<IExpression> arguments;

        public CallExpression(IExpression expression, IEnumerable<IExpression> arguments)
        {
            this.expression = expression;
            this.arguments = arguments;
        }

        public IExpression Expression { get { return this.expression; } }

        public IEnumerable<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(Context context)
        {
            ICallable function = (ICallable) this.expression.Evaluate(context);

            IList<object> values = new List<object>();

            if (arguments != null)
                foreach (var argument in arguments)
                    values.Add(argument.Evaluate(context));

            return function.Call(context, values);
        }
    }
}

