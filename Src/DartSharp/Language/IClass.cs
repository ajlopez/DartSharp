namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IClass : IType
    {
        void DefineVariable(string name, IType type);

        void DefineMethod(string name, IMethod method);
    }
}
