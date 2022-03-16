// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class BaseLibrariesTest
    {
        [TestMethod]
        public void BaseLibraries_SystemBooleanTest()
        {
            var lib = BaseLibraries.SystemBoolean;

            VariableValue constantValue;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(bool.FalseString), out constantValue));
            Assert.AreEqual(VariableValueType.String, constantValue.Type);
            Assert.AreEqual(bool.FalseString, constantValue.StringValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(bool.TrueString), out constantValue));
            Assert.AreEqual(VariableValueType.String, constantValue.Type);
            Assert.AreEqual(bool.TrueString, constantValue.StringValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemIntegerTest()
        {
            var lib = BaseLibraries.SystemInteger;

            VariableValue constantValue;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(int.MaxValue), out constantValue));
            Assert.AreEqual(VariableValueType.Integer, constantValue.Type);
            Assert.AreEqual(int.MaxValue, constantValue.IntegerValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(int.MinValue), out constantValue));
            Assert.AreEqual(VariableValueType.Integer, constantValue.Type);
            Assert.AreEqual(int.MinValue, constantValue.IntegerValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemDoubleTest()
        {
            var lib = BaseLibraries.SystemDouble;

            VariableValue constantValue;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.Epsilon), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.Epsilon, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.MaxValue), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.MaxValue, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.MinValue), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.MinValue, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.NaN), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.NaN, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.NegativeInfinity), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.NegativeInfinity, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.PositiveInfinity), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(double.PositiveInfinity, constantValue.DoubleValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemStringTest()
        {
            var lib = BaseLibraries.SystemString;

            VariableValue constantValue;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(string.Empty), out constantValue));
            Assert.AreEqual(VariableValueType.String, constantValue.Type);
            Assert.AreEqual(string.Empty, constantValue.StringValue);
        }

        [TestMethod]
        public void BaseLibraries_StringLibraryTest()
        {
            var lib = BaseLibraries.StringLibrary;

            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(StringLibrary.Length), out customFunction));
            foreach (var value in VariableValueTest.ValidStringValues)
            {
                var actual = customFunction(new[] { new VariableValue(value) });
                Assert.AreEqual(VariableValueType.Integer, actual.Type);
                Assert.AreEqual(value.Length, actual.IntegerValue);
            }
        }

        [TestMethod]
        public void BaseLibraries_ConvertLibraryTest()
        {
            var lib = BaseLibraries.ConvertLibrary;

            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToBoolean), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, bool.FalseString, bool.TrueString })
            {
                var expected = Convert.ToBoolean(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Boolean, actual.Type);
                Assert.AreEqual(expected, actual.BooleanValue);
            }

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToInteger), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1" })
            {
                var expected = Convert.ToInt32(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Integer, actual.Type);
                Assert.AreEqual(expected, actual.IntegerValue);
            }

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToDouble), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1", "-1.0", "0.0", "1.0" })
            {
                var expected = Convert.ToDouble(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Double, actual.Type);
                Assert.AreEqual(expected, actual.DoubleValue);
            }

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToString), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1", "-1.0", "0.0", "1.0", bool.FalseString, bool.TrueString })
            {
                var expected = Convert.ToString(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.String, actual.Type);
                Assert.AreEqual(expected, actual.StringValue);
            }
        }

        [TestMethod]
        public void BaseLibraries_SystemMathTest()
        {
            var lib = BaseLibraries.SystemMath;

            VariableValue constantValue;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.E), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(Math.E, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.PI), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(Math.PI, constantValue.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.Tau), out constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(Math.Tau, constantValue.DoubleValue);
        }
    }
}
