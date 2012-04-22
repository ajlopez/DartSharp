namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Expressions;
    using DartSharp.Language;

    public class DefineVariableCommand : ICommand
    {
        private IExpression typeexpression;
        private string name;
        private IExpression expression;

        public DefineVariableCommand(IExpression typeexpression, string name)
            : this(typeexpression, name, null)
        {
        }

        public DefineVariableCommand(IExpression typeexpression, string name, IExpression expression)
        {
            this.typeexpression = typeexpression;
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public IExpression TypeExpression { get { return this.typeexpression; } }

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

