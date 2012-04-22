namespace DartSharp.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DotExpression : IExpression
    {
        private IExpression expression;
        private string name;
        private IEnumerable<IExpression> arguments;
        private Type type;

        public DotExpression(IExpression expression, string name)
            : this(expression, name, null)
        {
        }

        public DotExpression(IExpression expression, string name, IEnumerable<IExpression> arguments)
        {
            this.expression = expression;
            this.name = name;
            this.arguments = arguments;
            this.type = AsType(this.expression);
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public Type Type { get { return this.type; } }

        public IEnumerable<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(Context context)
        {
            object obj = null;

            if (this.type == null)
               obj = this.expression.Evaluate(context);

            object[] parameters = null;

            if (this.arguments != null)
            {
                List<object> values = new List<object>();

                foreach (IExpression argument in this.arguments)
                    values.Add(argument.Evaluate(context));

                parameters = values.ToArray();
            }

            if (this.type != null)
                return TypeUtilities.InvokeTypeMember(this.type, this.name, parameters);

            if (obj is Type)
                return TypeUtilities.InvokeTypeMember((Type)obj, this.name, parameters);

            // TODO if undefined, do nothing
            if (obj == null)
                return null;

            return ObjectUtilities.GetValue(obj, this.name, parameters);
        }

        private static Type AsType(IExpression expression)
        {
            string name = AsName(expression);

            if (name == null)
                return null;

            return TypeUtilities.AsType(name);
        }

        private static string AsName(IExpression expression)
        {
            if (expression is VariableExpression)
                return ((VariableExpression)expression).Name;

            if (expression is DotExpression)
            {
                DotExpression dot = (DotExpression)expression;

                return AsName(dot.Expression) + "." + dot.Name;
            }

            return null;
        }
    }
}
