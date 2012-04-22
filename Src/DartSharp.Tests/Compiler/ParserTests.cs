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
        public void ParseLessOperator()
        {
            Parser parser = new Parser("a < 1");
            IExpression expr = parser.ParseExpression();

            Assert.IsNotNull(expr);
            Assert.IsInstanceOfType(expr, typeof(CompareExpression));

            CompareExpression cexpr = (CompareExpression)expr;

            Assert.AreEqual(ComparisonOperator.Less, cexpr.Operation);
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
            Parser parser = new Parser("var a;");
            var result = parser.ParseCommand();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineVariableCommand));

            DefineVariableCommand dvcmd = (DefineVariableCommand)result;
            Assert.AreEqual("a", dvcmd.Name);
            Assert.IsNull(dvcmd.Expression);
        }

        [TestMethod]
        public void ParseDefineVariableWithValue()
        {
            Parser parser = new Parser("var a = 1;");
            var result = parser.ParseCommand();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineVariableCommand));

            DefineVariableCommand dvcmd = (DefineVariableCommand)result;
            Assert.AreEqual("a", dvcmd.Name);
            Assert.IsNotNull(dvcmd.Expression);
            Assert.IsInstanceOfType(dvcmd.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseNameExpected()
        {
            Parser parser = new Parser("var 1;");
            parser.ParseCommand();
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
        public void ParseVoidFunctionDefinition()
        {
            Parser parser = new Parser("void myfun() { a = 1; b = 2; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("myfun", command.Name);
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        public void ParseIntFunctionDefinition()
        {
            Parser parser = new Parser("int myfun() { a = 1; b = 2; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("myfun", command.Name);
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        public void ParseIntFunctionDefinitionWithParameter()
        {
            Parser parser = new Parser("int myfun(int a) { return a+1; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("myfun", command.Name);
            Assert.IsNotNull(command.ArgumentNames);
            Assert.AreEqual(1, command.ArgumentNames.Count());
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        public void ParseIntFunctionDefinitionWithTwoParameters()
        {
            Parser parser = new Parser("int myfun(int a, int b) { return a+b; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("myfun", command.Name);
            Assert.IsNotNull(command.ArgumentNames);
            Assert.AreEqual(2, command.ArgumentNames.Count());
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfNoTypeInArgument()
        {
            Parser parser = new Parser("int myfun(a, b) { return a+b; }");
            parser.ParseCommand();
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfNoCommaToSeparateArguments()
        {
            Parser parser = new Parser("int myfun(int a int b) { return a+b; }");
            parser.ParseCommand();
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfMissingParenthesis()
        {
            Parser parser = new Parser("int myfun( { return a+b; }");
            parser.ParseCommand();
        }

        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void RaiseIfEndOfInput()
        {
            Parser parser = new Parser("int myfun(");
            parser.ParseCommand();
        }

        [TestMethod]
        public void ParseMainFunctionDefinition()
        {
            Parser parser = new Parser("main() { a = 1; b = 2; }");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefineFunctionCommand));

            DefineFunctionCommand command = (DefineFunctionCommand)result;

            Assert.AreEqual("main", command.Name);
            Assert.IsInstanceOfType(command.Command, typeof(CompositeCommand));
        }

        [TestMethod]
        public void ParseSimpleIfCommand()
        {
            Parser parser = new Parser("if (a) \r\n b = 2;");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IfCommand));

            IfCommand command = (IfCommand)result;

            Assert.IsNotNull(command.Condition);
            Assert.IsNotNull(command.ThenCommand);
            Assert.IsNull(command.ElseCommand);
        }

        [TestMethod]
        public void ParseSimpleWhileCommand()
        {
            Parser parser = new Parser("while (a) \r\n b = 2;");
            var result = parser.ParseCommand();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(WhileCommand));

            WhileCommand command = (WhileCommand)result;

            Assert.IsNotNull(command.Condition);
            Assert.IsNotNull(command.Command);
        }

        [TestMethod]
        public void ParseNullAsConstant()
        {
            Parser parser = new Parser("null");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression));

            ConstantExpression expression = (ConstantExpression)result;

            Assert.IsNull(expression.Value);
        }

        [TestMethod]
        public void ParseFalseAsConstant()
        {
            Parser parser = new Parser("false");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression));

            ConstantExpression expression = (ConstantExpression)result;

            Assert.AreEqual(false, expression.Value);
        }

        [TestMethod]
        public void ParseTrueAsConstant()
        {
            Parser parser = new Parser("true");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression));

            ConstantExpression expression = (ConstantExpression)result;

            Assert.AreEqual(true, expression.Value);
        }

        [TestMethod]
        public void ParseSimpleSum()
        {
            Parser parser = new Parser("1+2");
            var result = parser.ParseExpression();
            Assert.IsNull(parser.ParseExpression());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression expression = (ArithmeticBinaryExpression)result;

            Assert.IsInstanceOfType(expression.LeftExpression, typeof(ConstantExpression));
            Assert.IsInstanceOfType(expression.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseDotExpression()
        {
            Parser parser = new Parser("a.length");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DotExpression));

            DotExpression expression = (DotExpression)result;

            Assert.AreEqual("length", expression.Name);
            Assert.IsNull(expression.Arguments);
            Assert.IsInstanceOfType(expression.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseDotExpressionWithArguments()
        {
            Parser parser = new Parser("a.slice(1)");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DotExpression));

            DotExpression expression = (DotExpression)result;

            Assert.AreEqual("slice", expression.Name);
            Assert.IsNotNull(expression.Arguments);
            Assert.AreEqual(1, expression.Arguments.Count());
            Assert.IsInstanceOfType(expression.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseArrayExpression()
        {
            Parser parser = new Parser("[1,2,3]");
            var result = parser.ParseExpression();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ArrayExpression));

            ArrayExpression expr = (ArrayExpression)result;
            Assert.IsNotNull(expr.Expressions);
            Assert.AreEqual(3, expr.Expressions.Count());
        }
    }
}
