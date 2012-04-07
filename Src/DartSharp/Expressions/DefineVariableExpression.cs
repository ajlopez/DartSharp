namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;

    public class DefineVariableExpression : IExpression
    {
        private string name;
        private IExpression expression;

        public DefineVariableExpression(string name)
            : this(name, null)
        {
        }

        public DefineVariableExpression(string name, IExpression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public object Evaluate(Context context)
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

