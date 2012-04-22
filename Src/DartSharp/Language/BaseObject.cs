namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseObject : IObject
    {
        private IClass @class;
        private Dictionary<string, object> values;

        public BaseObject(IClass @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class");

            this.@class = @class;
        }

        public IClass Class
        {
            get { return this.@class; }
        }

        public object GetValue(string name)
        {
            if (this.@class.GetVariableType(name) == null)
                throw new InvalidOperationException(string.Format("Undefined Variable '{0}'", name));

            if (this.values == null || !this.values.ContainsKey(name))
                return null;

            return this.values[name];
        }

        public void SetValue(string name, object value)
        {
            if (this.@class.GetVariableType(name) == null)
                throw new InvalidOperationException(string.Format("Undefined Variable '{0}'", name));

            if (this.values == null)
                this.values = new Dictionary<string, object>();
            this.values[name] = value;
        }

        public object Invoke(string name, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
