// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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

        #region Valid Constant Tests

        [TestMethod]
        public void ScriptingHost_ValidBooleanConstantTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            bool[] expectedValues = VariableValueTest.ValidBooleanValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                string name = $"test{i}";
                host.DefineConstant(name, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetValue(name);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.BooleanValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidIntegerConstantTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            int[] expectedValues = VariableValueTest.ValidIntegerValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                string name = $"test{i}";
                host.DefineConstant(name, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetValue(name);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.IntegerValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidDoubleConstantTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            double[] expectedValues = VariableValueTest.ValidDoubleValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                string name = $"test{i}";
                host.DefineConstant(name, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetValue(name);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidStringConstantTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            string[] expectedValues = VariableValueTest.ValidStringValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                string name = $"test{i}";
                host.DefineConstant(name, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetValue(name);
                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.StringValue);
            }
        }

        #endregion

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

                VariableValue defaultValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Boolean, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.BooleanValue);
                
                host.AssignVariable(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetVariable(sv);
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

                VariableValue defaultValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Integer, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.IntegerValue);

                host.AssignVariable(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetVariable(sv);
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

                VariableValue defaultValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Double, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.DoubleValue);

                host.AssignVariable(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValues = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Double, actualValues.Type);
                Assert.AreEqual(expectedValues[i], actualValues.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidIntegerToDoubleTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            int[] expectedValues = VariableValueTest.ValidIntegerValues;

            for (int i = 0; i < expectedValues.Length; i++)
            {
                StateVariable sv = new StateVariable() { Name = $"test{i}", Type = VariableValueType.Double };
                host.Create(sv);

                VariableValue defaultValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Double, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.DoubleValue);

                host.AssignVariable(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValues[i], actualValue.DoubleValue);
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

                VariableValue defaultValue = host.GetVariable(sv);
                Assert.AreEqual(VariableValueType.String, defaultValue.Type);
                Assert.AreEqual(default, defaultValue.StringValue);

                host.AssignVariable(sv, new VariableValue(expectedValues[i]));

                VariableValue actualValue = host.GetVariable(sv);
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

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.GetVariable(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.AssignVariable(sv, new VariableValue(default(bool))));

            host.Create(sv);
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.AssignVariable(sv, new VariableValue(VariableValueType.String)));
        }

        [TestMethod]
        public void ScriptingHost_InvalidIntegerVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.Integer };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.GetVariable(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.AssignVariable(sv, new VariableValue(default(int))));

            host.Create(sv);
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.AssignVariable(sv, new VariableValue(default(bool))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidDoubleVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.Double };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.GetVariable(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.AssignVariable(sv, new VariableValue(default(double))));

            host.Create(sv);
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.AssignVariable(sv, new VariableValue(default(bool))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidStringVariableTest()
        {
            ScriptingHost host = new ScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable { Name = $"test", Type = VariableValueType.String };

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.GetVariable(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.AssignVariable(sv, new VariableValue(VariableValueType.String)));

            host.Create(sv);
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.AssignVariable(sv, new VariableValue(default(double))));
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
