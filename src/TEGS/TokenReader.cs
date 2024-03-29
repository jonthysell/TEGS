﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

// Adapted from https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TEGS
{
    public class TokenReader
    {
        public TokenType CurrentToken { get; private set; }

        public VariableValue CurrentValue { get; private set; }

        public string CurrentSymbol { get; private set; }

        protected string Expression { get; private set; }

        protected int CurrentIndex { get; private set; }

        protected char CurrentChar { get; private set; }

        public TokenReader(string expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Reset();
        }

        public void Reset()
        {
            CurrentToken = TokenType.End;
            CurrentIndex = -1;
            CurrentChar = EndChar;
            CurrentValue = default;
            CurrentSymbol = default;
            ReadChar();
            ReadNext();
        }

        public void ReadNext()
        {
            SkipWhiteSpace();

            if (TryParseSpecial(out TokenType tokenType))
            {
                CurrentToken = tokenType;
                CurrentValue = default;
                CurrentSymbol = default;
            }
            else if (TryParseValue(out VariableValue value, out string literal))
            {
                CurrentToken = TokenType.Value;
                CurrentValue = value;
                CurrentSymbol = literal;
            }
            else if (TryParseSymbol(out string symbol))
            {
                CurrentToken = TokenType.Symbol;
                CurrentValue = default;
                CurrentSymbol = symbol;
            }
            else
            {
                throw new UnexpectedTokenException(this);
            }
        }

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                ReadChar();
            }
        }

        private bool TryParseSpecial(out TokenType result)
        {
            switch (CurrentChar)
            {
                case EndChar:
                    ReadChar();
                    result = TokenType.End;
                    return true;
                case '+':
                    ReadChar();
                    result = TokenType.Add;
                    return true;
                case '-':
                    ReadChar();
                    result = TokenType.Subtract;
                    return true;
                case '*':
                    ReadChar();
                    result = TokenType.Multiply;
                    return true;
                case '/':
                    ReadChar();
                    result = TokenType.Divide;
                    return true;
                case '(':
                    ReadChar();
                    result = TokenType.OpenParens;
                    return true;
                case ')':
                    ReadChar();
                    result = TokenType.CloseParens;
                    return true;
                case ',':
                    ReadChar();
                    result = TokenType.Comma;
                    return true;
                case '=':
                    ReadChar();
                    if (CurrentChar == '=')
                    {
                        ReadChar();
                        result = TokenType.Equals;
                    }
                    else
                    {
                        result = TokenType.Assign;
                    }
                    return true;
                case '<':
                    ReadChar();
                    if (CurrentChar == '=')
                    {
                        ReadChar();
                        result = TokenType.LessThanEquals;
                    }
                    else
                    {
                        result = TokenType.LessThan;
                    }
                    return true;
                case '>':
                    ReadChar();
                    if (CurrentChar == '=')
                    {
                        ReadChar();
                        result = TokenType.GreaterThanEquals;
                    }
                    else
                    {
                        result = TokenType.GreaterThan;
                    }
                    return true;
                case '!':
                    ReadChar();
                    if (CurrentChar == '=')
                    {
                        ReadChar();
                        result = TokenType.NotEquals;
                    }
                    else
                    {
                        result = TokenType.Not;
                    }
                    return true;
                case '&':
                    ReadChar();
                    if (CurrentChar == '&')
                    {
                        ReadChar();
                        result = TokenType.ConditionalAnd;
                    }
                    else
                    {
                        result = TokenType.And;
                    }
                    return true;
                case '|':
                    ReadChar();
                    if (CurrentChar == '|')
                    {
                        ReadChar();
                        result = TokenType.ConditionalOr;
                    }
                    else
                    {
                        result = TokenType.Or;
                    }
                    return true;
            }

            result = default;
            return false;
        }

        private bool TryParseValue(out VariableValue result, out string literal)
        {
            if (char.IsDigit(CurrentChar) || CurrentChar == '.') // Number
            {
                int startIndex = CurrentIndex;
                bool hasDecimalPoint = false;

                while (char.IsDigit(CurrentChar) || (!hasDecimalPoint && CurrentChar == '.'))
                {
                    hasDecimalPoint = hasDecimalPoint || CurrentChar == '.';
                    ReadChar();
                }

                string value = Expression[startIndex..CurrentIndex];

                result = hasDecimalPoint ? new VariableValue(double.Parse(value, CultureInfo.InvariantCulture)) : new VariableValue(int.Parse(value, CultureInfo.InvariantCulture));
                literal = value;
                return true;
            }
            else if (CurrentChar == 't' || CurrentChar == 'f') // Boolean
            {
                string remaining = Expression[CurrentIndex..];

                if (remaining.StartsWith("true"))
                {
                    result = VariableValue.True;
                    literal = "true";
                    ReadChar(literal.Length);
                    return true;
                }
                else if (remaining.StartsWith("false"))
                {
                    result = VariableValue.False;
                    literal = "false";
                    ReadChar(literal.Length);
                    return true;
                }
            }
            else if (CurrentChar == '"') // String
            {
                StringBuilder sb = new StringBuilder();

                int startIndex = CurrentIndex;

                bool foundClosingQuote = false;

                while (CurrentChar != EndChar)
                {
                    ReadChar();

                    if (CurrentChar == '"')
                    {
                        foundClosingQuote = true;
                        break;
                    }
                    else if (CurrentChar == '\\')
                    {
                        ReadChar();
                        if (EscapedChars.TryGetValue(CurrentChar, out char escapedChar))
                        {
                            sb.Append(escapedChar);
                        }
                    }
                    else
                    {
                        sb.Append(CurrentChar);
                    }
                }

                if (foundClosingQuote)
                {
                    ReadChar();

                    result = new VariableValue(sb.ToString());
                    literal = Expression[startIndex..CurrentIndex];
                    return true;
                }
            }

            result = default;
            literal = default;
            return false;
        }

        private bool TryParseSymbol(out string result)
        {
            if (char.IsLetter(CurrentChar) || CurrentChar == '_')
            {
                int startIndex = CurrentIndex;

                while (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '.')
                {
                    ReadChar();
                }

                result = Expression[startIndex..CurrentIndex];
                return true;
            }

            result = default;
            return false;
        }

        private void ReadChar(int chars = 1)
        {
            CurrentIndex += chars;
            CurrentChar = CurrentIndex < Expression.Length ? Expression[CurrentIndex] : EndChar;
        }

        protected const char EndChar = '\0';

        protected readonly static Dictionary<char, char> EscapedChars = new Dictionary<char, char>()
        {
            { '\\', '\\'},
            { '"', '"'},
            { 'r', '\r'},
            { 'n', '\n'},
            { 't', '\t'},
        };
    }

    #region Exceptions

    public class UnexpectedTokenException : Exception
    {
        public readonly TokenReader TokenReader;
        public UnexpectedTokenException(TokenReader tokenReader) : base()
        {
            TokenReader = tokenReader ?? throw new ArgumentNullException(nameof(tokenReader));
        }
    }

    #endregion

    public enum TokenType
    {
        End,
        Add,
        Subtract,
        Multiply,
        Divide,
        OpenParens,
        CloseParens,
        Comma,
        Assign,
        LessThan,
        GreaterThan,
        LessThanEquals,
        GreaterThanEquals,
        Not,
        Equals,
        NotEquals,
        And,
        Or,
        ConditionalAnd,
        ConditionalOr,
        Symbol,
        Value,
    }
}
