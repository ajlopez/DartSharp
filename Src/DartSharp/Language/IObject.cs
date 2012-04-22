namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IObject
    {
        IClass Class { get; }

        object GetValue(string name);

        void SetValue(string name, object value);

        object Invoke(string name, object[] parameters);
    }
}
