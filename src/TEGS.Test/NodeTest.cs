﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class NodeTest
    {
        [TestMethod]
        public void Node_ValueTest()
        {
            foreach (var value in VariableValueTest.ValidParseValues)
            {
                var expectedValue = VariableValue.Parse(value);
                Evaluate_ValidTest(expectedValue, new NodeValue(expectedValue), new TestContext());
            }
        }

        [TestMethod]
        public void Node_VariableTest()
        {
            foreach (var value in VariableValueTest.ValidParseValues)
            {
                var expectedValue = VariableValue.Parse(value);

                var context = new TestContext();
                context.SetValue("test", expectedValue);

                Evaluate_ValidTest(expectedValue, new NodeVariable("test"), context);
            }
        }

        [TestMethod]
        public void Node_FunctionCallTest()
        {
            foreach (var value in VariableValueTest.ValidParseValues)
            {
                var expectedValue = VariableValue.Parse(value);

                var context = new TestContext();
                context.Functions.Add("test", (args) => expectedValue);

                Evaluate_ValidTest(expectedValue, new NodeFunctionCall("test", null), context);
            }
        }

        [TestMethod]
        public void Node_NegativeTest()
        {
            foreach (var value in VariableValueTest.ValidIntegerValues)
            {
                var variableValue = new VariableValue(value);

                var expectedValue = - variableValue;

                var rhs = new NodeValue(variableValue);

                Evaluate_ValidTest(expectedValue, new NodeNegative(rhs), new TestContext());
            }

            foreach (var value in VariableValueTest.ValidDoubleValues)
            {
                var variableValue = new VariableValue(value);

                var expectedValue = - variableValue;

                var rhs = new NodeValue(variableValue);

                Evaluate_ValidTest(expectedValue, new NodeNegative(rhs), new TestContext());
            }
        }

        [TestMethod]
        public void Node_NotTest()
        {
            foreach (var value in VariableValueTest.ValidBooleanValues)
            {
                var variableValue = new VariableValue(value);

                var expectedValue = ! variableValue;

                var rhs = new NodeValue(variableValue);

                Evaluate_ValidTest(expectedValue, new NodeNot(rhs), new TestContext());
            }
        }

        [TestMethod]
        public void Node_AssignTest()
        {
            foreach (var value in VariableValueTest.ValidParseValues)
            {
                var expectedValue = VariableValue.Parse(value);

                var context = new TestContext();

                var lhs = new NodeVariable("test");
                var rhs = new NodeValue(expectedValue);

                Evaluate_ValidTest(expectedValue, new NodeAssign(lhs, rhs), context);

                Assert.AreEqual(expectedValue, lhs.Evaluate(context));
            }
        }

        [TestMethod]
        public void Node_AddTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 + variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAddition(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 + variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAddition(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 + variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAddition(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 + variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAddition(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.ValidStringValues)
            {
                foreach (var value2 in VariableValueTest.ValidStringValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 + variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAddition(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_SubtractTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 - variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeSubtraction(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 - variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeSubtraction(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 - variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeSubtraction(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 - variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeSubtraction(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_MultiplyTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 * variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeMultiplication(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 * variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeMultiplication(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 * variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeMultiplication(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 * variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeMultiplication(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_DivideTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    if (value2 != 0)
                    {
                        var variableValue1 = new VariableValue(value1);
                        var variableValue2 = new VariableValue(value2);

                        var expectedValue = variableValue1 / variableValue2;

                        var lhs = new NodeValue(variableValue1);
                        var rhs = new NodeValue(variableValue2);

                        Evaluate_ValidTest(expectedValue, new NodeDivision(lhs, rhs), new TestContext());
                    }
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    if (value2 != 0.0)
                    {
                        var variableValue1 = new VariableValue(value1);
                        var variableValue2 = new VariableValue(value2);

                        var expectedValue = variableValue1 / variableValue2;

                        var lhs = new NodeValue(variableValue1);
                        var rhs = new NodeValue(variableValue2);

                        Evaluate_ValidTest(expectedValue, new NodeDivision(lhs, rhs), new TestContext());
                    }
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    if (value2 != 0.0)
                    {
                        var variableValue1 = new VariableValue(value1);
                        var variableValue2 = new VariableValue(value2);

                        var expectedValue = variableValue1 / variableValue2;

                        var lhs = new NodeValue(variableValue1);
                        var rhs = new NodeValue(variableValue2);

                        Evaluate_ValidTest(expectedValue, new NodeDivision(lhs, rhs), new TestContext());
                    }
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    if (value2 != 0)
                    {
                        var variableValue1 = new VariableValue(value1);
                        var variableValue2 = new VariableValue(value2);

                        var expectedValue = variableValue1 / variableValue2;

                        var lhs = new NodeValue(variableValue1);
                        var rhs = new NodeValue(variableValue2);

                        Evaluate_ValidTest(expectedValue, new NodeDivision(lhs, rhs), new TestContext());
                    }
                }
            }
        }

        [TestMethod]
        public void Node_LessThanTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 < variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThan(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 < variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThan(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 < variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThan(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 < variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThan(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_GreaterThanTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 > variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThan(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 > variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThan(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 > variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThan(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 > variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThan(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_LessThanEqualsTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 <= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThanEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 <= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThanEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 <= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThanEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 <= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeLessThanEquals(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_GreaterThanEqualsTest()
        {
            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 >= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThanEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 >= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThanEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 >= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThanEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 >= variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeGreaterThanEquals(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_EqualsTest()
        {
            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.ValidStringValues)
            {
                foreach (var value2 in VariableValueTest.ValidStringValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 == variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeEquals(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_NotEqualsTest()
        {
            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleIntValues)
            {
                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.SimpleDoubleValues)
            {
                foreach (var value2 in VariableValueTest.SimpleDoubleValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }

                foreach (var value2 in VariableValueTest.SimpleIntValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }
            }

            foreach (var value1 in VariableValueTest.ValidStringValues)
            {
                foreach (var value2 in VariableValueTest.ValidStringValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 != variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeNotEquals(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_AndTest()
        {
            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 & variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeAnd(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_OrTest()
        {
            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 | variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeOr(lhs, rhs), new TestContext());
                }
            }
        }

        [TestMethod]
        public void Node_ConditionalAndTest()
        {
            var context = new TestContext();
            context.Functions.Add("fail", (args) => throw new AssertFailedException());

            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 & variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = value1 ? (Node)new NodeValue(variableValue2) : new NodeFunctionCall("fail", null);

                    Evaluate_ValidTest(expectedValue, new NodeConditionalAnd(lhs, rhs), context);
                }
            }
        }

        [TestMethod]
        public void Node_ConditionalOrTest()
        {
            var context = new TestContext();
            context.Functions.Add("fail", (args) => throw new AssertFailedException());

            foreach (var value1 in VariableValueTest.ValidBooleanValues)
            {
                foreach (var value2 in VariableValueTest.ValidBooleanValues)
                {
                    var variableValue1 = new VariableValue(value1);
                    var variableValue2 = new VariableValue(value2);

                    var expectedValue = variableValue1 | variableValue2;

                    var lhs = new NodeValue(variableValue1);
                    var rhs = value1 ? (Node)new NodeFunctionCall("fail", null) : new NodeValue(variableValue2);

                    Evaluate_ValidTest(expectedValue, new NodeConditionalOr(lhs, rhs), context);
                }
            }
        }

        protected static void Evaluate_ValidTest(VariableValue expectedValue, Node node, IContext context)
        {
            var actualValue = node.Evaluate(context);
            Assert.AreEqual(expectedValue, actualValue);

            var reduced = node.Reduce();
            var reducedValue = reduced.Evaluate(context);
            Assert.AreEqual(expectedValue, reducedValue);
        }

        protected static void Evaluate_ValidTest(bool expectedValue, Node node, IContext context)
        {
            var actualValue = node.Evaluate(context);
            Assert.AreEqual(VariableValueType.Boolean, actualValue.Type);
            Assert.AreEqual(expectedValue, actualValue.BooleanValue);

            var reduced = node.Reduce();
            var reducedValue = reduced.Evaluate(context);
            Assert.AreEqual(VariableValueType.Boolean, reducedValue.Type);
            Assert.AreEqual(expectedValue, reducedValue.BooleanValue);
        }
    }
}
