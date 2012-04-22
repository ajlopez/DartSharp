namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Methods;
    using DartSharp.Language;

    public class DefineClassCommand : ICommand
    {
        private string name;
        private ICommand command;

        public DefineClassCommand(string name, ICommand command)
        {
            this.name = name;
            this.command = command;
        }

        public string Name { get { return this.name; } }

        public ICommand Command { get { return this.command; } }

        public object Execute(Context context)
        {
            IType type = new BaseClass(this.name, (IClass)context.GetValue("Object"));
            context.SetValue(this.name, type);
            return null;
        }
    }
}
