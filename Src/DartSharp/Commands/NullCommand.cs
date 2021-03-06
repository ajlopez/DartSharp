﻿namespace DartSharp.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DartSharp.Expressions;

    public class NullCommand : ICommand
    {
        private static NullCommand instance = new NullCommand();

        private NullCommand()
        {
        }

        public static NullCommand Instance { get { return instance; } }

        public object Execute(Context context)
        {
            return null;
        }
    }
}
