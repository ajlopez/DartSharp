namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IObject
    {
        IType Type { get; }

        object GetValue(string name);

        void SetValue(string name, object value);

        object Invoke(string name, Context context, object[] parameters);
    }
}
