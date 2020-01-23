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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TEGS.Expressions;

namespace TEGS.Test
{
    [TestClass]
    public class TokenReaderTest
    {
        [TestMethod]
        public void TokenReader_EmptyValidTest() => TokenReader_ValidTest("", new TokenType[] { TokenType.End });

        [TestMethod]
        public void TokenReader_WhiteSpaceValidTest() => TokenReader_ValidTest(" ", new TokenType[] { TokenType.End });

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TokenReader_NullInvalidTest()
        {
            TokenReader tr = new TokenReader(null);
        }

        private void TokenReader_ValidTest(string expression, IEnumerable<TokenType> expectedTokens)
        {
            TokenReader tr = new TokenReader(expression);
            Assert.IsNotNull(tr);

            foreach (TokenType expectedToken in expectedTokens)
            {
                Assert.AreEqual(expectedToken, tr.CurrentToken);
                tr.ReadNext();
            }
        }
    }
}
