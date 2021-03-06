﻿// 
// CodeGeneratorTest.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
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

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    [DeploymentItem("TestGraphs")]
    public class CodeGeneratorTest
    {
        [TestMethod]
        public void CodeGenerator_CarwashValidTest() => CodeGenerator_ValidTest(TestGraph.Carwash);

        [TestMethod]
        public void CodeGenerator_BreakdownValidTest() => CodeGenerator_ValidTest(TestGraph.Breakdown);

        [TestMethod]
        public void CodeGenerator_CarwashXMLValidTest() => CodeGenerator_ValidTest(TestGraph.LoadXml("carwash.xml"));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CodeGenerator_NullInvalidTest() => CodeGenerator.Generate(null);

        private static void CodeGenerator_ValidTest(Graph graph)
        {
            string code = CodeGenerator.Generate(graph);
            Assert.IsNotNull(code);
        }
    }
}
