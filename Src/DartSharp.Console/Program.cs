using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DartSharp.Compiler;
using DartSharp.Commands;
using DartSharp.Methods;

namespace DartSharp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("DartSharp 0.0.0");

            Lexer lexer = new Lexer(System.Console.In);
            Context context = new Context();
            context.SetValue("print", new Print(System.Console.Out));
            Parser parser = new Parser(lexer);

            for (ICommand cmd = parser.ParseCommand(); cmd != null; cmd = parser.ParseCommand())
                cmd.Execute(context);
        }
    }
}

