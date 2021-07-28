// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

// Adapted from https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

using System;
using System.Collections.Generic;

namespace TEGS
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
            result = default;
            return false;
        }

        private Node ParseOr()
        {
            Node lhs = ParseAnd();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.Or:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAnd();
                            lhs = new NodeOr(lhs, rhs);
                            continue;
                        }
                    case TokenType.ConditionalOr:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseAnd();
                            lhs = new NodeConditionalOr(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

        private Node ParseAnd()
        {
            Node lhs = ParseEqualsNotEquals();

            while (true)
            {
                switch (Tokens.CurrentToken)
                {
                    case TokenType.And:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseEqualsNotEquals();
                            lhs = new NodeAnd(lhs, rhs);
                            continue;
                        }
                    case TokenType.ConditionalAnd:
                        {
                            Tokens.ReadNext();
                            Node rhs = ParseEqualsNotEquals();
                            lhs = new NodeConditionalAnd(lhs, rhs);
                            continue;
                        }
                    default:
                        return lhs;
                }
            }
        }

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
