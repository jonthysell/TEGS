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
        public void BaseLibraries_SystemBoolTest()
        {
            var lib = BaseLibraries.SystemBool;

            VariableValue actual;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(bool.FalseString), out actual));
            Assert.AreEqual(VariableValueType.String, actual.Type);
            Assert.AreEqual(bool.FalseString, actual.StringValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(bool.TrueString), out actual));
            Assert.AreEqual(VariableValueType.String, actual.Type);
            Assert.AreEqual(bool.TrueString, actual.StringValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemIntegerTest()
        {
            var lib = BaseLibraries.SystemInteger;

            VariableValue actual;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(int.MaxValue), out actual));
            Assert.AreEqual(VariableValueType.Integer, actual.Type);
            Assert.AreEqual(int.MaxValue, actual.IntegerValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(int.MinValue), out actual));
            Assert.AreEqual(VariableValueType.Integer, actual.Type);
            Assert.AreEqual(int.MinValue, actual.IntegerValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemDoubleTest()
        {
            var lib = BaseLibraries.SystemDouble;

            VariableValue actual;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.Epsilon), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.Epsilon, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.MaxValue), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.MaxValue, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.MinValue), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.MinValue, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.NaN), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.NaN, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.NegativeInfinity), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.NegativeInfinity, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(double.PositiveInfinity), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(double.PositiveInfinity, actual.DoubleValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemStringTest()
        {
            var lib = BaseLibraries.SystemString;

            VariableValue actual;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(string.Empty), out actual));
            Assert.AreEqual(VariableValueType.String, actual.Type);
            Assert.AreEqual(string.Empty, actual.StringValue);
        }

        [TestMethod]
        public void BaseLibraries_SystemMathTest()
        {
            var lib = BaseLibraries.SystemMath;

            VariableValue actual;

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.E), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(Math.E, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.PI), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(Math.PI, actual.DoubleValue);

            Assert.IsTrue(lib.Constants.TryGetValue(nameof(Math.Tau), out actual));
            Assert.AreEqual(VariableValueType.Double, actual.Type);
            Assert.AreEqual(Math.Tau, actual.DoubleValue);
        }
    }
}
