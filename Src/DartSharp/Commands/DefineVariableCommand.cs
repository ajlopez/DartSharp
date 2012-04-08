namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;
    using DartSharp.Expressions;

    public class DefineVariableCommand : ICommand
    {
        private string name;
        private IExpression expression;

        public DefineVariableCommand(string name)
            : this(name, null)
        {
        }

        public DefineVariableCommand(string name, IExpression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public object Execute(Context context)
        {
            context.DefineVariable(this.name);

            if (this.expression != null)
            {
                var value = this.expression.Evaluate(context);
                context.SetValue(this.name, value);
                return value;
            }

            return null;
        }
    }
}

