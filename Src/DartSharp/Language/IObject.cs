namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IObject
    {
        object GetValue(string name);

        void SetValue(string name, object value);

        ICollection<string> GetNames();

        object Invoke(string name, object[] parameters);

        object Invoke(ICallable method, object[] parameters);
    }
}
