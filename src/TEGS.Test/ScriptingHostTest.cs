// 
// ScriptingHostTest.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
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

using TEGS.Lua;

namespace TEGS.Test
{
    [TestClass]
    public class LuaScriptingHostTest : ScriptingHostTest<LuaScriptingHost> { }

    public abstract class ScriptingHostTest<TScriptingHost> where TScriptingHost : ScriptingHost, new()
    {
        [TestMethod]
        public void ScriptingHost_NewTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);
        }

        #region Valid Variable Tests

        [TestMethod]
        public void ScriptingHost_ValidBooleanVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            bool[] test = { true, false };

            for (int i = 0; i < test.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Boolean);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Boolean, defaultValue.Type);
                Assert.AreEqual(default(bool), defaultValue.BooleanValue);
                
                host.Assign(sv, new VariableValue(test[i]));

                VariableValue testValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Boolean, testValue.Type);
                Assert.AreEqual(test[i], testValue.BooleanValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidIntegerVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            int[] test = { 0, 1, int.MinValue, int.MaxValue };

            for (int i = 0; i < test.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Integer);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Integer, defaultValue.Type);
                Assert.AreEqual(default(int), defaultValue.IntegerValue);

                host.Assign(sv, new VariableValue(test[i]));

                VariableValue testValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Integer, testValue.Type);
                Assert.AreEqual(test[i], testValue.IntegerValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidDoubleVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            double[] test = { 0, 1, double.MinValue, double.MinValue };

            for (int i = 0; i < test.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.Double);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Double, defaultValue.Type);
                Assert.AreEqual(default(double), defaultValue.DoubleValue);

                host.Assign(sv, new VariableValue(test[i]));

                VariableValue testValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.Double, testValue.Type);
                Assert.AreEqual(test[i], testValue.DoubleValue);
            }
        }

        [TestMethod]
        public void ScriptingHost_ValidStringVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            string[] test = { null, "", "test" };

            for (int i = 0; i < test.Length; i++)
            {
                StateVariable sv = new StateVariable($"test{i}", VariableValueType.String);
                host.Create(sv);

                VariableValue defaultValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.String, defaultValue.Type);
                Assert.AreEqual(default(string), defaultValue.StringValue);

                host.Assign(sv, new VariableValue(test[i]));

                VariableValue testValue = host.Get(sv);
                Assert.AreEqual(VariableValueType.String, testValue.Type);
                Assert.AreEqual(test[i], testValue.StringValue);
            }
        }

        #endregion

        #region Invalid Variable Tests

        [TestMethod]
        public void ScriptingHost_InvalidBooleanVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Boolean);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(bool))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(string))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidIntegerVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Integer);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(int))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(bool))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidDoubleVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.Double);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(double))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(int))));
        }

        [TestMethod]
        public void ScriptingHost_InvalidStringVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            StateVariable sv = new StateVariable("test", VariableValueType.String);

            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Get(sv));
            Assert.ThrowsException<StateVariableNotFoundException>(() => host.Assign(sv, new VariableValue(default(string))));
            Assert.ThrowsException<StateVariableAssignmentException>(() => host.Assign(sv, new VariableValue(default(double))));
        }

        #endregion
    }
}
