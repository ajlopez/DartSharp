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
            IExpression expression = this.ParseSimpleExpression();

            if (expression == null)
                return null;

            Token token = this.NextToken();

            while (token != null && token.Type == TokenType.Operator && (token.Value == "+" || token.Value == "-"))
            {
                ArithmeticOperator oper = (token.Value == "+" ? ArithmeticOperator.Add : ArithmeticOperator.Subtract);
                expression = new ArithmeticBinaryExpression(oper, expression, this.ParseSimpleExpression());
                token = this.NextToken();
            }

            if (token != null)
                this.PushToken(token);

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
                        this.PushToken(token);
                        return new CallExpression(new VariableExpression(name), this.ParseArguments());
                    }

                    IExpression expr = new VariableExpression(name);

                    if (token != null)
                        this.PushToken(token);

                    return expr;

                case TokenType.String:
                    return new ConstantExpression(token.Value);

                case TokenType.Separator:
                    if (token.Value == "(")
                    {
                        var result = this.ParseExpression();
                        this.ParseToken(")", TokenType.Separator);
                        return result;
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

                if (this.IsType(token.Value))
                {
                    string name = this.ParseName();

                    if (this.TryPeekToken("(", TokenType.Separator))
                        return this.ParseDefineFunction(name);

                    return this.ParseDefineVariableCommand(name);
                }
            }

            this.PushToken(token);

            IExpression expression = this.ParseExpression();

            if (expression == null)
                return null;

            token = this.NextToken();

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

        private DefineFunctionCommand ParseDefineFunction(string name)
        {
            IList<string> argnames = this.ParseArgumentNames();
            ICommand body = this.ParseCommand();
            return new DefineFunctionCommand(name, argnames, body);
        }

        private ICommand ParseDefineVariableCommand(string name)
        {
            DefineVariableCommand command;

            if (this.TryParseToken("=", TokenType.Operator))
                command = new DefineVariableCommand(name, this.ParseExpression());
            else
                command =  new DefineVariableCommand(name);

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

        private IEnumerable<IExpression> ParseArguments()
        {
            IList<IExpression> arguments = new List<IExpression>();

            this.ParseToken("(", TokenType.Separator);

            while (!this.TryParseToken(")", TokenType.Separator))
            {
                if (arguments.Count > 0)
                    this.ParseToken(",", TokenType.Separator);
                arguments.Add(this.ParseExpression());
            }

            return arguments;
        }

        private IList<string> ParseArgumentNames()
        {
            this.ParseToken("(", TokenType.Separator);

            IList<string> arguments = new List<string>();

            Token token = this.NextToken();

            while (token != null && token.Type == TokenType.Name)
            {
                if (!this.IsType(token.Value))
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

        private bool IsType(string name)
        {
            return typenames.Contains(name);
        }
    }
}
