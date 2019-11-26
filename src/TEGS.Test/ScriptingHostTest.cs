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
                host.CreateBoolean($"test{i}");
                Assert.AreEqual(default(bool), host.GetBoolean($"test{i}"));

                host.AssignBoolean($"test{i}", test[i]);
                Assert.AreEqual(test[i], host.GetBoolean($"test{i}"));
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
                host.CreateInteger($"test{i}");
                Assert.AreEqual(default(int), host.GetInteger($"test{i}"));

                host.AssignInteger($"test{i}", test[i]);
                Assert.AreEqual(test[i], host.GetInteger($"test{i}"));
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
                host.CreateDouble($"test{i}");
                Assert.AreEqual(default(double), host.GetDouble($"test{i}"));

                host.AssignDouble($"test{i}", test[i]);
                Assert.AreEqual(test[i], host.GetDouble($"test{i}"));
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
                host.CreateString($"test{i}");
                Assert.AreEqual(default(string), host.GetString($"test{i}"));

                host.AssignString($"test{i}", test[i]);
                Assert.AreEqual(test[i], host.GetString($"test{i}"));
            }
        }

        #endregion

        #region Invalid Variable Tests

        [TestMethod]
        public void ScriptingHost_InvalidBooleanVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            Assert.ThrowsException<GlobalVariableNotFoundException>(() => host.GetBoolean("test"));
        }

        [TestMethod]
        public void ScriptingHost_InvalidIntegerVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            Assert.ThrowsException<GlobalVariableNotFoundException>(() => host.GetInteger("test"));
        }

        [TestMethod]
        public void ScriptingHost_InvalidDoubleVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            Assert.ThrowsException<GlobalVariableNotFoundException>(() => host.GetDouble("test"));
        }

        [TestMethod]
        public void ScriptingHost_InvalidStringVariableTest()
        {
            TScriptingHost host = new TScriptingHost();
            Assert.IsNotNull(host);

            Assert.ThrowsException<GlobalVariableNotFoundException>(() => host.GetString("test"));
        }

        #endregion
    }
}
