namespace DartSharp.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;
    using DartSharp.Commands;

    public class DefinedFunction : ICallable
    {
        private ICommand command;
        private IEnumerable<string> argnames;

        public DefinedFunction(IEnumerable<string> argnames, ICommand command)
        {
            this.argnames = argnames;
            this.command = command;
        }

        public object Call(Context context, IList<object> arguments)
        {
            Context newctx = new Context(context);

            return this.command.Execute(newctx);
        }
    }
}
