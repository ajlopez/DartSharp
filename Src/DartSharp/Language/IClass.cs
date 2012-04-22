namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IClass
    {
        IClass Superclass { get; }

        void DefineVariable(string name, IClass type);

        void DefineMethod(string name, IMethod method);

        IClass GetVariableType(string name);

        IMethod GetMethod(string name);
    }
}
