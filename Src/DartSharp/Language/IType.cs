namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IType
    {
        string Name { get; }

        IType Super { get; }

        IType GetVariableType(string name);

        IMethod GetMethod(string name);
    }
}
