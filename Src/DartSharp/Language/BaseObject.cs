namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseObject : IObject
    {
        private IType type;
        private Dictionary<string, object> values;

        public BaseObject(IType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.type = type;
        }

        public IType Type
        {
            get { return this.type; }
        }

        public object GetValue(string name)
        {
            if (this.type.GetVariableType(name) == null)
                throw new InvalidOperationException(string.Format("Undefined Variable '{0}'", name));

            if (this.values == null || !this.values.ContainsKey(name))
                return null;

            return this.values[name];
        }

        public void SetValue(string name, object value)
        {
            if (this.type.GetVariableType(name) == null)
                throw new InvalidOperationException(string.Format("Undefined Variable '{0}'", name));

            if (this.values == null)
                this.values = new Dictionary<string, object>();
            this.values[name] = value;
        }

        public object Invoke(string name, Context context, object[] parameters)
        {
            IMethod method = this.type.GetMethod(name);

            if (method == null)
                throw new InvalidOperationException(string.Format("Undefined Method '{0}'", name));

            return method.Call(this, context, parameters);
        }
    }
}
