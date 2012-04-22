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
            EvaluateCommands("if (a == 0) print('Hello, world');\r\n", context);
            writer.Close();
            Assert.AreEqual("Hello, world\r\n", writer.ToString());
        }

        [TestMethod]
        public void EvaluateSimpleReturn()
        {
            Context context = new Context();
            EvaluateCommands("return 0;", context);
            Assert.IsNotNull(context.ReturnValue);
            Assert.AreEqual(0, context.ReturnValue.Value);
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

        [TestMethod]
        public void EvaluateSimpleWhile()
        {
            Context context = new Context();
            context.SetValue("a", 0);
            EvaluateCommands("while (a < 10) a = a +1;", context);
            Assert.AreEqual(10, context.GetValue("a"));
        }

        [TestMethod]
        public void EvaluateSimpleArithmeticExpressions()
        {
            Assert.AreEqual(2, EvaluateExpression("1+1", null));
            Assert.AreEqual(-1, EvaluateExpression("1-2", null));
            Assert.AreEqual(6, EvaluateExpression("2*3", null));
            Assert.AreEqual(3.0, EvaluateExpression("6/2", null));
            Assert.AreEqual(10, EvaluateExpression("(2+3)*2", null));
        }

        [TestMethod]
        public void EvaluateStringConcatenation()
        {
            Assert.AreEqual("foobar", EvaluateExpression("'foo' + 'bar'", null));
            Assert.AreEqual("foo1", EvaluateExpression("'foo' + 1", null));
        }

        [TestMethod]
        public void EvaluateNameInStringInterpolation()
        {
            Context context = new Context();
            context.SetValue("name", "World");
            Assert.AreEqual("Hello, World!", EvaluateExpression("'Hello, $name!'", context));
        }

        [TestMethod]
        public void EvaluateNameInSimpleStringInterpolation()
        {
            Context context = new Context();
            context.SetValue("name", "World");
            Assert.AreEqual("World", EvaluateExpression("'$name'", context));
        }

        [TestMethod]
        public void EvaluateExpressionInStringInterpolation()
        {
            Context context = new Context();
            context.SetValue("name", "World");
            Assert.AreEqual("Hello, WORLD!", EvaluateExpression("'Hello, ${name.ToUpper()}!'", context));
        }

        [TestMethod]
        public void EvaluateNameAndExpressionInStringInterpolation()
        {
            Context context = new Context();
            context.SetValue("name", "World");
            Assert.AreEqual("Hello, World, WORLD!", EvaluateExpression("'Hello, $name, ${name.ToUpper()}!'", context));
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfUnclosedExpressionInInterpolation()
        {
            EvaluateExpression("'Hello, ${name.ToUpper()!'", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfNoNameInterpolation()
        {
            EvaluateExpression("'Hello, $0!'", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfNoInterpolation()
        {
            EvaluateExpression("'Hello, $'", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfBadExpressionInInterpolation()
        {
            EvaluateExpression("'Hello, ${0+1 2+3}'", null);
        }

        [TestMethod]
        public void EvaluateSimpleCompareExpressions()
        {
            Assert.AreEqual(true, EvaluateExpression("1==1", null));
            Assert.AreEqual(true, EvaluateExpression("1<2", null));
            Assert.AreEqual(false, EvaluateExpression("1<1", null));
            Assert.AreEqual(true, EvaluateExpression("1<=2", null));
            Assert.AreEqual(true, EvaluateExpression("1<=1", null));
            Assert.AreEqual(false, EvaluateExpression("1>=2", null));
            Assert.AreEqual(true, EvaluateExpression("1>=1", null));
        }

        [TestMethod]
        public void EvaluateSimpleFunctionCall()
        {
            Context context = new Context();
            EvaluateCommands("int foo() { return 1; } a = foo();", context);
            Assert.AreEqual(1, context.GetValue("a"));
        }

        [TestMethod]
        public void EvaluateSimpleFunctionCallWithArgument()
        {
            Context context = new Context();
            EvaluateCommands("int inc(int n) { return n+1; } a = inc(1);", context);
            Assert.AreEqual(2, context.GetValue("a"));
        }

        [TestMethod]
        public void EvaluateSimpleDotExpressions()
        {
            Context context = new Context();
            Assert.AreEqual(3, EvaluateExpression("'foo'.Length", context));
            Assert.AreEqual("FOO", EvaluateExpression("'foo'.ToUpper()", context));
            Assert.AreEqual("oo", EvaluateExpression("'foo'.Substring(1)", context));
        }

        [TestMethod]
        public void DefineVariables()
        {
            Context context = new Context();
            EvaluateCommands("var a; int b; double c; String d; bool e;", context);
            Assert.IsTrue(context.HasVariable("a"));
            Assert.IsTrue(context.HasVariable("b"));
            Assert.IsTrue(context.HasVariable("c"));
            Assert.IsTrue(context.HasVariable("d"));
            Assert.IsTrue(context.HasVariable("e"));
        }

        private static object EvaluateExpression(string text, Context context)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpression();

            Assert.IsNull(parser.ParseExpression());

            return result.Evaluate(context);
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
