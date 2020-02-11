// 
// VariableValueTest.cs
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class VariableValueTest
    {
        [TestMethod]
        public void VariableValue_NewBooleanTest()
        {
            foreach (var expectedValue in ValidBooleanValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsBoolean);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void VariableValue_NewIntegerTest()
        {
            foreach (var expectedValue in ValidIntegerValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }
        }

        [TestMethod]
        public void VariableValue_NewDoubleTest()
        {
            foreach (var expectedValue in ValidDoubleValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }
        }

        [TestMethod]
        public void VariableValue_NewStringTest()
        {
            foreach (var expectedValue in ValidStringValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsString);
                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.StringValue);
            }
        }

        [TestMethod]
        public void VariableValue_ParseBooleanTest()
        {
            foreach (var expectedValue in ValidBooleanValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsBoolean);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void VariableValue_ParseIntegerTest()
        {
            foreach (var expectedValue in ValidIntegerValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }
        }

        [TestMethod]
        public void VariableValue_ParseDoubleTest()
        {
            foreach (var expectedValue in ValidDoubleValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }
        }

        [TestMethod]
        public void VariableValue_ParseStringTest()
        {
            foreach (var expectedValue in ValidStringValues)
            {
                if (null != expectedValue)
                {
                    var actualValue = VariableValue.Parse(expectedValue);
                    Assert.IsTrue(actualValue.IsString);
                    Assert.AreEqual(VariableValueType.String, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.StringValue);
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorTrueFalseTest()
        {
            if (!VariableValue.True)
            {
                Assert.Fail("VariableValue.True does not evaluate as true.");
            }

            if (VariableValue.False)
            {
                Assert.Fail("VariableValue.False does not evaluate as false.");
            }
        }

        public static readonly bool[] ValidBooleanValues = new bool[] { false, true };

        public static readonly int[] ValidIntegerValues = new int[] { int.MinValue, -1, 0, 1, int.MaxValue };

        public static readonly double[] ValidDoubleValues = new double[] { double.NegativeInfinity, double.MinValue, -1.0, 0.0, double.Epsilon, 1.0, double.MaxValue, double.PositiveInfinity };

        public static readonly string[] ValidStringValues = new string[] { null, "", "test" };
    }
}
