namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Methods;

    public class CompositeCommand : ICommand
    {
        private IEnumerable<ICommand> commands;

        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            this.commands = commands;
        }

        public IEnumerable<ICommand> Commands { get { return this.commands; } }

        public object Execute(Context context)
        {
            object result = null;

            foreach (var command in this.commands)
                result = command.Execute(context);

            return result;
        }
    }
}
