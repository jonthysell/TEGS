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
            Graph graph = LoadXml(CarwashGraphXML);
            Assert.IsNotNull(graph);

            Assert.AreEqual(4, graph.VertexCount);
            Assert.AreEqual(5, graph.EdgeCount);
        }

        [TestMethod]
        public void Graph_SaveTest()
        {
            Graph graph = LoadXml(CarwashGraphXML);
            Assert.IsNotNull(graph);

            StringBuilder sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb))
            {
                graph.SaveXml(xw);
            }

            Assert.IsNotNull(sb);
            string graphXml = sb.ToString();

            Assert.IsFalse(string.IsNullOrWhiteSpace(graphXml));

            Assert.AreEqual(CarwashGraphXML, graphXml);
        }

        private Graph LoadXml(string xml)
        {
            Graph graph = null;

            using (StringReader sr = new StringReader(xml))
            {
                graph = Graph.LoadXml(XmlReader.Create(sr));
            }

            return graph;
        }

        private static string CarwashGraphXML = "<?xml version=\"1.0\" encoding=\"utf-16\"?><graph description=\"An automatic carwash\"><verticies><vertex id=\"0\" name=\"RUN\" description=\"The simulation run is started\" code=\"\" parameters=\"QUEUE, SERVERS\" x=\"0\" y=\"0\" starting=\"True\" /><vertex id=\"1\" name=\"ENTER\" description=\"Cars enter the line\" code=\"QUEUE = QUEUE + 1\" parameters=\"\" x=\"0\" y=\"0\" /><vertex id=\"2\" name=\"START\" description=\"Service starts\" code=\"SERVERS = SERVERS - 1&#xD;&#xA;QUEUE = QUEUE - 1\" parameters=\"\" x=\"0\" y=\"0\" /><vertex id=\"3\" name=\"LEAVE\" description=\"Cars leave\" code=\"SERVERS = SERVERS + 1\" parameters=\"\" x=\"0\" y=\"0\" /></verticies><edges><edge id=\"0\" source=\"0\" target=\"1\" action=\"Schedule\" description=\"The car will enter the line\" condition=\"\" delay=\"\" priority=\"5\" parameters=\"\" /><edge id=\"1\" source=\"1\" target=\"1\" action=\"Schedule\" description=\"The next customer enters in 3 to 8 minutes\" condition=\"\" delay=\"t_uniformvariate(3, 8)\" priority=\"6\" parameters=\"\" /><edge id=\"2\" source=\"1\" target=\"2\" action=\"Schedule\" description=\"There are available servers to start washing the car\" condition=\"SERVERS &gt; 0\" delay=\"\" priority=\"5\" parameters=\"\" /><edge id=\"3\" source=\"2\" target=\"3\" action=\"Schedule\" description=\"The car will be in service for at least 5 minutes\" condition=\"\" delay=\"t_uniformvariate(5, 20)\" priority=\"6\" parameters=\"\" /><edge id=\"4\" source=\"3\" target=\"2\" action=\"Schedule\" description=\"There are cars in queue, start service for the next car in line\" condition=\"QUEUE &gt; 0\" delay=\"\" priority=\"5\" parameters=\"\" /></edges></graph>";
    }
}
