// 
// TokenReaderTest.cs
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

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TEGS.Expressions;

namespace TEGS.Test
{
    [TestClass]
    public class TokenReaderTest
    {
        [TestMethod]
        public void TokenReader_EmptyValidTest() => TokenReader_ValidTest("", new [] { TokenType.End });

        [TestMethod]
        public void TokenReader_WhiteSpaceValidTest() => TokenReader_ValidTest(" ", new [] { TokenType.End });

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TokenReader_NullInvalidTest() => new TokenReader(null);

        [TestMethod]
        public void TokenReader_SpecialValidTest()
        {
            List<TokenizeTestCase> tests = new List<TokenizeTestCase>()
            {
                new TokenizeTestCase("+", new [] { TokenType.Add, TokenType.End }),
                new TokenizeTestCase("-", new [] { TokenType.Subtract, TokenType.End }),
                new TokenizeTestCase("*", new [] { TokenType.Multiply, TokenType.End }),
                new TokenizeTestCase("/", new [] { TokenType.Divide, TokenType.End }),
                new TokenizeTestCase("(", new [] { TokenType.OpenParens, TokenType.End }),
                new TokenizeTestCase(")", new [] { TokenType.CloseParens, TokenType.End }),
                new TokenizeTestCase(",", new [] { TokenType.Comma, TokenType.End }),
                new TokenizeTestCase("=", new [] { TokenType.Assign, TokenType.End }),
                new TokenizeTestCase("<", new [] { TokenType.LessThan, TokenType.End }),
                new TokenizeTestCase(">", new [] { TokenType.GreaterThan, TokenType.End }),
                new TokenizeTestCase("<=", new [] { TokenType.LessThanEquals, TokenType.End }),
                new TokenizeTestCase(">=", new [] { TokenType.GreaterThanEquals, TokenType.End }),
                new TokenizeTestCase("!", new [] { TokenType.Not, TokenType.End }),
                new TokenizeTestCase("==", new [] { TokenType.Equals, TokenType.End }),
                new TokenizeTestCase("!=", new [] { TokenType.NotEquals, TokenType.End }),
                new TokenizeTestCase("&", new [] { TokenType.And, TokenType.End }),
                new TokenizeTestCase("|", new [] { TokenType.Or, TokenType.End }),
                new TokenizeTestCase("&&", new [] { TokenType.ConditionalAnd, TokenType.End }),
                new TokenizeTestCase("||", new [] { TokenType.ConditionalOr, TokenType.End }),
            };

            TokenReader_ValidTest(tests);
        }

        [TestMethod]
        public void TokenReader_ValueValidTest()
        {
            List<TokenizeTestCase> tests = new List<TokenizeTestCase>()
            {
                new TokenizeTestCase("false", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase("true", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase("0", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase("1", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase("0.0", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase("1.0", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(".0", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@""" """, new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""test""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""\""""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""\r""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""\n""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""\t""", new [] { TokenType.Value, TokenType.End }),
                new TokenizeTestCase(@"""\r\n""", new [] { TokenType.Value, TokenType.End }),
            };

            TokenReader_ValidTest(tests);
        }

        [TestMethod]
        public void TokenReader_SymbolValidTest()
        {
            List<TokenizeTestCase> tests = new List<TokenizeTestCase>()
            {
                new TokenizeTestCase("test", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("test1", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("_test", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("_test1", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("test.test", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("test1.test1", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("_test._test", new [] { TokenType.Symbol, TokenType.End }),
                new TokenizeTestCase("_test1._test1", new [] { TokenType.Symbol, TokenType.End }),
            };

            TokenReader_ValidTest(tests);
        }

        private void TokenReader_ValidTest(string expression, IEnumerable<TokenType> expectedTokens)
        {
            TokenReader_ValidTest(new [] { new TokenizeTestCase(expression, expectedTokens) });
        }

        private void TokenReader_ValidTest(IEnumerable<TokenizeTestCase> tests)
        {
            bool allPass = true;

            foreach (var test in tests)
            {
                allPass = allPass && TryTokenizeTestCase(test);
            }

            Assert.IsTrue(allPass, "All tests did not pass.");
        }

        private bool TryTokenizeTestCase(TokenizeTestCase test)
        {
            Trace.Write($"Tokenize \"{ test.Expression }\": ");

            TokenReader tr = new TokenReader(test.Expression);

            if (null == tr)
            {
                Trace.WriteLine("FAIL, TokenReader is null.");
                return false;
            }

            foreach (TokenType expectedToken in test.ExpectedTokens)
            {
                var actualToken = tr.CurrentToken;

                if (expectedToken != actualToken)
                {
                    Trace.WriteLine($"FAIL, Expected: {expectedToken}, actual: { actualToken }.");
                    return false;
                }
                tr.ReadNext();
            }

            Trace.WriteLine("PASS.");
            return true;
        }

        private struct TokenizeTestCase
        {
            public readonly string Expression;
            public readonly IEnumerable<TokenType> ExpectedTokens;

            public TokenizeTestCase(string expression, IEnumerable<TokenType> expectedTokens)
            {
                Expression = expression;
                ExpectedTokens = expectedTokens;
            }
        }
    }
}
