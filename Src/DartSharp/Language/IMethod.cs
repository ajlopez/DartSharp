namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IMethod
    {
        IClass Type { get; }

        object Call(object self, Context context, IList<object> arguments);
    }
}
