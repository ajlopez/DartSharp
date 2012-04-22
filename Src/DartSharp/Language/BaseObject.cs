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
            this.@class = @class;
        }

        public IClass Class
        {
            get { return this.@class; }
        }

        public object GetValue(string name)
        {
            if (this.values == null || !this.values.ContainsKey(name))
                return null;

            return this.values[name];
        }

        public void SetValue(string name, object value)
        {
            if (this.values == null)
                this.values = new Dictionary<string, object>();
            this.values[name] = value;
        }

        public IEnumerable<string> GetNames()
        {
            throw new NotImplementedException();
        }

        public object Invoke(string name, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
