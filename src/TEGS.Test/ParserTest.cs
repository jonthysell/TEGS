﻿// 
// ParserTest.cs
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

using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TEGS.Expressions;

namespace TEGS.Test
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void Parser_ValueTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("true", true),
                new ParseAndEvaluateTestCase("false", false),
                new ParseAndEvaluateTestCase("0", 0),
                new ParseAndEvaluateTestCase("0.0", 0.0),
                new ParseAndEvaluateTestCase("1", 1),
                new ParseAndEvaluateTestCase("1.0", 1.0),
                new ParseAndEvaluateTestCase("-1", -1),
                new ParseAndEvaluateTestCase("-1.0", -1.0),
                new ParseAndEvaluateTestCase(@"""""", ""),
                new ParseAndEvaluateTestCase(@""" """, " "),
                new ParseAndEvaluateTestCase(@"""test""", "test"),
                new ParseAndEvaluateTestCase(@"""\""""", "\""),
                new ParseAndEvaluateTestCase(@"""\r""", "\r"),
                new ParseAndEvaluateTestCase(@"""\n""", "\n"),
                new ParseAndEvaluateTestCase(@"""\t""", "\t"),
                new ParseAndEvaluateTestCase(@"""\r\n""", "\r\n"),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_AdditionTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 + 0", 0 + 0),
                new ParseAndEvaluateTestCase("0.0 + 0.0", 0.0 + 0.0),
                new ParseAndEvaluateTestCase("1 + 1", 1 + 1),
                new ParseAndEvaluateTestCase("1.0 + 1.0", 1.0 + 1.0),
                new ParseAndEvaluateTestCase("-1 + -1", -1 + -1),
                new ParseAndEvaluateTestCase("-1.0 + -1.0", -1.0 + -1.0),
                new ParseAndEvaluateTestCase(@"""""+""""", ""+""),
                new ParseAndEvaluateTestCase(@"""test""+""test""", "test"+"test"),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_SubtractionTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 - 0", 0 - 0),
                new ParseAndEvaluateTestCase("0.0 - 0.0", 0.0 - 0.0),
                new ParseAndEvaluateTestCase("1 - 1", 1 - 1),
                new ParseAndEvaluateTestCase("1.0 - 1.0", 1.0 - 1.0),
                new ParseAndEvaluateTestCase("-1 - -1", -1 - -1),
                new ParseAndEvaluateTestCase("-1.0 - -1.0", -1.0 - -1.0)
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_MultiplicationTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 * 0", 0 * 0),
                new ParseAndEvaluateTestCase("0.0 * 0.0", 0.0 * 0.0),
                new ParseAndEvaluateTestCase("1 * 1", 1 * 1),
                new ParseAndEvaluateTestCase("1.0 * 1.0", 1.0 * 1.0),
                new ParseAndEvaluateTestCase("-1 * -1", -1 * -1),
                new ParseAndEvaluateTestCase("-1.0 * -1.0", -1.0 * -1.0)
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_DivisionTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 / 1", 0 / 1),
                new ParseAndEvaluateTestCase("0.0 / 1.0", 0.0 / 1.0),
                new ParseAndEvaluateTestCase("1 / 1", 1 / 1),
                new ParseAndEvaluateTestCase("1.0 / 1.0", 1.0 / 1.0),
                new ParseAndEvaluateTestCase("-1 / -1", -1 / -1),
                new ParseAndEvaluateTestCase("-1.0 / -1.0", -1.0 / -1.0)
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_LessThanTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 < 0", 0 < 0),
                new ParseAndEvaluateTestCase("0 < 1", 0 < 1),
                new ParseAndEvaluateTestCase("1 < 0", 1 < 0),
                new ParseAndEvaluateTestCase("0 < -1", 0 < -1),
                new ParseAndEvaluateTestCase("-1 < 0", -1 < 0),
                new ParseAndEvaluateTestCase("0.0 < 0.0", 0.0 < 0.0),
                new ParseAndEvaluateTestCase("0.0 < 1.0", 0.0 < 1.0),
                new ParseAndEvaluateTestCase("1.0 < 0.0", 1.0 < 0.0),
                new ParseAndEvaluateTestCase("0.0 < -1.0", 0.0 < -1.0),
                new ParseAndEvaluateTestCase("-1.0 < 0.0", -1.0 < 0.0),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_GreaterThanTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 > 0", 0 > 0),
                new ParseAndEvaluateTestCase("0 > 1", 0 > 1),
                new ParseAndEvaluateTestCase("1 > 0", 1 > 0),
                new ParseAndEvaluateTestCase("0 > -1", 0 > -1),
                new ParseAndEvaluateTestCase("-1 > 0", -1 > 0),
                new ParseAndEvaluateTestCase("0.0 > 0.0", 0.0 > 0.0),
                new ParseAndEvaluateTestCase("0.0 > 1.0", 0.0 > 1.0),
                new ParseAndEvaluateTestCase("1.0 > 0.0", 1.0 > 0.0),
                new ParseAndEvaluateTestCase("0.0 > -1.0", 0.0 > -1.0),
                new ParseAndEvaluateTestCase("-1.0 > 0.0", -1.0 > 0.0),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_LessThanEqualsTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 <= 0", 0 <= 0),
                new ParseAndEvaluateTestCase("0 <= 1", 0 <= 1),
                new ParseAndEvaluateTestCase("1 <= 0", 1 <= 0),
                new ParseAndEvaluateTestCase("0 <= -1", 0 <= -1),
                new ParseAndEvaluateTestCase("-1 <= 0", -1 <= 0),
                new ParseAndEvaluateTestCase("0.0 <= 0.0", 0.0 <= 0.0),
                new ParseAndEvaluateTestCase("0.0 <= 1.0", 0.0 <= 1.0),
                new ParseAndEvaluateTestCase("1.0 <= 0.0", 1.0 <= 0.0),
                new ParseAndEvaluateTestCase("0.0 <= -1.0", 0.0 <= -1.0),
                new ParseAndEvaluateTestCase("-1.0 <= 0.0", -1.0 <= 0.0),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_GreaterThanEqualsTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("0 >= 0", 0 >= 0),
                new ParseAndEvaluateTestCase("0 >= 1", 0 >= 1),
                new ParseAndEvaluateTestCase("1 >= 0", 1 >= 0),
                new ParseAndEvaluateTestCase("0 >= -1", 0 >= -1),
                new ParseAndEvaluateTestCase("-1 >= 0", -1 >= 0),
                new ParseAndEvaluateTestCase("0.0 >= 0.0", 0.0 >= 0.0),
                new ParseAndEvaluateTestCase("0.0 >= 1.0", 0.0 >= 1.0),
                new ParseAndEvaluateTestCase("1.0 >= 0.0", 1.0 >= 0.0),
                new ParseAndEvaluateTestCase("0.0 >= -1.0", 0.0 >= -1.0),
                new ParseAndEvaluateTestCase("-1.0 >= 0.0", -1.0 >= 0.0),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_EqualsTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false == false", false == false),
                new ParseAndEvaluateTestCase("false == true", false == true),
                new ParseAndEvaluateTestCase("true == false", true == false),
                new ParseAndEvaluateTestCase("true == true", true == true),
                new ParseAndEvaluateTestCase("0 == 0", 0 == 0),
                new ParseAndEvaluateTestCase("0 == 1", 0 == 1),
                new ParseAndEvaluateTestCase("1 == 0", 1 == 0),
                new ParseAndEvaluateTestCase("0 == -1", 0 == -1),
                new ParseAndEvaluateTestCase("-1 == 0", -1 == 0),
                new ParseAndEvaluateTestCase("0.0 == 0.0", 0.0 == 0.0),
                new ParseAndEvaluateTestCase("0.0 == 1.0", 0.0 == 1.0),
                new ParseAndEvaluateTestCase("1.0 == 0.0", 1.0 == 0.0),
                new ParseAndEvaluateTestCase("0.0 == -1.0", 0.0 == -1.0),
                new ParseAndEvaluateTestCase("-1.0 == 0.0", -1.0 == 0.0),
                new ParseAndEvaluateTestCase("\"\" == \"\"", "" == ""),
                new ParseAndEvaluateTestCase("\"\" == \"test\"", "" == "test"),
                new ParseAndEvaluateTestCase("\"test\" == \"\"", "test" == ""),
                new ParseAndEvaluateTestCase("\"test\" == \"test\"", "test" == "test"),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_NotEqualsTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false != false", false != false),
                new ParseAndEvaluateTestCase("false != true", false != true),
                new ParseAndEvaluateTestCase("true != false", true != false),
                new ParseAndEvaluateTestCase("true != true", true != true),
                new ParseAndEvaluateTestCase("0 != 0", 0 != 0),
                new ParseAndEvaluateTestCase("0 != 1", 0 != 1),
                new ParseAndEvaluateTestCase("1 != 0", 1 != 0),
                new ParseAndEvaluateTestCase("0 != -1", 0 != -1),
                new ParseAndEvaluateTestCase("-1 != 0", -1 != 0),
                new ParseAndEvaluateTestCase("0.0 != 0.0", 0.0 != 0.0),
                new ParseAndEvaluateTestCase("0.0 != 1.0", 0.0 != 1.0),
                new ParseAndEvaluateTestCase("1.0 != 0.0", 1.0 != 0.0),
                new ParseAndEvaluateTestCase("0.0 != -1.0", 0.0 != -1.0),
                new ParseAndEvaluateTestCase("-1.0 != 0.0", -1.0 != 0.0),
                new ParseAndEvaluateTestCase("\"\" != \"\"", "" != ""),
                new ParseAndEvaluateTestCase("\"\" != \"test\"", "" != "test"),
                new ParseAndEvaluateTestCase("\"test\" != \"\"", "test" != ""),
                new ParseAndEvaluateTestCase("\"test\" != \"test\"", "test" != "test"),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_AndTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false & false", false & false),
                new ParseAndEvaluateTestCase("false & true", false & true),
                new ParseAndEvaluateTestCase("true & false", true & false),
                new ParseAndEvaluateTestCase("true & true", true & true),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_OrTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false | false", false | false),
                new ParseAndEvaluateTestCase("false | true", false | true),
                new ParseAndEvaluateTestCase("true | false", true | false),
                new ParseAndEvaluateTestCase("true | true", true | true),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_ConditionalAndTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false && false", false && false),
                new ParseAndEvaluateTestCase("false && true", false && true),
                new ParseAndEvaluateTestCase("true && false", true && false),
                new ParseAndEvaluateTestCase("true && true", true && true),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_ConditionalOrTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("false || false", false || false),
                new ParseAndEvaluateTestCase("false || true", false || true),
                new ParseAndEvaluateTestCase("true || false", true || false),
                new ParseAndEvaluateTestCase("true || true", true || true),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        [TestMethod]
        public void Parser_ComplicatedTest()
        {
            List<ParseAndEvaluateTestCase> tests = new List<ParseAndEvaluateTestCase>()
            {
                new ParseAndEvaluateTestCase("1+10*4-2*(16/4)/2/0.5+9", 1+10*4-2*(16/4)/2/0.5+9),
            };

            IContext context = new TestContext();

            ParseAndEvaluate_ValidTests(tests, context);
        }

        private void ParseAndEvaluate_ValidTests(IEnumerable<ParseAndEvaluateTestCase> tests, IContext context)
        {
            bool allPass = true;

            foreach (ParseAndEvaluateTestCase test in tests)
            {
                allPass = allPass && TryParseAndEvaluate_ValidTest(test, context);
            }

            Assert.IsTrue(allPass, "All tests did not pass.");
        }

        private bool TryParseAndEvaluate_ValidTest(ParseAndEvaluateTestCase test, IContext context)
        {
            Trace.Write($"ParseAndEvaluate \"{ test.Expression }\" => { test.ExpectedValue } : ");

            Node node = Parser.Parse(test.Expression);

            if (null == node)
            {
                Trace.WriteLine("FAIL, Node is null.");
                return false;
            }

            VariableValue actualValue = node.Evaluate(context);

            if (test.ExpectedValue != actualValue)
            {
                Trace.WriteLine($"FAIL, Wrong value: { actualValue }.");
                return false;
            }

            if (test.ExpectedValue.Type != actualValue.Type)
            {
                Trace.WriteLine($"FAIL, Wrong type: { actualValue.Type.ToString() }");
                return false;
            }

            Node reduced = node.Reduce();

            VariableValue reducedValue = reduced.Evaluate(context);

            if (test.ExpectedValue != reducedValue)
            {
                Trace.WriteLine($"FAIL, Wrong reduced value: { actualValue }.");
                return false;
            }

            if (test.ExpectedValue.Type != reducedValue.Type)
            {
                Trace.WriteLine($"FAIL, Wrong reduced type: { actualValue.Type.ToString() }");
                return false;
            }

            Trace.WriteLine("PASS.");
            return true;
        }

        private struct ParseAndEvaluateTestCase
        {
            public readonly string Expression;
            public readonly VariableValue ExpectedValue;

            public ParseAndEvaluateTestCase(string expression, bool expectedValue)
            {
                Expression = expression;
                ExpectedValue = new VariableValue(expectedValue);
            }

            public ParseAndEvaluateTestCase(string expression, int expectedValue)
            {
                Expression = expression;
                ExpectedValue = new VariableValue(expectedValue);
            }

            public ParseAndEvaluateTestCase(string expression, double expectedValue)
            {
                Expression = expression;
                ExpectedValue = new VariableValue(expectedValue);
            }

            public ParseAndEvaluateTestCase(string expression, string expectedValue)
            {
                Expression = expression;
                ExpectedValue = new VariableValue(expectedValue);
            }
        }
    }
}
