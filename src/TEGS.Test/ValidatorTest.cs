// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
                new NoStartingVertexValidationError(graph)
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
                new NoStartingVertexValidationError(graph)
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
                new BlankVertexNameValidationError(graph, vertex)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidParameterNameVertexInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex = graph.AddVertex("RUN", true);
            vertex.ParameterNames.Add("test");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidParameterNameVertexValidationError(graph, vertex, "test")
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
                new DuplicateVertexNamesValidationError(graph, new List<Vertex>() { vertex1, vertex2 })
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
                new MultipleStartingVertexValidationError(graph, new List<Vertex>() { vertex1, vertex2 })
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
                new BlankStateVariableNameValidationError(graph, stateVariable)
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
                new InvalidStateVariableNameValidationError(graph, stateVariable)
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
                new ReservedKeywordStateVariableValidationError(graph, stateVariable)
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
                new DuplicateStateVariableNamesValidationError(graph, new List<StateVariable>(){ stateVariable1, stateVariable2 })
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

            vertex2.ParameterNames.Add("test");

            Edge edge = graph.AddEdge(vertex1, vertex2);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new ParametersRequiredEdgeValidationError(graph, edge)
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

            vertex2.ParameterNames.Add("test1");
            vertex2.ParameterNames.Add("test2");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.ParameterExpressions.Add("0");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidParametersEdgeValidationError(graph, edge)
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
