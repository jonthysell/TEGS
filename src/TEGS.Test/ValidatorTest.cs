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
        public void Validator_CarwashFileValidTest()
        {
            Validator_ValidTest(TestGraph.Load("carwash.json"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validator_NullGraphInvalidTest()
        {
            Validator.Validate((Graph)null);
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
        public void Validator_SourceMissingEdgeInvalidTest()
        {
            Graph graph = new Graph();
            _ = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            Edge edge = graph.AddEdge(null, vertex2);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new SourceMissingEdgeValidationError(graph, edge)
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_TargetMissingEdgeInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            _ = graph.AddVertex("RUN2");

            Edge edge = graph.AddEdge(vertex1, null);

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new TargetMissingEdgeValidationError(graph, edge)
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

        [TestMethod]
        public void Validator_InvalidParameterEdgeInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test1", VariableValueType.Integer);
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            vertex2.ParameterNames.Add("test1");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.ParameterExpressions.Add("hello");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidParameterEdgeValidationError(graph, edge, "test1", "")
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidConditionEdgeInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.Condition = "test";

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidConditionEdgeValidationError(graph, edge, "")
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidDelayEdgeInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.Delay = "test";

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidDelayEdgeValidationError(graph, edge, "")
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidPriorityEdgeInvalidTest()
        {
            Graph graph = new Graph();
            Vertex vertex1 = graph.AddVertex("RUN1", true);
            Vertex vertex2 = graph.AddVertex("RUN2");

            Edge edge = graph.AddEdge(vertex1, vertex2);
            edge.Priority = "test";

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidPriorityEdgeValidationError(graph, edge, "")
            };

            Validator_InvalidTest(graph, expectedErrors);
        }

        [TestMethod]
        public void Validator_CarwashSimulationValidTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash);

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Validator_ValidTest(s);
        }

        [TestMethod]
        public void Validator_BreakdownSimulationValidTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Breakdown);

            args.StartParameterExpressions.Add("0");

            Simulation s = new Simulation(args);
            Validator_ValidTest(s);

            Validator_ValidTest(TestGraph.Breakdown);
        }

        [TestMethod]
        public void Validator_CarwashFileSimulationValidTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Load("carwash.json"));

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Validator_ValidTest(s);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validator_NullSimulationInvalidTest()
        {
            Validator.Validate((Simulation)null);
        }

        [TestMethod]
        public void Validator_StartingParametersRequiredInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test1", VariableValueType.Integer);

            Vertex vertex1 = graph.AddVertex("RUN1", true);
            vertex1.ParameterNames.Add("test1");

            Simulation sim = new Simulation(new SimulationArgs(graph));

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new StartingParametersRequiredValidationError(sim)
            };

            Validator_InvalidTest(sim, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidStartingParametersInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test1", VariableValueType.Integer);
            graph.AddStateVariable("test2", VariableValueType.Integer);

            Vertex vertex1 = graph.AddVertex("RUN1", true);
            vertex1.ParameterNames.Add("test1");
            vertex1.ParameterNames.Add("test2");

            Simulation sim = new Simulation(new SimulationArgs(graph));

            sim.Args.StartParameterExpressions.Add("0");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidStartingParametersValidationError(sim)
            };

            Validator_InvalidTest(sim, expectedErrors);
        }

        [TestMethod]
        public void Validator_InvalidStartingParameterInvalidTest()
        {
            Graph graph = new Graph();
            graph.AddStateVariable("test1", VariableValueType.Integer);

            Vertex vertex1 = graph.AddVertex("RUN1", true);
            vertex1.ParameterNames.Add("test1");

            Simulation sim = new Simulation(new SimulationArgs(graph));

            sim.Args.StartParameterExpressions.Add("hello");

            List<ValidationError> expectedErrors = new List<ValidationError>
            {
                new InvalidStartingParameterValidationError(sim, "test1", "")
            };

            Validator_InvalidTest(sim, expectedErrors);
        }

        private static void Validator_ValidTest(Graph graph)
        {
            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(graph);

            TraceValidationErrors(validationErrors);

            Assert.AreEqual(0, validationErrors.Count);
        }

        private static void Validator_ValidTest(Simulation simulation)
        {
            Validator_ValidTest(simulation.Graph);

            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(simulation);

            TraceValidationErrors(validationErrors);

            Assert.AreEqual(0, validationErrors.Count);
        }

        private static void Validator_InvalidTest(Simulation simulation, IReadOnlyList<ValidationError> expectedErrors = null)
        {
            Validator_ValidTest(simulation.Graph);

            if (expectedErrors is not null)
            {
                Trace.WriteLine($"Expected errors:");
                TraceValidationErrors(expectedErrors);
            }

            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(simulation);

            Trace.WriteLine($"Actual errors:");
            TraceValidationErrors(validationErrors);

            if (expectedErrors is null)
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

        private static void Validator_InvalidTest(Graph graph, IReadOnlyList<ValidationError> expectedErrors = null)
        {
            if (expectedErrors is not null)
            {
                Trace.WriteLine($"Expected errors:");
                TraceValidationErrors(expectedErrors);
            }

            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(graph);

            Trace.WriteLine($"Actual errors:");
            TraceValidationErrors(validationErrors);

            if (expectedErrors is null)
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
