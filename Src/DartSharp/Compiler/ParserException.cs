namespace DartSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ParserException : Exception
    {
        public ParserException(string msg)
            : base(msg)
        {
        }
    }
}
