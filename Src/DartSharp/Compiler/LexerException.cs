namespace DartSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LexerException : Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }
    }
}
