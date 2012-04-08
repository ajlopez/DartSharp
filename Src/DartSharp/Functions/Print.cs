namespace DartSharp.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Language;
    using System.IO;

    public class Print : ICallable
    {
        private TextWriter writer;

        public Print(TextWriter writer)
        {
            this.writer = writer;
        }

        public object Call(Context context, IList<object> arguments)
        {
            if (arguments == null || arguments.Count == 0)
                writer.WriteLine();
            else if (arguments.Count > 1)
                throw new InvalidOperationException("print accepts only one argument");
            else
                writer.WriteLine(arguments[0]);

            writer.Flush();

            return null;
        }
    }
}
