// 
// Parser.cs
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

namespace TEGS.Expressions
{
    public class Parser
    {
        public TokenReader Tokens { get; private set; }

        public Parser(TokenReader tokenReader)
        {
            Tokens = tokenReader ?? throw new ArgumentNullException(nameof(tokenReader));
        }

        public Node Parse()
        {
            if (!TryParseAssign(out Node n))
            {
                n = ParseOr();
            }

            if (Tokens.CurrentToken != TokenType.End)
            {
                throw new SyntaxException();
            }

            return n;
        }

        private bool TryParseAssign(out Node result)
        {
            if (Tokens.CurrentToken == TokenType.Symbol)
            {
                string name = Tokens.CurrentSymbol;

                Tokens.ReadNext();
                if (Tokens.CurrentToken == TokenType.Assign)
                {
                    Node lhs = new NodeVariable(name);

                    Tokens.ReadNext();
                    Node rhs = ParseOr();
                    result = new NodeAssign(lhs, rhs);
                    return true;
                }
            }

            Tokens.Reset();
            result = default(Node);
            return false;
        }

        private Node ParseOr() => ParseAnd();

        private Node ParseAnd() => ParseEqualsNotEquals();

        private Node ParseEqualsNotEquals()
        {
            Node lhs = ParseLessThanGreaterThan();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.Equals:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseLessThanGreaterThan();
                            lhs = new NodeEquals(lhs, rhs);
                            continue;
                        }
                    case TokenType.NotEquals:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseLessThanGreaterThan();
                            lhs = new NodeNotEquals(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

        private Node ParseLessThanGreaterThan()
        {
            Node lhs = ParseAddSubtract();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.LessThan:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAddSubtract();
                            lhs = new NodeLessThan(lhs, rhs);
                            continue;
                        }
                    case TokenType.GreaterThan:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAddSubtract();
                            lhs = new NodeGreaterThan(lhs, rhs);
                            continue;
                        }
                    case TokenType.LessThanEquals:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAddSubtract();
                            lhs = new NodeLessThanEquals(lhs, rhs);
                            continue;
                        }
                    case TokenType.GreaterThanEquals:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAddSubtract();
                            lhs = new NodeGreaterThanEquals(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

        private Node ParseAddSubtract()
        {
            Node lhs = ParseMultiplyDivide();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.Add:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseMultiplyDivide();
                            lhs = new NodeAddition(lhs, rhs);
                            continue;
                        }
                    case TokenType.Subtract:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseMultiplyDivide();
                            lhs = new NodeSubtraction(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

        private Node ParseMultiplyDivide()
        {
            Node lhs = ParseUnary();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.Multiply:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseUnary();
                            lhs = new NodeMultiplication(lhs, rhs);
                            continue;
                        }
                    case TokenType.Divide:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseUnary();
                            lhs = new NodeDivision(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

        private Node ParseUnary()
        {
            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.Add:
                        Tokens.ReadNext();
                        continue;
                    case TokenType.Subtract:
                        Tokens.ReadNext();
                        return new NodeNegative(ParseUnary());
                    case TokenType.Not:
                        Tokens.ReadNext();
                        return new NodeNot(ParseUnary());
                    default:
                        return ParseLeaf();
                }
            }
        }

        private Node ParseLeaf()
        {
            switch (Tokens.CurrentToken)
            {
                case TokenType.Value:
                    {
                        Node node = new NodeValue(Tokens.CurrentValue);
                        Tokens.ReadNext();
                        return node;
                    }
                case TokenType.OpenParens:
                    {
                        Tokens.ReadNext();
                        Node node = ParseOr();
                        if (Tokens.CurrentToken != TokenType.CloseParens)
                        {
                            throw new SyntaxException();
                        }
                        Tokens.ReadNext();
                        return node;
                    }
                case TokenType.Symbol:
                    {
                        string name = Tokens.CurrentSymbol;
                        Tokens.ReadNext();

                        if (Tokens.CurrentToken != TokenType.OpenParens)
                        {
                            return new NodeVariable(name);
                        }
                        else
                        {
                            Tokens.ReadNext();

                            if (Tokens.CurrentToken == TokenType.CloseParens)
                            {
                                Tokens.ReadNext();
                                return new NodeFunctionCall(name, null);
                            }

                            List<Node> arguments = new List<Node>();

                            while (true)
                            {
                                arguments.Add(ParseOr());

                                if (Tokens.CurrentToken == TokenType.Comma)
                                {
                                    Tokens.ReadNext();
                                    continue;
                                }

                                break;
                            }

                            if (Tokens.CurrentToken != TokenType.CloseParens)
                            {
                                throw new SyntaxException();
                            }

                            Tokens.ReadNext();
                            return new NodeFunctionCall(name, arguments.ToArray());
                        }
                    }
                default:
                    throw new SyntaxException();
            }
        }

        public static Node Parse(string expression)
        {
            Parser p = new Parser(new TokenReader(expression));
            return p.Parse();
        }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException() : base() { }
    }
}
