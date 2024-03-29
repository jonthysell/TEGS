﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class VariableValueTest
    {
        [TestMethod]
        public void VariableValue_NewTest()
        {
            foreach (var expectedValue in ValidBooleanValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsBoolean);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.BooleanValue);
            }

            foreach (var expectedValue in ValidIntegerValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }

            foreach (var expectedValue in ValidDoubleValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }

            foreach (var expectedValue in ValidStringValues)
            {
                var actualValue = new VariableValue(expectedValue);
                Assert.IsTrue(actualValue.IsString);
                Assert.AreEqual(VariableValueType.String, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.StringValue);
            }
        }

        [TestMethod]
        public void VariableValue_ParseTest()
        {
            foreach (var expectedValue in ValidBooleanValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsBoolean);
                Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.BooleanValue);
            }

            foreach (var expectedValue in ValidIntegerValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }

            foreach (var expectedValue in ValidDoubleValues)
            {
                var actualValue = VariableValue.Parse(expectedValue);
                Assert.IsTrue(actualValue.IsNumber);
                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                Assert.AreEqual(expectedValue, actualValue.AsNumber());
            }

            foreach (var expectedValue in ValidStringValues)
            {
                if (expectedValue is not null)
                {
                    var actualValue = VariableValue.Parse(expectedValue);
                    Assert.IsTrue(actualValue.IsString);
                    Assert.AreEqual(VariableValueType.String, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.StringValue);
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorEqualsTest()
        {
            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidIntegerValues)
            {
                foreach (var value2 in ValidIntegerValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in ValidDoubleValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidDoubleValues)
            {
                foreach (var value2 in ValidDoubleValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in ValidIntegerValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    var expectedValue = value1 == value2;
                    var actualValue = new VariableValue(value1) == new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorNotEqualsTest()
        {
            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidIntegerValues)
            {
                foreach (var value2 in ValidIntegerValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in ValidDoubleValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidDoubleValues)
            {
                foreach (var value2 in ValidDoubleValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in ValidIntegerValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    var expectedValue = value1 != value2;
                    var actualValue = new VariableValue(value1) != new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorNegativeTest()
        {
            foreach (var value in ValidIntegerValues)
            {
                var expectedValue = - value;
                var actualValue = - new VariableValue(value);

                Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.IntegerValue);
            }

            foreach (var value in ValidDoubleValues)
            {
                var expectedValue = -value;
                var actualValue = -new VariableValue(value);

                Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                Assert.AreEqual(expectedValue, actualValue.DoubleValue);
            }

            foreach (var value in ValidBooleanValues)
            {
                Assert.ThrowsException<ValueIsNotANumberException>(() =>
                {
                    var actualValue = -new VariableValue(value);
                });
            }

            foreach (var value in ValidStringValues)
            {
                Assert.ThrowsException<ValueIsNotANumberException>(() =>
                {
                    var actualValue = -new VariableValue(value);
                });
            }
        }

        [TestMethod]
        public void VariableValue_OperatorNotTest()
        {
            foreach (var value in ValidBooleanValues)
            {
                var expectedValue = ! value;
                var actualValue = ! new VariableValue(value);

                Assert.AreEqual(expectedValue, actualValue);
            }

            foreach (var value in ValidIntegerValues)
            {
                Assert.ThrowsException<ValueIsNotABooleanException>(() =>
                {
                    var actualValue = !new VariableValue(value);
                });
            }

            foreach (var value in ValidDoubleValues)
            {
                Assert.ThrowsException<ValueIsNotABooleanException>(() =>
                {
                    var actualValue = !new VariableValue(value);
                });
            }

            foreach (var value in ValidStringValues)
            {
                Assert.ThrowsException<ValueIsNotABooleanException>(() =>
                {
                    var actualValue = !new VariableValue(value);
                });
            }
        }

        [TestMethod]
        public void VariableValue_OperatorAddTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 + value2;
                    var actualValue = new VariableValue(value1) + new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 + value2;
                    var actualValue = new VariableValue(value1) + new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 + value2;
                    var actualValue = new VariableValue(value1) + new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 + value2;
                    var actualValue = new VariableValue(value1) + new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);

                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    var expectedValue = value1 + value2;
                    var actualValue = new VariableValue(value1) + new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.String, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.StringValue);
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) + new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorSubtractTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 - value2;
                    var actualValue = new VariableValue(value1) - new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 - value2;
                    var actualValue = new VariableValue(value1) - new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 - value2;
                    var actualValue = new VariableValue(value1) - new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);

                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 - value2;
                    var actualValue = new VariableValue(value1) - new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);

                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) - new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) - new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorMultiplyTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 * value2;
                    var actualValue = new VariableValue(value1) * new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 * value2;
                    var actualValue = new VariableValue(value1) * new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 * value2;
                    var actualValue = new VariableValue(value1) * new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);

                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 * value2;
                    var actualValue = new VariableValue(value1) * new VariableValue(value2);

                    Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                    Assert.AreEqual(expectedValue, actualValue.DoubleValue);

                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) * new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) * new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorDivideTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    if (value2 != 0)
                    {
                        var expectedValue = value1 / value2;
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);

                        Assert.AreEqual(VariableValueType.Integer, actualValue.Type);
                        Assert.AreEqual(expectedValue, actualValue.IntegerValue);
                    }
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    if (value2 != 0.0)
                    {
                        var expectedValue = value1 / value2;
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);

                        Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                        Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                    }
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    if (value2 != 0.0)
                    {
                        var expectedValue = value1 / value2;
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);

                        Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                        Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                    }
                }

                foreach (var value2 in SimpleIntValues)
                {
                    if (value2 != 0)
                    {
                        var expectedValue = value1 / value2;
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);

                        Assert.AreEqual(VariableValueType.Double, actualValue.Type);
                        Assert.AreEqual(expectedValue, actualValue.DoubleValue);
                    }
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) / new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorLessThanTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 < value2;
                    var actualValue = new VariableValue(value1) < new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 < value2;
                    var actualValue = new VariableValue(value1) < new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 < value2;
                    var actualValue = new VariableValue(value1) < new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 < value2;
                    var actualValue = new VariableValue(value1) < new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) < new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) < new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorGreaterThanTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 > value2;
                    var actualValue = new VariableValue(value1) > new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 > value2;
                    var actualValue = new VariableValue(value1) > new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 > value2;
                    var actualValue = new VariableValue(value1) > new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 > value2;
                    var actualValue = new VariableValue(value1) > new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) > new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) > new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorLessThanEqualsTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 <= value2;
                    var actualValue = new VariableValue(value1) <= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 <= value2;
                    var actualValue = new VariableValue(value1) <= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 <= value2;
                    var actualValue = new VariableValue(value1) <= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 <= value2;
                    var actualValue = new VariableValue(value1) <= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) <= new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) <= new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorGreaterThanEqualsTest()
        {
            foreach (var value1 in SimpleIntValues)
            {
                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 >= value2;
                    var actualValue = new VariableValue(value1) >= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 >= value2;
                    var actualValue = new VariableValue(value1) >= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in SimpleDoubleValues)
            {
                foreach (var value2 in SimpleDoubleValues)
                {
                    var expectedValue = value1 >= value2;
                    var actualValue = new VariableValue(value1) >= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }

                foreach (var value2 in SimpleIntValues)
                {
                    var expectedValue = value1 >= value2;
                    var actualValue = new VariableValue(value1) >= new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) >= new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) >= new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorAndTest()
        {
            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    var expectedValue = value1 & value2;
                    var actualValue = new VariableValue(value1) & new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidIntegerValues)
            {
                foreach (var value2 in ValidIntegerValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) & new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidDoubleValues)
            {
                foreach (var value2 in ValidDoubleValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) & new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) & new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorOrTest()
        {
            foreach (var value1 in ValidBooleanValues)
            {
                foreach (var value2 in ValidBooleanValues)
                {
                    var expectedValue = value1 | value2;
                    var actualValue = new VariableValue(value1) | new VariableValue(value2);

                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            foreach (var value1 in ValidIntegerValues)
            {
                foreach (var value2 in ValidIntegerValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) | new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidDoubleValues)
            {
                foreach (var value2 in ValidDoubleValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) | new VariableValue(value2);
                    });
                }
            }

            foreach (var value1 in ValidStringValues)
            {
                foreach (var value2 in ValidStringValues)
                {
                    Assert.ThrowsException<VariableValueValueOperationException>(() =>
                    {
                        var actualValue = new VariableValue(value1) | new VariableValue(value2);
                    });
                }
            }
        }

        [TestMethod]
        public void VariableValue_OperatorTrueTest()
        {
            if (!VariableValue.True)
            {
                Assert.Fail("VariableValue.True does not evaluate as true.");
            }
        }

        [TestMethod]
        public void VariableValue_OperatorFalseTest()
        {
            if (VariableValue.False)
            {
                Assert.Fail("VariableValue.False does not evaluate as false.");
            }
        }

        public static readonly bool[] ValidBooleanValues = new bool[] { false, true };

        public static readonly int[] ValidIntegerValues = new int[] { int.MinValue, -1, 0, 1, int.MaxValue };

        public static readonly double[] ValidDoubleValues = new double[] { double.NegativeInfinity, double.MinValue, -1.0, 0.0, double.Epsilon, 1.0, double.MaxValue, double.PositiveInfinity };

        public static readonly string[] ValidStringValues = new string[] { "", "test" };

        public static readonly object[] ValidParseValues = new object[] { false, true, int.MinValue, -1, 0, 1, int.MaxValue, double.NegativeInfinity, double.MinValue, -1.0, 0.0, double.Epsilon, 1.0, double.MaxValue, double.PositiveInfinity, "", "test" };

        public static readonly int[] SimpleIntValues = new int[] { int.MinValue / 2, -2, -1, 0, 1, 2, int.MaxValue / 2 };

        public static readonly double[] SimpleDoubleValues = new double[] { double.MinValue / 2.0, -2.0, -1.0, -0.1, 0.0, 0.1, 1.0, 2.0, double.MaxValue / 2.0 };
    }
}
