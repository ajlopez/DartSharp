namespace DartSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseClass : IClass
    {
        private IClass super;
        private string name;
        private Dictionary<string, IMethod> methods;
        private Dictionary<string, IType> variables;

        public BaseClass(string name, IClass super)
        {
            this.name = name;
            this.super = super;
        }

        public string Name { get { return this.name; } }

        public IType Super { get { return this.super; } }

        public void DefineVariable(string name, IType type)
        {
            if (this.variables == null)
                this.variables = new Dictionary<string, IType>();
            else if (this.variables.ContainsKey(name))
                throw new InvalidOperationException("Variable already defined");

            this.variables[name] = type;
        }

        public void DefineMethod(string name, IMethod method)
        {
            throw new NotImplementedException();
        }

        public IType GetVariableType(string name)
        {
            if (this.variables == null || !this.variables.ContainsKey(name))
                if (this.super != null)
                    return this.super.GetVariableType(name);
                else
                    return null;

            return this.variables[name];
        }

        public IMethod GetMethod(string name)
        {
            throw new NotImplementedException();
        }
    }
}
