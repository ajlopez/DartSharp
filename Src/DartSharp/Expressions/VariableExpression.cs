namespace DartSharp.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;

    public class VariableExpression : IExpression
    {
        private IExpression typeexpression;
        private string name;

        public VariableExpression(string name)
        {
            this.name = name;
        }

        public VariableExpression(IExpression typeexpression, string name)
        {
            this.typeexpression = typeexpression;
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public IExpression TypeExpression { get { return this.typeexpression; } }

        public object Evaluate(Context context)
        {
            object result = context.GetValue(this.name);

            return result;
        }
    }
}

