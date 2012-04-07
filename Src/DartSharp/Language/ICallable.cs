namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ICallable
    {
        object Call(Context context, IList<object> arguments);
    }
}
