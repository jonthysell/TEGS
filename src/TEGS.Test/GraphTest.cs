// 
// GraphTest.cs
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

using System.IO;
using System.Text;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    [DeploymentItem("TestGraphs")]
    public class GraphTest
    {
        [TestMethod]
        public void Graph_NewTest()
        {
            Graph graph = new Graph();
            Assert.IsNotNull(graph);

            Assert.AreEqual(0, graph.VertexCount);
            Assert.AreEqual(0, graph.EdgeCount);
        }

        [TestMethod]
        public void Graph_LoadTest()
        {
            Graph graph = LoadXml("carwash.xml");
            Assert.IsNotNull(graph);

            Assert.AreEqual(4, graph.VertexCount);
            Assert.AreEqual(5, graph.EdgeCount);
        }

        [TestMethod]
        public void Graph_SaveTest()
        {
            Graph graph = LoadXml("carwash.xml");
            Assert.IsNotNull(graph);

            MemoryStream ms = new MemoryStream();
            graph.SaveXml(ms);

            Assert.IsNotNull(ms);
            string graphXml = Encoding.UTF8.GetString(ms.ToArray());

            Assert.IsFalse(string.IsNullOrWhiteSpace(graphXml));
        }

        private Graph LoadXml(string fileName)
        {
            Graph graph = null;

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                graph = Graph.LoadXml(fs);
            }

            return graph;
        }
    }
}
