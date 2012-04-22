namespace DartSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DartSharp.Expressions;
    using System.Globalization;
    using DartSharp.Commands;
    using DartSharp.Language;

    public class Parser
    {
        private static string[] typenames = new string[] { "var", "void", "int", "String" };
        private Lexer lexer;
        private Stack<Token> tokens = new Stack<Token>();

        public Parser(string text)
            : this(new Lexer(text))
        {
        }

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        public IExpression ParseExpression()
        {
            var expression = this.ParseBinaryExpressionLevelMultiply();

            if (expression == null)
                return null;

            Token token = this.NextToken();

            if (token == null)
                return expression;

            if (token.Type == TokenType.Operator)
            {
                if (token.Value == "==")
                    return new CompareExpression(ComparisonOperator.Equal, expression, this.ParseExpression());
                if (token.Value == "<")
                    return new CompareExpression(ComparisonOperator.Less, expression, this.ParseExpression());
                if (token.Value == ">")
                    return new CompareExpression(ComparisonOperator.Greater, expression, this.ParseExpression());
                if (token.Value == "<=")
                    return new CompareExpression(ComparisonOperator.LessEqual, expression, this.ParseExpression());
                if (token.Value == ">=")
                    return new CompareExpression(ComparisonOperator.GreaterEqual, expression, this.ParseExpression());
            }

            this.PushToken(token);

            return expression;
        }

        private IExpression ParseBinaryExpressionLevelMultiply()
        {
            IExpression expression = this.ParseBinaryExpressionLevelAdd();

            if (expression == null)
                return null;

            Token token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && (token.Value == "*" || token.Value == "/"))
            {
                ArithmeticOperator oper = (token.Value == "*" ? ArithmeticOperator.Multiply : ArithmeticOperator.Divide);
                expression = new ArithmeticBinaryExpression(oper, expression, this.ParseBinaryExpressionLevelAdd());
                token = this.NextToken();
            }

            if (token != null)
                this.PushToken(token);

            return expression;
        }

        private IExpression ParseBinaryExpressionLevelAdd()
        {
            IExpression expression = this.ParseDotExpression();

            if (expression == null)
                return null;

            Token token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && (token.Value == "+" || token.Value == "-"))
            {
                ArithmeticOperator oper = (token.Value == "+" ? ArithmeticOperator.Add : ArithmeticOperator.Subtract);
                expression = new ArithmeticBinaryExpression(oper, expression, this.ParseDotExpression());
                token = this.NextToken();
            }

            if (token != null)
                this.PushToken(token);

            return expression;
        }

        private IExpression ParseDotExpression()
        {
            IExpression expression = this.ParseSimpleExpression();

            if (expression == null)
                return null;

            while (TryParseToken(".", TokenType.Separator))
            {
                string name = this.ParseName();
                if (TryParseToken("(", TokenType.Separator))
                {
                    expression = new DotExpression(expression, name, this.ParseExpressionList(")"));
                }
                else
                    expression = new DotExpression(expression, name);
            }

            return expression;
        }

        private IExpression ParseSimpleExpression()
        {
            Token token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Separator && (token.Value == "}" || token.Value == ")"))
            {
                this.PushToken(token);
                return null;
            }

            switch (token.Type)
            {
                case TokenType.Integer:
                    return new ConstantExpression(int.Parse(token.Value, CultureInfo.InvariantCulture));
                
                case TokenType.Name:
                    string name = token.Value;

                    if (name == "null")
                        return new ConstantExpression(null);
                    if (name == "true")
                        return new ConstantExpression(true);
                    if (name == "false")
                        return new ConstantExpression(false);

                    token = this.NextToken();

                    if (token != null && token.Type == TokenType.Separator && token.Type == TokenType.Separator && token.Value == "(")
                    {
                        return new CallExpression(new VariableExpression(name), this.ParseExpressionList(")"));
                    }

                    IExpression expr = new VariableExpression(name);

                    if (token != null)
                        this.PushToken(token);

                    return expr;

                case TokenType.String:
                    if (token.Value.Contains('$'))
                        return ParseStringInterpolation(token.Value);

                    return new ConstantExpression(token.Value);

                case TokenType.Separator:
                    if (token.Value == "(")
                    {
                        var result = this.ParseExpression();
                        this.ParseToken(")", TokenType.Separator);
                        return result;
                    }

                    if (token.Value == "[")
                    {
                        var result = this.ParseExpressionList("]");
                        return new ArrayExpression(result);
                    }

                    break;
            }

            throw new ParserException(string.Format("Unexpected '{0}'", token.Value));
        }

        public IList<ICommand> ParseCommands()
        {
            ICommand command = this.ParseCommand();

            if (command == null)
                return null;

            IList<ICommand> commands = new List<ICommand>();
            commands.Add(command);

            while ((command = this.ParseCommand()) != null)
                commands.Add(command);

            return commands;
        }

        public ICommand ParseCommand()
        {
            Token token = this.NextToken();

            if (token == null)
                return null;

            if (token.Type == TokenType.Separator)
            {
                if (token.Value == ";")
                    return NullCommand.Instance;

                if (token.Value == "{")
                {
                    ICommand commands = new CompositeCommand(this.ParseCommands());
                    this.ParseToken("}", TokenType.Separator);
                    return commands;
                }
            }

            if (token.Type == TokenType.Name)
            {
                if (token.Value == "if")
                    return this.ParseIfCommand();

                if (token.Value == "while")
                    return this.ParseWhileCommand();

                if (token.Value == "return")
                {
                    var expr = this.ParseExpression();
                    this.ParseToken(";", TokenType.Separator);
                    return new ReturnCommand(expr);
                }

                if (token.Value == "void")
                    return this.ParseDefineFunction(this.ParseName());

                if (token.Value == "main")
                    return this.ParseDefineFunction(token.Value);
            }

            this.PushToken(token);

            IExpression expression = this.ParseExpression();

            if (expression == null)
                return null;

            token = this.NextToken();

            if (token != null && token.Type == TokenType.Name && IsTypeExpression(expression))
            {
                if (this.TryPeekToken("(", TokenType.Separator))
                    return this.ParseDefineFunction(token.Value);

                return this.ParseDefineVariableCommand(expression, token.Value);
            }

            ICommand command = null;

            if (expression is VariableExpression && token.Type == TokenType.Operator && token.Value == "=")
            {
                string name = ((VariableExpression)expression).Name;
                IExpression expr = this.ParseExpression();

                command = new SetVariableCommand(name, expr);

                token = this.NextToken();
            }
            else
                command = new ExpressionCommand(expression);

            if (token == null || token.Type != TokenType.Separator || token.Value != ";")
                throw new ParserException("Expected ';'");

            return command;;
        }

        private IExpression ParseStringInterpolation(string text)
        {
            IList<IExpression> expressions = new List<IExpression>();

            while (true)
            {
                int pos = text.IndexOf('$');

                if (pos < 0)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        IExpression textexpr = new ConstantExpression(text);
                        expressions.Add(textexpr);
                    }

                    break;
                }

                if (pos == text.Length - 1)
                    throw new ParserException("Unexpected End of String");

                string left = text.Substring(0, pos);

                if (!string.IsNullOrEmpty(left))
                    expressions.Add(new ConstantExpression(left));

                if (text[pos + 1] == '{')
                {
                    int pos2 = text.IndexOf('}', pos + 1);

                    if (pos2 < 0)
                        throw new ParserException("Unexpected End of String");

                    string subtext = text.Substring(pos + 2, pos2 - pos - 2);
                    text = text.Substring(pos2 + 1);

                    Parser parser = new Parser(subtext);

                    IExpression newexpr = parser.ParseExpression();

                    if (parser.ParseExpression() != null)
                        throw new ParserException("Bad String Interpolation");

                    expressions.Add(newexpr);
                }
                else if (char.IsLetter(text[pos + 1]))
                {
                    Parser parser = new Parser(text.Substring(pos + 1));
                    string name = parser.ParseName();
                    IExpression varexpr = new VariableExpression(name);
                    expressions.Add(varexpr);
                    text = text.Substring(pos + name.Length + 1);
                }
                else
                    throw new ParserException("Bad String Interpolation");
            }

            if (expressions.Count == 1)
                return expressions[0];

            IExpression expression = expressions[0];

            foreach (var expr in expressions.Skip(1))
                expression = new ArithmeticBinaryExpression(ArithmeticOperator.Add, expression, expr);

            return expression;
        }

        private DefineFunctionCommand ParseDefineFunction(string name)
        {
            IList<string> argnames = this.ParseArgumentNames();
            ICommand body = this.ParseCommand();
            return new DefineFunctionCommand(name, argnames, body);
        }

        private ICommand ParseDefineVariableCommand(IExpression typeexpression, string name)
        {
            DefineVariableCommand command;

            if (this.TryParseToken("=", TokenType.Operator))
                command = new DefineVariableCommand(typeexpression, name, this.ParseExpression());
            else
                command =  new DefineVariableCommand(typeexpression, name);

            this.ParseToken(";", TokenType.Separator);

            return command;
        }

        private ICommand ParseIfCommand()
        {
            this.ParseToken("(", TokenType.Separator);
            IExpression condition = this.ParseExpression();
            this.ParseToken(")", TokenType.Separator);

            ICommand thencommand = this.ParseCommand();
            ICommand elsecommand = null;

            if (this.TryParseToken("else", TokenType.Name))
                elsecommand = this.ParseCommand();

            return new IfCommand(condition, thencommand, elsecommand);
        }

        private ICommand ParseWhileCommand()
        {
            this.ParseToken("(", TokenType.Separator);
            IExpression condition = this.ParseExpression();
            this.ParseToken(")", TokenType.Separator);

            ICommand command = this.ParseCommand();

            return new WhileCommand(condition, command);
        }

        private IEnumerable<IExpression> ParseExpressionList(string upto)
        {
            IList<IExpression> expressions = new List<IExpression>();

            while (!this.TryParseToken(upto, TokenType.Separator))
            {
                if (expressions.Count > 0)
                    this.ParseToken(",", TokenType.Separator);
                expressions.Add(this.ParseExpression());
            }

            return expressions;
        }

        private IList<string> ParseArgumentNames()
        {
            this.ParseToken("(", TokenType.Separator);

            IList<string> arguments = new List<string>();

            Token token = this.NextToken();

            while (token != null && token.Type == TokenType.Name)
            {
                this.PushToken(token);
                IExpression expr = this.ParseExpression();
                if (!this.IsTypeExpression(expr))
                    throw new ParserException(string.Format("Unexpected '{0}'", token.Value));
                
                string name = this.ParseName();
                arguments.Add(name);

                token = this.NextToken();

                if (token != null && token.Type == TokenType.Separator && token.Value == ")")
                    break;

                if (token == null || token.Type != TokenType.Separator || token.Value != ",")
                    throw new ParserException("Expected ',' or ')'");

                token = this.NextToken();
            }

            if (token == null || token.Type != TokenType.Separator || token.Value != ")")
                throw new ParserException("Expected ')'");

            return arguments;
        }

        private bool TryParseToken(string value, TokenType type)
        {
            Token token = this.NextToken();

            if (token != null && token.Value == value && token.Type == type)
                return true;

            this.PushToken(token);

            return false;
        }

        private bool TryPeekToken(string value, TokenType type)
        {
            Token token = this.NextToken();

            this.PushToken(token);

            if (token != null && token.Value == value && token.Type == type)
                return true;

            return false;
        }

        private void ParseToken(string value, TokenType type)
        {
            Token token = this.NextToken();

            if (token == null || token.Value != value || token.Type != type)
                throw new ParserException(string.Format("Expected '{0}'", value));
        }

        private string ParseName()
        {
            Token token = this.NextToken();

            if (token == null || token.Type != TokenType.Name)
                throw new ParserException("Name expected");

            return token.Value;
        }

        private Token NextToken()
        {
            if (this.tokens.Count>0)
                return this.tokens.Pop();

            return this.lexer.NextToken();
        }

        private void PushToken(Token token)
        {
            this.tokens.Push(token);
        }

        private bool IsTypeExpression(IExpression expression)
        {
            if (expression is VariableExpression)
                return true;

            if (expression is DotExpression && ((DotExpression)expression).Arguments == null)
                return true;

            return false;
        }
    }
}
