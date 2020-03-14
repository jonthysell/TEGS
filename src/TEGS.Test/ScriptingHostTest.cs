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

            bool[] expectedValues = VariableValueTest.ValidBooleanValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable() { Name = $"test{i}", Type = VariableValueType.Boolean };
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Boolean, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.BooleanValue);
                
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

            int[] expectedValues = VariableValueTest.ValidIntegerValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable() { Name = $"test{i}", Type = VariableValueType.Integer };
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Integer, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.IntegerValue);

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

            double[] expectedValues = VariableValueTest.ValidDoubleValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable() { Name = $"test{i}", Type = VariableValueType.Double };
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Double, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.DoubleValue);

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

            string[] expectedValues = VariableValueTest.ValidStringValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable() { Name = $"test{i}", Type = VariableValueType.String };
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.String, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.StringValue);

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

            StateVariable sv = new StateVariable() { Name = $"test", Type = VariableValueType.Boolean };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(bool))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(VariableValueType.String)));
        }

        [TestMethod]
        public void ScriptingHost_InvalidIntegerVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.Integer };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(int))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(bool))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidDoubleVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.Double };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(double))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(int))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidStringVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.String };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(VariableValueType.String)));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(double))));
        }

        #endregion

        #region Custom Function Tests

        [TestMethod]
        public void ScriptingHost_ValidNoParamsBooleanCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            foreach (var expectedValue in VariableValueTest.ValidBooleanValues)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValue);
                });

                host.DefineCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()");

                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsIntegerCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            foreach (var expectedValue in VariableValueTest.ValidIntegerValues)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValue);
                });

                host.DefineCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()");

                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsDoubleCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            foreach (var expectedValue in VariableValueTest.ValidDoubleValues)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValue);
                });

                host.DefineCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()");

                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidNoParamsStringCustomFunctionTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            foreach (var expectedValue in VariableValueTest.ValidStringValues)
            {
                CustomFunction function = new CustomFunction((args) =>
                {
                    return new VariableValue(expectedValue);
                });

                host.DefineCustomFunction("test", function);
                VariableValue actualValue = host.Evaluate("test()");

                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.StringValue);
            }
        }

        #endregion
    }
}
