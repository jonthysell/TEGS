﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.IO;
using System.Text;

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

            Assert.AreEqual(0, graph.StateVariables.Count);
            Assert.AreEqual(0, graph.Vertices.Count);
            Assert.AreEqual(0, graph.Edges.Count);
        }

        [TestMethod]
        public void Graph_LoadTest()
        {
            Graph graph = TestGraph.Load("carwash.json");
            Assert.IsNotNull(graph);

            Assert.AreEqual(2, graph.StateVariables.Count);
            Assert.AreEqual(4, graph.Vertices.Count);
            Assert.AreEqual(5, graph.Edges.Count);
        }

        [TestMethod]
        public void Graph_SaveTest()
        {
            Graph graph = TestGraph.Load("carwash.json");
            Assert.IsNotNull(graph);

            MemoryStream ms = new MemoryStream();
            graph.Save(ms);

            Assert.IsNotNull(ms);
            string graphOutput = Encoding.UTF8.GetString(ms.ToArray());

            Assert.IsFalse(string.IsNullOrWhiteSpace(graphOutput));
        }

        [TestMethod]
        public void Graph_CarwashTest()
        {
            Graph graph = TestGraph.Carwash;
            Assert.IsNotNull(graph);

            Assert.AreEqual(2, graph.StateVariables.Count);
            Assert.AreEqual(4, graph.Vertices.Count);
            Assert.AreEqual(5, graph.Edges.Count);
        }

        [TestMethod]
        public void Graph_BreakdownTest()
        {
            Graph graph = TestGraph.Breakdown;
            Assert.IsNotNull(graph);

            Assert.AreEqual(2, graph.StateVariables.Count);
            Assert.AreEqual(6, graph.Vertices.Count);
            Assert.AreEqual(10, graph.Edges.Count);
        }
    }
}
