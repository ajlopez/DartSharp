namespace DartSharp.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    public class Lexer
    {
        private static string[] operators = { "=", "==", "+", "-", "*", "/", "<", ">", "<=", ">=" };
        private static string separators = ";(),{}.[]";

        private Stack<int> characters = new Stack<int>();

        private TextReader reader;

        public Lexer(string text)
            : this(new StringReader(text))
        {
        }

        public Lexer(TextReader reader)
        {
            this.reader = reader;
        }

        public Token NextToken()
        {
            int ich = this.NextChar();

            while (ich != -1 && char.IsWhiteSpace((char)ich))
                ich = this.NextChar();

            if (ich == -1)
                return null;

            char ch = (char)ich;

            if (ch == '"' || ch == '\'')
                return this.NextString(ch);

            if (char.IsLetter(ch))
                return this.NextName(ch);

            if (char.IsDigit(ch))
                return this.NextInteger(ch);

            if (separators.Contains(ch))
                return new Token(ch.ToString(), TokenType.Separator);

            if (operators.Any(op => op[0] == ch))
                return this.NextOperator(ch);

            throw new LexerException(string.Format("Unexpected '{0}'", ch));
        }

        private Token NextName(char ch)
        {
            string value = ch.ToString();
            int ich;

            for (ich = this.NextChar(); ich != -1 && char.IsLetterOrDigit((char)ich); ich = this.reader.Read())
                value += (char)ich;

            if (ich != -1)
                this.PushChar(ich);

            return new Token(value, TokenType.Name);
        }

        private Token NextString(char delimeter)
        {
            string value = "";
            int ich;

            for (ich = this.NextChar(); ich != -1 && ((char)ich) != delimeter; ich = this.reader.Read())
                value += (char)ich;

            if (ich == -1)
                throw new LexerException("Unclosed String");

            return new Token(value, TokenType.String);
        }

        private Token NextInteger(char ch)
        {
            string value = ch.ToString();
            int ich;

            for (ich = this.NextChar(); ich != -1 && char.IsDigit((char)ich); ich = this.reader.Read())
                value += (char)ich;

            if (ich != -1)
                this.PushChar(ich);

            return new Token(value, TokenType.Integer);
        }

        private Token NextOperator(char ch)
        {
            // TODO improve/fix algorithm, multicharacter operators
            string value = ch.ToString();
            int ich = this.NextChar();

            if (ich != -1)
            {
                string value2 = value + (char)ich;

                if (operators.Contains(value2))
                    return new Token(value2, TokenType.Operator);

                this.PushChar(ich);
            }

            return new Token(ch.ToString(), TokenType.Operator);
        }

        private int NextChar()
        {
            int ich = this.NextSimpleChar();

            if (ich < 0)
                return ich;

            char ch = (char)ich;

            if (ch != '/')
                return ich;

            int ich2 = this.NextSimpleChar();

            if (ich2 < 0)
            {
                this.PushChar(ich2);
                return ich;
            }

            if ((char)ich2 == '/')
            {
                for (ich = this.NextSimpleChar(); ich >= 0 && ((char)ich) != '\r' && ((char)ich) != '\n'; ich = this.NextSimpleChar())
                    ;
            }
            else if ((char)ich2 == '*')
            {
                while (true)
                {
                    for (ich = this.NextSimpleChar(); ich >= 0 && ((char)ich) != '*'; ich = this.NextSimpleChar())
                        ;

                    if (ich < 0)
                        throw new LexerException("Unexpected End of Input");

                    ich = this.NextSimpleChar();

                    if (ich < 0)
                        throw new LexerException("Unexpected End of Input");

                    // TODO avoid recursion
                    if ((char)ich == '/')
                        return this.NextChar();
                }
            }
            else
                this.PushChar(ich2);

            return ich;
        }

        private int NextSimpleChar()
        {
            if (this.characters.Count > 0)
                return this.characters.Pop();

            return this.reader.Read();
        }

        private void PushChar(int ich)
        {
            this.characters.Push(ich);
        }
    }
}
