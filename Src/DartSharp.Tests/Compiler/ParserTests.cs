using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Compiler;
using DartSharp.Expressions;
using DartSharp.Commands;
using DartSharp.Language;

namespace DartSharp.Tests.Compiler
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseInteger()
        {
            Parser parser = new Parser("123");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)expr;

            Assert.AreEqual(123, cexpr.Evaluate(null));

            Assert.IsNull(parser.ParseExpression());
        }

        [TestMethod]
        public void ParseSimpleString()
        {
            Parser parser = new Parser("\"foo\"");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)expr;

            Assert.AreEqual("foo", cexpr.Evaluate(null));

            Assert.IsNull(parser.ParseExpression());
        }

        [TestMethod]
        public void ParseVariable()
        {
            Parser parser = new Parser("foo");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(VariableExpression));

            VariableExpression vexpr = (VariableExpression)expr;

            Assert.AreEqual("foo", vexpr.Name);

            Assert.IsNull(parser.ParseExpression());
        }

        [TestMethod]
        public void ParseSimpleCall()
        {
            Parser parser = new Parser("prints(1)");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(CallExpression));

            CallExpression cexpr = (CallExpression)expr;

            Assert.IsInstanceOfType(cexpr.Expression, typeof(VariableExpression));
            Assert.AreEqual(1, cexpr.Arguments.Count());
            Assert.IsInstanceOfType(cexpr.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfMissingClosingParenthesis()
        {
            Parser parser = new Parser("prints(1");
            parser.ParseExpression();
        }

        [TestMethod]
        public void ParseEqualsOperator()
        {
            Parser parser = new Parser("a == 1");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(CompareExpression));

            CompareExpression cexpr = (CompareExpression)expr;

            Assert.AreEqual(ComparisonOperator.Equal, cexpr.Operation);
            Assert.IsInstanceOfType(cexpr.LeftExpression, typeof(VariableExpression));
            Assert.IsInstanceOfType(cexpr.RightExpression, typeof(ConstantExpression));

            Assert.IsNull(parser.ParseExpression());
        }

        [TestMethod]
        public void ParseSimpleCallWithParenthesisAndTwoArguments()
        {
            Parser parser = new Parser("myfunc(a, b)");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(CallExpression));

            CallExpression cexpr = (CallExpression)expr;

            Assert.IsInstanceOfType(cexpr.Expression, typeof(VariableExpression));
            Assert.AreEqual(2, cexpr.Arguments.Count());
            Assert.IsInstanceOfType(cexpr.Arguments.First(), typeof(VariableExpression));
            Assert.IsInstanceOfType(cexpr.Arguments.Skip(1).First(), typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseSimpleCallAsCommand()
        {
            Parser parser = new Parser("prints(1);");
            ICommand cmd = parser.ParseCommand();

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOfType(cmd, typeof(ExpressionCommand));

            ExpressionCommand ccmd = (ExpressionCommand) cmd;

            Assert.IsInstanceOfType(ccmd.Expression, typeof(CallExpression));
        }

        [TestMethod]
        public void ParseSimpleCallAsCommandPrecededByNewLine()
        {
            Parser parser = new Parser("\r\nprints(1);");
            ICommand cmd = parser.ParseCommand();

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOfType(cmd, typeof(ExpressionCommand));

            ExpressionCommand ccmd = (ExpressionCommand)cmd;

            Assert.IsInstanceOfType(ccmd.Expression, typeof(CallExpression));
        }

        [TestMethod]
        public void ParseTwoSimpleCallsAsCommands()
        {
            Parser parser = new Parser("print(1);\r\nprint(2);\r\n");
            ICommand cmd = parser.ParseCommand();

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOfType(cmd, typeof(ExpressionCommand));

            ExpressionCommand ccmd = (ExpressionCommand)cmd;

            Assert.IsInstanceOfType(ccmd.Expression, typeof(CallExpression));

            cmd = parser.ParseCommand();

            Assert.IsNotNull(cmd);
            Assert.IsInstanceOfType(cmd, typeof(ExpressionCommand));

            ccmd = (ExpressionCommand)cmd;

            Assert.IsInstanceOfType(ccmd.Expression, typeof(CallExpression));

            Assert.IsNull(parser.ParseCommand());
        }

        [TestMethod]
        public void ParseSimpleAssignmentCommand()
        {
            Parser parser = new Parser("a=1;");
            ICommand command = parser.ParseCommand();

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            SetVariableCommand scommand = (SetVariableCommand)command;

            Assert.AreEqual("a", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            Assert.IsNull(parser.ParseCommand());
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfSemicolonIsMissing()
        {
            Parser parser = new Parser("a=1");
            parser.ParseCommand();
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfBinaryOperator()
        {
            Parser parser = new Parser("==");
            parser.ParseExpression();
        }

        [TestMethod]
        public void ParseTwoLineSimpleAssignmentCommands()
        {
            Parser parser = new Parser("a=1;\r\nb=1;");
            ICommand command = parser.ParseCommand();

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            SetVariableCommand scommand = (SetVariableCommand)command;

            Assert.AreEqual("a", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            command = parser.ParseCommand();

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            scommand = (SetVariableCommand)command;

            Assert.AreEqual("b", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            Assert.IsNull(parser.ParseCommand());
        }

        [TestMethod]
        public void ParseSeparatedSimpleAssignmentCommands()
        {
            Parser parser = new Parser("a=1;b=1;");
            ICommand command = parser.ParseCommand();

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            SetVariableCommand scommand = (SetVariableCommand)command;

            Assert.AreEqual("a", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            command = parser.ParseCommand();

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            scommand = (SetVariableCommand)command;

            Assert.AreEqual("b", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            Assert.IsNull(parser.ParseCommand());
        }

        [TestMethod]
        public void ParseTwoCommands()
        {
            Parser parser = new Parser("a=1;\r\nb=1;");
            IList<ICommand> commands = parser.ParseCommands();

            Assert.IsNotNull(commands);
            Assert.AreEqual(2, commands.Count);

            ICommand command = commands[0];

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            SetVariableCommand scommand = (SetVariableCommand)command;

            Assert.AreEqual("a", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            ConstantExpression cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            command = commands[1];

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            scommand = (SetVariableCommand)command;

            Assert.AreEqual("b", scommand.Name);
            Assert.IsInstanceOfType(scommand.Expression, typeof(ConstantExpression));

            cexpr = (ConstantExpression)scommand.Expression;

            Assert.AreEqual(1, cexpr.Value);

            Assert.IsNull(parser.ParseCommand());
        }

        [TestMethod]
        public void ParseNoCommands()
        {
            Parser parser = new Parser("");
            Assert.IsNull(parser.ParseCommands());
        }

        [TestMethod]
        public void ParseClosingBraceAsNoCommand()
        {
            Parser parser = new Parser("}");
            Assert.IsNull(parser.ParseCommands());
        }

        [TestMethod]
        public void ParseClosingParenthesisAsNoExpression()
        {
            Parser parser = new Parser(")");
            Assert.IsNull(parser.ParseExpression());
        }

        [TestMethod]
        public void ParseEmptyCommand()
        {
            Parser parser = new Parser(";");
            Assert.AreEqual(NullCommand.Instance, parser.ParseCommand());
        }

        [TestMethod]
        public void ParseDefineVariable()
        {
            Parser parser = new Parser("var a");
            var result = parser.ParseExpression();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineVariableExpression));

            DefineVariableExpression dvexpr = (DefineVariableExpression)result;
            Assert.AreEqual("a", dvexpr.Name);
            Assert.IsNull(dvexpr.Expression);
        }

        [TestMethod]
        public void ParseDefineVariableWithValue()
        {
            Parser parser = new Parser("var a = 1");
            var result = parser.ParseExpression();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineVariableExpression));

            DefineVariableExpression dvexpr = (DefineVariableExpression)result;
            Assert.AreEqual("a", dvexpr.Name);
            Assert.IsNotNull(dvexpr.Expression);
            Assert.IsInstanceOfType(dvexpr.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseNameExpected()
        {
            Parser parser = new Parser("var 1");
            parser.ParseExpression();
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseNameExpectedAfterVar()
        {
            Parser parser = new Parser("var");
            parser.ParseExpression();
        }

        [TestMethod]
        public void ParseCompositeCommand()
        {
            Parser parser = new Parser("{ a = 1; b = 2; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CompositeCommand));

            CompositeCommand command = (CompositeCommand)result;

            Assert.AreEqual(2, command.Commands.Count());
        }

        [TestMethod]
        public void ParseFunctionDefinition()
        {
            Parser parser = new Parser("main() { a = 1; b = 2; }");
            var result = parser.ParseTopCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("main", command.Name);
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        public void ParseDefineVariableAsTopCommand()
        {
            Parser parser = new Parser("var a = 1;");
            var result = parser.ParseTopCommand();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ExpressionCommand));
        }
    }
}
