namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DartSharp.Expressions;
    using DartSharp.Functions;

    public class ReturnCommand : ICommand
    {
        private IExpression expression;

        public ReturnCommand(IExpression expression)
        {
            this.expression = expression;
        }

        public IExpression Expression { get { return this.expression; } }

        public object Execute(Context context)
        {
            object value = this.expression.Evaluate(context);
            context.ReturnValue = new ReturnValue(value);
            return value;
        }
    }
}
