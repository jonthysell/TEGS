// 
// ValidatorTest.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019, 2020 Jon Thysell <http://jonthysell.com>
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
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    [DeploymentItem("TestGraphs")]
    public class ValidatorTest
    {
        [TestMethod]
        public void Validator_CarwashValidTest()
        {
            Validator_ValidTest(TestGraph.Carwash);
        }

        [TestMethod]
        public void Validator_BreakdownValidTest()
        {
            Validator_ValidTest(TestGraph.Breakdown);
        }

        [TestMethod]
        public void Validator_CarwashXMLValidTest()
        {
            Validator_ValidTest(TestGraph.LoadXml("carwash.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validator_NullInvalidTest()
        {
            Validator_InvalidTest(null);
        }

        [TestMethod]
        public void Validator_NewGraphInvalidTest()
        {
            Graph graph = new Graph();

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new NoStartingVertexValidationError()
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_NoStartingVertexInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddVertex("RUN");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new NoStartingVertexValidationError()
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_BlankVertexNameInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex = graph.AddVertex("", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new BlankVertexNameValidationError(vertex)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidParameterNameVertexInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex = graph.AddVertex("RUN", true);
            vertex.AddParameter("test");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidParameterNameVertexValidationError(vertex, "test")
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_DuplicateVertexNameInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN", true);
            Vertex vertex2 = graph.AddVertex("RUN");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new DuplicateVertexNamesValidationError(new List<Vertex>() { vertex1, vertex2 })
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_MultipleStartingVertexInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new MultipleStartingVertexValidationError(new List<Vertex>() { vertex1, vertex2 })
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_BlankStateVariableNameInvalidTest()
        {
            Graph graph = new Graph();
            StateVariable stateVariable = graph.AddStateVariable("", VariableValueType.Integer);
            graph.AddVertex("RUN", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new BlankStateVariableNameValidationError(stateVariable)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidStateVariableNameInvalidTest()
        {
            Graph graph = new Graph();
            StateVariable stateVariable = graph.AddStateVariable("0test", VariableValueType.Integer);
            graph.AddVertex("RUN", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidStateVariableNameValidationError(stateVariable)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_ReservedKeywordStateVariableInvalidTest()
        {
            Graph graph = new Graph();
            StateVariable stateVariable = graph.AddStateVariable("new", VariableValueType.Integer);
            graph.AddVertex("RUN", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new ReservedKeywordStateVariableValidationError(stateVariable)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_DuplicateStateVariableNamesInvalidTest()
        {
            Graph graph = new Graph();
            StateVariable stateVariable1 = graph.AddStateVariable("test", VariableValueType.Integer);
            StateVariable stateVariable2 = graph.AddStateVariable("test", VariableValueType.Integer);
            graph.AddVertex("RUN", true);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new DuplicateStateVariableNamesValidationError(new List<StateVariable>(){ stateVariable1, stateVariable2 })
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_ParametersRequiredEdgeInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test", VariableValueType.Integer);
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            vertex2.AddParameter("test");

            Edge edge = graph.AddEdge(vertex1, vertex2);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new ParametersRequiredEdgeValidationError(edge)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidParametersEdgeInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test1", VariableValueType.Integer);
            graph.AddStateVariable("test2", VariableValueType.Integer);
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            vertex2.AddParameter("test1");
            vertex2.AddParameter("test2");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.AddParameter("0");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidParametersEdgeValidationError(edge)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        private static void Validator_ValidTest(Graph graph)
        {
            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(graph);

            TraceValidationErrors(validationErrors);

            Assert.AreEqual(0, validationErrors.Count);
        }

        private static void Validator_InvalidTest(Graph graph, IReadOnlyList<ValidationError> expectedErrors = null)
        {
            if (null != expectedErrors)
            {
                Trace.WriteLine($"Expected errors:");
                TraceValidationErrors(expectedErrors);
            }

            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(graph);

            Trace.WriteLine($"Actual errors:");
            TraceValidationErrors(validationErrors);

            if (null == expectedErrors)
            {
                Assert.AreNotEqual(0, validationErrors.Count);
            }
            else
            {
                Assert.AreEqual(expectedErrors.Count, validationErrors.Count);

                for (int i = 0; i < expectedErrors.Count; i++)
                {
                    Assert.AreEqual(expectedErrors[i].GetType(), validationErrors[i].GetType());
                }
            }
        }

        private static void TraceValidationErrors(IReadOnlyList<ValidationError> validationErrors)
        {
            if (validationErrors.Count > 0)
            {
                Trace.Indent();

                foreach (var error in validationErrors)
                {
                    Trace.WriteLine(error.Message);
                }

                Trace.Unindent();
            }
        }
    }
}
