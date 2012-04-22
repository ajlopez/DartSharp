namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncMethod : IMethod
    {
        private IType type;
        private Func<object, Context, object[], object> func;

        public FuncMethod(IType type, Func<object, Context, object[], object> func)
        {
            this.type = type;
            this.func = func;
        }

        public IType Type { get { return this.type; } }

        public object Call(object self, Context context, object[] arguments)
        {
            return this.func(self, context, arguments);
        }
    }
}
