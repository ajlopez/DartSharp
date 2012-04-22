namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IMethod
    {
        IType Type { get; }

        object Call(object self, Context context, object[] arguments);
    }
}
