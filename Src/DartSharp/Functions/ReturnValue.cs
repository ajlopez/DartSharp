namespace DartSharp.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ReturnValue
    {
        private object value;

        public ReturnValue(object value)
        {
            this.value = value;
        }

        public object Value { get { return this.value; } }
    }
}
