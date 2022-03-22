// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

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
            var testedKeys = new HashSet<string>();

            ConstantTest(lib, testedKeys, nameof(bool.FalseString), bool.FalseString);
            ConstantTest(lib, testedKeys, nameof(bool.TrueString), bool.TrueString);

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_SystemIntegerTest()
        {
            var lib = BaseLibraries.SystemInteger;
            var testedKeys = new HashSet<string>();

            ConstantTest(lib, testedKeys, nameof(int.MaxValue), int.MaxValue);
            ConstantTest(lib, testedKeys, nameof(int.MinValue), int.MinValue);

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_SystemDoubleTest()
        {
            var lib = BaseLibraries.SystemDouble;
            var testedKeys = new HashSet<string>();

            ConstantTest(lib, testedKeys, nameof(double.Epsilon), double.Epsilon);
            ConstantTest(lib, testedKeys, nameof(double.MaxValue), double.MaxValue);
            ConstantTest(lib, testedKeys, nameof(double.MinValue), double.MinValue);
            ConstantTest(lib, testedKeys, nameof(double.NaN), double.NaN);
            ConstantTest(lib, testedKeys, nameof(double.NegativeInfinity), double.NegativeInfinity);
            ConstantTest(lib, testedKeys, nameof(double.PositiveInfinity), double.PositiveInfinity);

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_SystemStringTest()
        {
            var lib = BaseLibraries.SystemString;
            var testedKeys = new HashSet<string>();

            ConstantTest(lib, testedKeys, nameof(string.Empty), string.Empty);

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_StringLibraryTest()
        {
            var lib = BaseLibraries.StringLibrary;
            var testedKeys = new HashSet<string>();

            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(StringLibrary.Length), out customFunction));
            foreach (var value in VariableValueTest.ValidStringValues)
            {
                var actual = customFunction(new[] { new VariableValue(value) });
                Assert.AreEqual(VariableValueType.Integer, actual.Type);
                Assert.AreEqual(value.Length, actual.IntegerValue);
            }
            testedKeys.Add(nameof(StringLibrary.Length));

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_ConvertLibraryTest()
        {
            var lib = BaseLibraries.ConvertLibrary;
            var testedKeys = new HashSet<string>();

            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToBoolean), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, bool.FalseString, bool.TrueString })
            {
                var expected = Convert.ToBoolean(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Boolean, actual.Type);
                Assert.AreEqual(expected, actual.BooleanValue);
            }
            testedKeys.Add(nameof(ConvertLibrary.ToBoolean));

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToInteger), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1" })
            {
                var expected = Convert.ToInt32(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Integer, actual.Type);
                Assert.AreEqual(expected, actual.IntegerValue);
            }
            testedKeys.Add(nameof(ConvertLibrary.ToInteger));

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToDouble), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1", "-1.0", "0.0", "1.0" })
            {
                var expected = Convert.ToDouble(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.Double, actual.Type);
                Assert.AreEqual(expected, actual.DoubleValue);
            }
            testedKeys.Add(nameof(ConvertLibrary.ToDouble));

            Assert.IsTrue(lib.Functions.TryGetValue(nameof(ConvertLibrary.ToString), out customFunction));
            foreach (var value in new object[] { -1, 0, 1, false, true, -1.0, 0.0, 1.0, "-1", "0", "1", "-1.0", "0.0", "1.0", bool.FalseString, bool.TrueString })
            {
                var expected = Convert.ToString(value);
                var actual = customFunction(new[] { VariableValue.Parse(value) });
                Assert.AreEqual(VariableValueType.String, actual.Type);
                Assert.AreEqual(expected, actual.StringValue);
            }
            testedKeys.Add(nameof(ConvertLibrary.ToString));

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_SystemMathTest()
        {
            var lib = BaseLibraries.SystemMath;
            var testedKeys = new HashSet<string>();

            ConstantTest(lib, testedKeys, nameof(Math.E), Math.E);
            ConstantTest(lib, testedKeys, nameof(Math.PI), Math.PI);
            ConstantTest(lib, testedKeys, nameof(Math.Tau), Math.Tau);

            NumericFunctionTest(lib, testedKeys, Math.Abs, Math.Abs, nameof(Math.Abs));
            NumericFunctionTest(lib, testedKeys, Math.Acos, nameof(Math.Acos));
            NumericFunctionTest(lib, testedKeys, Math.Acosh, nameof(Math.Acosh));
            NumericFunctionTest(lib, testedKeys, Math.Asin, nameof(Math.Asin));
            NumericFunctionTest(lib, testedKeys, Math.Asinh, nameof(Math.Asinh));
            NumericFunctionTest(lib, testedKeys, Math.Atan, nameof(Math.Atan));
            NumericFunctionTest(lib, testedKeys, Math.Atanh, nameof(Math.Atanh));
            NumericFunctionTest(lib, testedKeys, Math.Atan2, nameof(Math.Atan2));
            NumericFunctionTest(lib, testedKeys, Math.BitDecrement, nameof(Math.BitDecrement));
            NumericFunctionTest(lib, testedKeys, Math.BitIncrement, nameof(Math.BitIncrement));
            NumericFunctionTest(lib, testedKeys, Math.Cbrt, nameof(Math.Cbrt));
            NumericFunctionTest(lib, testedKeys, Math.Ceiling, nameof(Math.Ceiling));
            NumericFunctionTest(lib, testedKeys, Math.Clamp, Math.Clamp, nameof(Math.Clamp));
            NumericFunctionTest(lib, testedKeys, Math.CopySign, nameof(Math.CopySign));
            NumericFunctionTest(lib, testedKeys, Math.Cos, nameof(Math.Cos));
            NumericFunctionTest(lib, testedKeys, Math.Cosh, nameof(Math.Cosh));
            NumericFunctionTest(lib, testedKeys, Math.Exp, nameof(Math.Exp));
            NumericFunctionTest(lib, testedKeys, Math.Floor, nameof(Math.Floor));
            NumericFunctionTest(lib, testedKeys, Math.FusedMultiplyAdd, nameof(Math.FusedMultiplyAdd));
            NumericFunctionTest(lib, testedKeys, Math.IEEERemainder, nameof(Math.IEEERemainder));
            NumericFunctionTest(lib, testedKeys, (a) => Math.Log(a), nameof(Math.Log));
            NumericFunctionTest(lib, testedKeys, Math.Log10, nameof(Math.Log10));
            NumericFunctionTest(lib, testedKeys, Math.Log2, nameof(Math.Log2));
            NumericFunctionTest(lib, testedKeys, Math.Max, Math.Max, nameof(Math.Max));
            NumericFunctionTest(lib, testedKeys, Math.MaxMagnitude, nameof(Math.MaxMagnitude));
            NumericFunctionTest(lib, testedKeys, Math.Min, Math.Min, nameof(Math.Min));
            NumericFunctionTest(lib, testedKeys, Math.MinMagnitude, nameof(Math.MinMagnitude));
            NumericFunctionTest(lib, testedKeys, Math.Pow, nameof(Math.Pow));
            NumericFunctionTest(lib, testedKeys, Math.ReciprocalEstimate, nameof(Math.ReciprocalEstimate));
            NumericFunctionTest(lib, testedKeys, Math.ReciprocalSqrtEstimate, nameof(Math.ReciprocalSqrtEstimate));
            NumericFunctionTest(lib, testedKeys, Math.Round, nameof(Math.Round));
            NumericFunctionTest(lib, testedKeys, Math.Sign, nameof(Math.Sign));
            NumericFunctionTest(lib, testedKeys, Math.Sin, nameof(Math.Sin));
            NumericFunctionTest(lib, testedKeys, Math.Sinh, nameof(Math.Sinh));
            NumericFunctionTest(lib, testedKeys, Math.Sqrt, nameof(Math.Sqrt));
            NumericFunctionTest(lib, testedKeys, Math.Tan, nameof(Math.Tan));
            NumericFunctionTest(lib, testedKeys, Math.Tanh, nameof(Math.Tanh));
            NumericFunctionTest(lib, testedKeys, Math.Truncate, nameof(Math.Truncate));

            VerifyAllItemsTested(lib, testedKeys);
        }

        [TestMethod]
        public void BaseLibraries_RandomVariateLibraryTest()
        {
            var testedKeys = new HashSet<string>();

            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).UniformVariate, nameof(RandomExtensions.UniformVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).ExponentialVariate, nameof(RandomExtensions.ExponentialVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).NormalVariate, nameof(RandomExtensions.NormalVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).LogNormalVariate, nameof(RandomExtensions.LogNormalVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).TriangularVariate, nameof(RandomExtensions.TriangularVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).GammaVariate, nameof(RandomExtensions.GammaVariate));
            NumericFunctionTest(BaseLibraries.RandomVariateLibrary(12345), testedKeys, new Random(12345).BetaVariate, nameof(RandomExtensions.BetaVariate));

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
            testedKeys.Add(nameof(RandomExtensions.ErlangVariate));

            VerifyAllItemsTested(lib, testedKeys);
        }

        private static void ConstantTest(ReflectionLibraryBase lib, HashSet<string> testedKeys, string key, bool expectedValue)
        {
            Assert.IsTrue(lib.Constants.TryGetValue(key, out var constantValue));
            Assert.AreEqual(VariableValueType.Boolean, constantValue.Type);
            Assert.AreEqual(expectedValue, constantValue.BooleanValue);
            testedKeys.Add(key);
        }

        private static void ConstantTest(ReflectionLibraryBase lib, HashSet<string> testedKeys, string key, int expectedValue)
        {
            Assert.IsTrue(lib.Constants.TryGetValue(key, out var constantValue));
            Assert.AreEqual(VariableValueType.Integer, constantValue.Type);
            Assert.AreEqual(expectedValue, constantValue.IntegerValue);
            testedKeys.Add(key);
        }

        private static void ConstantTest(ReflectionLibraryBase lib, HashSet<string> testedKeys, string key, double expectedValue)
        {
            Assert.IsTrue(lib.Constants.TryGetValue(key, out var constantValue));
            Assert.AreEqual(VariableValueType.Double, constantValue.Type);
            Assert.AreEqual(expectedValue, constantValue.DoubleValue);
            testedKeys.Add(key);
        }

        private static void ConstantTest(ReflectionLibraryBase lib, HashSet<string> testedKeys, string key, string expectedValue)
        {
            Assert.IsTrue(lib.Constants.TryGetValue(key, out var constantValue));
            Assert.AreEqual(VariableValueType.String, constantValue.Type);
            Assert.AreEqual(expectedValue, constantValue.StringValue);
            testedKeys.Add(key);
        }

        private static void VerifyAllItemsTested(ReflectionLibraryBase lib, HashSet<string> testedKeys)
        {
            foreach (var key in lib.Constants.Keys)
            {
                Assert.IsTrue(testedKeys.Contains(key), $"Constant { key } in { lib.Name } was not tested.");
            }

            foreach (var key in lib.Functions.Keys)
            {
                Assert.IsTrue(testedKeys.Contains(key), $"Function { key } in { lib.Name } was not tested.");
            }
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int> intFunction, Func<double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, testedKeys, intFunction, functionName);
            NumericFunctionTest(lib, testedKeys, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int> function, string functionName)
        {
            CustomFunction customFunction;

            Assert.IsTrue(lib.Functions.TryGetValue(functionName, out customFunction), $"Function { functionName } not found.");

            foreach (var value in VariableValueTest.SimpleIntValues)
            {
                VerifyReturnValue(() => function(value), () => customFunction(new[] { VariableValue.Parse(value) }));
            }

            testedKeys.Add(functionName);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<double, double> function, string functionName, bool testIntToDouble = true)
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

            testedKeys.Add(functionName);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int, int> intFunction, Func<double, double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, testedKeys, intFunction, functionName);
            NumericFunctionTest(lib, testedKeys, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int, int> function, string functionName)
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

            testedKeys.Add(functionName);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<double, double, double> function, string functionName, bool testIntToDouble = true)
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

            testedKeys.Add(functionName);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int, int, int> intFunction, Func<double, double, double, double> doubleFunction, string functionName)
        {
            NumericFunctionTest(lib, testedKeys, intFunction, functionName);
            NumericFunctionTest(lib, testedKeys, doubleFunction, functionName, false);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<int, int, int, int> function, string functionName)
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

            testedKeys.Add(functionName);
        }

        private static void NumericFunctionTest(SystemLibrary lib, HashSet<string> testedKeys, Func<double, double, double, double> function, string functionName, bool testIntToDouble = true)
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

            testedKeys.Add(functionName);
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
