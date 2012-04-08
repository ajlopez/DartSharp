using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Compiler;
using DartSharp.Commands;
using DartSharp.Methods;
using System.IO;

namespace DartSharp.Tests
{
    [TestClass]
    public class EvaluateTests
    {
        [TestMethod]
        public void EvaluatePrintCommand()
        {
            Context context = new Context();
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            context.SetValue("print", print);
            EvaluateCommands("print('Hello, world');", context);
            writer.Close();
            Assert.AreEqual("Hello, world\r\n", writer.ToString());
        }

        [TestMethod]
        public void EvaluateSimpleIfCommand()
        {
            Context context = new Context();
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            context.SetValue("print", print);
            context.SetValue("a", 0);
            EvaluateCommands("if (a == 0) print('Hello, world');", context);
            writer.Close();
            Assert.AreEqual("Hello, world\r\n", writer.ToString());
        }

        [TestMethod]
        public void EvaluateSimpleIfCommandWithElse()
        {
            Context context = new Context();
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            context.SetValue("print", print);
            context.SetValue("a", 0);
            EvaluateCommands("if (a == 1) print('Hello'); else print('Hello, world');", context);
            writer.Close();
            Assert.AreEqual("Hello, world\r\n", writer.ToString());
        }

        private static void EvaluateCommands(string text, Context context)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseCommands();

            var command = new CompositeCommand(result);
            command.Execute(context);
        }
    }
}
