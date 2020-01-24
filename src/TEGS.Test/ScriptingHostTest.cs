// 
// ScriptingHostTest.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019, 2020 Jon Thysell <http://jonthysell.com>
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
    public class ScriptingHostTest
    {
        [TestMethod]
        public void ScriptingHost_NewTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);
        }

        #region Valid Variable Tests

        [TestMethod]
        public void ScriptingHost_ValidBooleanVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            bool[] expectedValues = ValidBooleans;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Boolean);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Boolean, defaultValue.Type);
                Assert.AreEqual(default(bool), defaultValue.BooleanValue);
                
                host.Assign(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidIntegerVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            int[] expectedValues = ValidIntegers;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Integer);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Integer, defaultValue.Type);
                Assert.AreEqual(default(int), defaultValue.IntegerValue);

                host.Assign(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.IntegerValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidDoubleVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            double[] expectedValues = ValidDoubles;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Double);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Double, defaultValue.Type);
                Assert.AreEqual(default(double), defaultValue.DoubleValue);

                host.Assign(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValues = host.Get(sv);
                Assert.AreEqual(VariableValueType.Double, actualValues.Type);
                Assert.AreEqual(expectedValues[i], actualValues.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidStringVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            string[] expectedValues = ValidStrings;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.String);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.String, defaultValue.Type);
                Assert.AreEqual(default(string), defaultValue.StringValue);

                host.Assign(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.StringValue);
            }
        }

        #endregion

        #region Invalid Variable Tests

        [TestMethod]
        public void ScriptingHost_InvalidBooleanVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Boolean);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(bool))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(string))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidIntegerVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Integer);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(int))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(bool))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidDoubleVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Double);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(double))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(int))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidStringVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.String);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(string))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(double))));
        }

        #endregion

        #region Custom Function Tests

        [TestMethod]
        public void ScriptingHost_ValidNoParamsBooleanCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            bool[] expectedValues = ValidBooleans;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValues[i]);
                });

                host.AssignCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()", VariableValueType.Boolean);

                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsIntegerCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            int[] expectedValues = ValidIntegers;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValues[i]);
                });

                host.AssignCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()", VariableValueType.Integer);

                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.IntegerValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsDoubleCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            double[] expectedValues = ValidDoubles;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValues[i]);
                });

                host.AssignCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()", VariableValueType.Double);

                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsStringCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            string[] expectedValues = ValidStrings;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValues[i]);
                });

                host.AssignCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()", VariableValueType.String);

                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.StringValue);
            }
        }

        #endregion

        protected readonly static bool[] ValidBooleans = { true, false };

        protected readonly static int[] ValidIntegers = { 0, 1, int.MinValue, int.MaxValue };

        protected readonly static double[] ValidDoubles = { 0, 1, double.MinValue, double.MinValue };

        protected readonly static string[] ValidStrings = { null, "", "test" };
    }
}
