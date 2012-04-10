using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Compiler;

namespace DartSharp.Tests.Compiler
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void GetName()
        {
            Lexer lexer = new Lexer("foo");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("foo", token.Value);
            Assert.AreEqual(TokenType.Name, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNameWithSpaces()
        {
            Lexer lexer = new Lexer("  foo   ");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("foo", token.Value);
            Assert.AreEqual(TokenType.Name, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetNameWithLineComment()
        {
            Lexer lexer = new Lexer("  foo  // Foo Variable ");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("foo", token.Value);
            Assert.AreEqual(TokenType.Name, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetAssignmentOperator()
        {
            Lexer lexer = new Lexer("=");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("=", token.Value);
            Assert.AreEqual(TokenType.Operator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetDivideOperator()
        {
            Lexer lexer = new Lexer("/");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("/", token.Value);
            Assert.AreEqual(TokenType.Operator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetEqualOperator()
        {
            Lexer lexer = new Lexer("==");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("==", token.Value);
            Assert.AreEqual(TokenType.Operator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetPointSeparator()
        {
            Lexer lexer = new Lexer(".");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(".", token.Value);
            Assert.AreEqual(TokenType.Separator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetInteger()
        {
            Lexer lexer = new Lexer("123");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("123", token.Value);
            Assert.AreEqual(TokenType.Integer, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetIntegerWithSpaces()
        {
            Lexer lexer = new Lexer("  123   ");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("123", token.Value);
            Assert.AreEqual(TokenType.Integer, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetSimpleString()
        {
            Lexer lexer = new Lexer("\"foo\"");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("foo", token.Value);
            Assert.AreEqual(TokenType.String, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetSimpleStringSingleQuote()
        {
            Lexer lexer = new Lexer("'foo'");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("foo", token.Value);
            Assert.AreEqual(TokenType.String, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException))]
        public void RaiseUnclosedString()
        {
            Lexer lexer = new Lexer("'foo");
            lexer.NextToken();
        }

        [TestMethod]
        [ExpectedException(typeof(LexerException))]
        public void RaiseForUnknownOperator()
        {
            Lexer lexer = new Lexer("^");
            lexer.NextToken();
        }

        [TestMethod]
        public void GetEndOfCommand()
        {
            Lexer lexer = new Lexer(";");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(";", token.Value);
            Assert.AreEqual(TokenType.Separator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetCommaAsSeparator()
        {
            Lexer lexer = new Lexer(",");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(",", token.Value);
            Assert.AreEqual(TokenType.Separator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetParentheses()
        {
            Lexer lexer = new Lexer("()");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("(", token.Value);
            Assert.AreEqual(TokenType.Separator, token.Type);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(")", token.Value);
            Assert.AreEqual(TokenType.Separator, token.Type);

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void SkipNewLine()
        {
            Lexer lexer = new Lexer("\n");
            Token token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void SkipCarriageReturnNewLine()
        {
            Lexer lexer = new Lexer("\r\n");
            Token token = lexer.NextToken();

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void SkipNewLineCarriageReturn()
        {
            Lexer lexer = new Lexer("\n\r");
            Token token = lexer.NextToken();

            Assert.IsNull(lexer.NextToken());
        }

        [TestMethod]
        public void GetSimpleAssignmentCommand()
        {
            Lexer lexer = new Lexer("a=123");
            Token token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("a", token.Value);
            Assert.AreEqual(TokenType.Name, token.Type);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("=", token.Value);
            Assert.AreEqual(TokenType.Operator, token.Type);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual("123", token.Value);
            Assert.AreEqual(TokenType.Integer, token.Type);

            Assert.IsNull(lexer.NextToken());
        }
    }
}
