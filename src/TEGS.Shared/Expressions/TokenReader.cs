// 
// Tokenizer.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted from https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TEGS.Expressions
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
            else if (TryParseValue(out VariableValue value))
            {
                CurrentToken = TokenType.Value;
                CurrentValue = value;
                CurrentSymbol = default;
            }
            else if (TryParseSymbol(out string symbol))
            {
                CurrentToken = TokenType.Symbol;
                CurrentValue = default;
                CurrentSymbol = symbol;
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

        private bool TryParseValue(out VariableValue result)
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

                string value = Expression.Substring(startIndex, CurrentIndex - startIndex);

                result = hasDecimalPoint ? new VariableValue(double.Parse(value, CultureInfo.InvariantCulture)) : new VariableValue(int.Parse(value, CultureInfo.InvariantCulture));
                return true;
            }
            else if (CurrentChar == 't' || CurrentChar == 'f') // Boolean
            {
                string remaining = Expression.Substring(CurrentIndex);

                if (remaining.StartsWith("true"))
                {
                    result = VariableValue.True;
                    ReadChar("true".Length);
                    return true;
                }
                else if (remaining.StartsWith("false"))
                {
                    result = VariableValue.False;
                    ReadChar("false".Length);
                    return true;
                }
            }
            else if (CurrentChar == '"') // String
            {
                StringBuilder sb = new StringBuilder();

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
                    return true;
                }
            }

            result = default;
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

                result = Expression.Substring(startIndex, CurrentIndex - startIndex);
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
        public UnexpectedTokenException() : base() { }
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
