namespace DartSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Context
    {
        private Context parent;
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public Context()
        {
        }

        public Context(Context parent)
        {
            this.parent = parent;
        }

        public void SetValue(string name, object value)
        {
            this.values[name] = value;
        }

        public object GetValue(string name)
        {
            if (!this.values.ContainsKey(name))
                if (this.parent != null)
                    return this.parent.GetValue(name);
                else
                    return null;
            
            return this.values[name];
        }
    }
}
