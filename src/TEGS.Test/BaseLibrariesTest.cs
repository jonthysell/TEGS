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

            NumericFunctionTest(lib, Math.Abs, Math.Abs, nameof(Math.Abs));
            NumericFunctionTest(lib, Math.Acos, nameof(Math.Acos));
            NumericFunctionTest(lib, Math.Acosh, nameof(Math.Acosh));
            NumericFunctionTest(lib, Math.Asin, nameof(Math.Asin));
            NumericFunctionTest(lib, Math.Asinh, nameof(Math.Asinh));
            NumericFunctionTest(lib, Math.Atan, nameof(Math.Atan));
            NumericFunctionTest(lib, Math.Atanh, nameof(Math.Atanh));
            NumericFunctionTest(lib, Math.Atan2, nameof(Math.Atan2));
            NumericFunctionTest(lib, Math.BitDecrement, nameof(Math.BitDecrement));
            NumericFunctionTest(lib, Math.BitIncrement, nameof(Math.BitIncrement));
            NumericFunctionTest(lib, Math.Cbrt, nameof(Math.Cbrt));
            NumericFunctionTest(lib, Math.Ceiling, nameof(Math.Ceiling));
            NumericFunctionTest(lib, Math.Clamp, Math.Clamp, nameof(Math.Clamp));
            NumericFunctionTest(lib, Math.CopySign, nameof(Math.CopySign));
            NumericFunctionTest(lib, Math.Cos, nameof(Math.Cos));
            NumericFunctionTest(lib, Math.Cosh, nameof(Math.Cosh));
            NumericFunctionTest(lib, Math.Exp, nameof(Math.Exp));
            NumericFunctionTest(lib, Math.Floor, nameof(Math.Floor));
            NumericFunctionTest(lib, Math.FusedMultiplyAdd, nameof(Math.FusedMultiplyAdd));
            NumericFunctionTest(lib, Math.IEEERemainder, nameof(Math.IEEERemainder));
            NumericFunctionTest(lib, (a) => Math.Log(a), nameof(Math.Log));
            NumericFunctionTest(lib, Math.Log10, nameof(Math.Log10));
            NumericFunctionTest(lib, Math.Log2, nameof(Math.Log2));
            NumericFunctionTest(lib, Math.Max, Math.Max, nameof(Math.Max));
            NumericFunctionTest(lib, Math.MaxMagnitude, nameof(Math.MaxMagnitude));
            NumericFunctionTest(lib, Math.Min, Math.Min, nameof(Math.Min));
            NumericFunctionTest(lib, Math.MinMagnitude, nameof(Math.MinMagnitude));
            NumericFunctionTest(lib, Math.Pow, nameof(Math.Pow));
            NumericFunctionTest(lib, Math.ReciprocalEstimate, nameof(Math.ReciprocalEstimate));
            NumericFunctionTest(lib, Math.ReciprocalSqrtEstimate, nameof(Math.ReciprocalSqrtEstimate));
            NumericFunctionTest(lib, Math.Round, nameof(Math.Round));
            NumericFunctionTest(lib, Math.Sign, nameof(Math.Sign));
            NumericFunctionTest(lib, Math.Sin, nameof(Math.Sin));
            NumericFunctionTest(lib, Math.Sinh, nameof(Math.Sinh));
            NumericFunctionTest(lib, Math.Sqrt, nameof(Math.Sqrt));
            NumericFunctionTest(lib, Math.Tan, nameof(Math.Tan));
            NumericFunctionTest(lib, Math.Tanh, nameof(Math.Tanh));
            NumericFunctionTest(lib, Math.Truncate, nameof(Math.Truncate));
        }

        [TestMethod]
        public void BaseLibraries_RandomVariateLibraryTest()
        {
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).UniformVariate, nameof(RandomExtensions.UniformVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).ExponentialVariate, nameof(RandomExtensions.ExponentialVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).NormalVariate, nameof(RandomExtensions.NormalVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).LogNormalVariate, nameof(RandomExtensions.LogNormalVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).TriangularVariate, nameof(RandomExtensions.TriangularVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).GammaVariate, nameof(RandomExtensions.GammaVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), new Random(12345).BetaVariate, nameof(RandomExtensions.BetaVariate));

            // ErlangVariate has a unique signature, not worth creating a NumericFunctionTest override yet
            CustomFunction customFunction;
            var lib = BaseLibraries.RandomVariateLibrary(12345);
            Assert.IsTrue(lib.Functions.TryGetValue(nameof(RandomExtensions.ErlangVariate), out customFunction), $"Function { nameof(RandomExtensions.ErlangVariate) } not found.");

            var function = new Random(12345).ErlangVariate;
            for (int value1 = -3; value1 < 3; value1++)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    VerifyReturnValue(() => function(value1, value2), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2) }));
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    VerifyReturnValue(() => function(value1, value2), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2) }));
                }
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int> intFunction, Func<double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, intFunction, functionName);
            NumericFunctionTest(lib, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int> function, string functionName)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value in VariableValueTest.SimpleIntValues)
            {
                VerifyReturnValue(() => function(value), () => customFunction(new[] { VariableValue.Parse(value) }));
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<double, double> function, string functionName, bool testIntToDouble = true)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value in VariableValueTest.SimpleDoubleValues)
            {
                VerifyReturnValue(() => function(value), () => customFunction(new[] { VariableValue.Parse(value) }));
            }

            if (testIntToDouble)
            {
                foreach (var value in VariableValueTest.SimpleDoubleValues)
                {
                    VerifyReturnValue(() => function(value), () => customFunction(new[] { VariableValue.Parse(value) }));
                }
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int, int> intFunction, Func<double, double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, intFunction, functionName);
            NumericFunctionTest(lib, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int, int> function, string functionName)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    VerifyReturnValue(() => function(value1, value2), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2) }));
                }
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<double, double, double> function, string functionName, bool testIntToDouble = true)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    VerifyReturnValue(() => function(value1, value2), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2) }));
                }
            }

            if (testIntToDouble)
            {
                foreach (var value1 in VariableValueTest.SimpleIntValues)
                {
                    foreach (var value2 in VariableValueTest.SimpleIntValues)
                    {
                        VerifyReturnValue(() => function(value1, value2), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2) }));
                    }
                }
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int, int, int> intFunction, Func<double, double, double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, intFunction, functionName);
            NumericFunctionTest(lib, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<int, int, int, int> function, string functionName)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    foreach (var value3 in VariableValueTest.SimpleIntValues)
                    {
                        VerifyReturnValue(() => function(value1, value2, value3), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2), VariableValue.Parse(value3) }));
                    }
                }
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, Func<double, double, double, double> function, string functionName, bool testIntToDouble = true)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    foreach (var value3 in VariableValueTest.SimpleDoubleValues)
                    {
                        VerifyReturnValue(() => function(value1, value2, value3), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2), VariableValue.Parse(value3) }));
                    }
                }
            }

            if (testIntToDouble)
            {
                foreach (var value1 in VariableValueTest.SimpleIntValues)
                {
                    foreach (var value2 in VariableValueTest.SimpleIntValues)
                    {
                        foreach (var value3 in VariableValueTest.SimpleIntValues)
                        {
                            VerifyReturnValue(() => function(value1, value2, value3), () => customFunction(new[] { VariableValue.Parse(value1), VariableValue.Parse(value2), VariableValue.Parse(value3) }));
                        }
                    }
                }
            }
        }

        private static void VerifyReturnValue(Func<int> function, Func<VariableValue> customFunction)
        {
            int? expected = null;
            try
            {
                expected = function();
            }
            catch { }

            VariableValue? actual = null;
            try
            {
                actual = customFunction();
            }
            catch { }

            Assert.AreEqual(expected is null, actual is null);

            if (expected.HasValue && actual.HasValue)
            {
                Assert.AreEqual(VariableValueType.Integer, actual.Value.Type);
                Assert.AreEqual(expected.Value, actual.Value.IntegerValue);
            }
        }

        private static void VerifyReturnValue(Func<double> function, Func<VariableValue> customFunction)
        {
            double? expected = null;
            try
            {
                expected = function();
            }
            catch { }

            VariableValue? actual = null;
            try
            {
                actual = customFunction();
            }
            catch { }

            Assert.AreEqual(expected is null, actual is null);

            if (expected.HasValue && actual.HasValue)
            {
                Assert.AreEqual(VariableValueType.Double, actual.Value.Type);
                Assert.AreEqual(expected.Value, actual.Value.DoubleValue);
            }
        }
    }
}
