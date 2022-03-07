// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS
{
    public abstract class ValidationError
    {
        public readonly Graph Graph;
        public abstract string Message { get; }

        public ValidationError(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
    }

    public class NoStartingVertexValidationError : ValidationError
    {
        public override string Message => "Graph has no starting vertex.";

        public NoStartingVertexValidationError(Graph graph) : base(graph) { }
    }

    #region State Variable Validation Errors

    public abstract class StateVariableValidationError : ValidationError
    {
        public readonly StateVariable StateVariable;

        public StateVariableValidationError(Graph graph, StateVariable stateVariable) : base(graph)
        {
            StateVariable = stateVariable ?? throw new ArgumentNullException(nameof(stateVariable));
        }
    }

    public class BlankStateVariableNameValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable has a blank name.";

        public BlankStateVariableNameValidationError(Graph graph, StateVariable stateVariable) : base(graph, stateVariable) { }
    }

    public class InvalidStateVariableNameValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable cannot be named \"{StateVariable.Name}\".";

        public InvalidStateVariableNameValidationError(Graph graph, StateVariable stateVariable) : base(graph, stateVariable) { }
    }

    public class ReservedKeywordStateVariableValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable cannot be named after reserved keyword \"{StateVariable.Name}\".";

        public ReservedKeywordStateVariableValidationError(Graph graph, StateVariable stateVariable) : base(graph, stateVariable) { }
    }

    #endregion

    #region State Variables Validation Errors

    public abstract class StateVariablesValidationError : ValidationError
    {
        public readonly IReadOnlyList<StateVariable> StateVariables;

        public StateVariablesValidationError(Graph graph, IReadOnlyList<StateVariable> stateVariables) : base(graph)
        {
            StateVariables = stateVariables ?? throw new ArgumentNullException(nameof(stateVariables));
        }

        protected string[] GetNames()
        {
            List<string> names = new List<string>();

            foreach (StateVariable sv in StateVariables)
            {
                names.Add(sv.Name);
            }

            return names.ToArray();
        }
    }

    public class DuplicateStateVariableNamesValidationError : StateVariablesValidationError
    {
        public override string Message
        {
            get
            {
                string[] names = GetNames();

                return $"Graph has {names.Length} state variables named \"{names[0]}\".";
            }
        }

        public DuplicateStateVariableNamesValidationError(Graph graph, IReadOnlyList<StateVariable> stateVariables) : base(graph, stateVariables) { }
    }

    #endregion

    #region Vertex Validation Errors

    public abstract class VertexValidationError : ValidationError
    {
        public readonly Vertex Vertex;

        public VertexValidationError(Graph graph, Vertex vertex) : base(graph)
        {
            Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
        }
    }

    public class BlankVertexNameValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Graph.Vertices.IndexOf(Vertex)} has a blank name.";

        public BlankVertexNameValidationError(Graph graph, Vertex vertex) : base(graph, vertex) { }
    }

    public class InvalidParameterNameVertexValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Graph.Vertices.IndexOf(Vertex)} has invalid parameter name \"{ParameterName}\".";

        public readonly string ParameterName;

        public InvalidParameterNameVertexValidationError(Graph graph, Vertex vertex, string parameterName) : base(graph, vertex)
        {
            ParameterName = parameterName;
        }
    }

    public class InvalidCodeVertexValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Graph.Vertices.IndexOf(Vertex)} has invalid code \"{Code}\": {Error}";

        public readonly string Code;

        public readonly string Error;

        public InvalidCodeVertexValidationError(Graph graph, Vertex vertex, string code, string error) : base(graph, vertex)
        {
            Code = code;
            Error = error;
        }
    }

    #endregion

    #region Vertices Validation Errors

    public abstract class VerticesValidationError : ValidationError
    {
        public readonly IReadOnlyList<Vertex> Vertices;

        public VerticesValidationError(Graph graph, IReadOnlyList<Vertex> vertices) : base(graph)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        }

        protected string[] GetNames()
        {
            List<string> names = new List<string>();

            foreach (Vertex v in Vertices)
            {
                names.Add(v.Name);
            }

            return names.ToArray();
        }
    }

    public class DuplicateVertexNamesValidationError : VerticesValidationError
    {
        public override string Message
        {
            get
            {
                string[] names = GetNames();

                return $"Graph has {names.Length} vertices named \"{names[0]}\".";
            }
        }

        public DuplicateVertexNamesValidationError(Graph graph, IReadOnlyList<Vertex> vertices) : base(graph, vertices) { }
    }

    public class MultipleStartingVertexValidationError : VerticesValidationError
    {
        public override string Message
        {
            get
            {
                string[] names = GetNames();

                return $"Graph has {names.Length} starting vertices: \"{string.Join("\", \"", names)}\".";
            }
        }

        public MultipleStartingVertexValidationError(Graph graph, IReadOnlyList<Vertex> vertices) : base(graph, vertices) { }
    }

    #endregion

    #region Edge Validation Errors

    public abstract class EdgeValidationError : ValidationError
    {
        public readonly Edge Edge;

        public EdgeValidationError(Graph graph, Edge edge) : base(graph)
        {
            Edge = edge ?? throw new ArgumentNullException(nameof(edge));
        }
    }

    public class SourceMissingEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} is missing a source vertex.";

        public SourceMissingEdgeValidationError(Graph graph, Edge edge) : base(graph, edge) { }
    }

    public class TargetMissingEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} is missing a target vertex.";

        public TargetMissingEdgeValidationError(Graph graph, Edge edge) : base(graph, edge) { }
    }

    public class ParametersRequiredEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" requires {Edge.Target?.ParameterNames.Count} parameters.";

        public ParametersRequiredEdgeValidationError(Graph graph, Edge edge) : base(graph, edge) { }
    }

    public class InvalidParametersEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" has an invalid number of parameters.";

        public InvalidParametersEdgeValidationError(Graph graph, Edge edge) : base(graph, edge) { }
    }

    public class InvalidParameterEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" has invalid parameter \"{Parameter}\": {Error}";

        public readonly string Parameter;

        public readonly string Error;

        public InvalidParameterEdgeValidationError(Graph graph, Edge edge, string parameter, string error) : base(graph, edge)
        {
            Parameter = parameter;
            Error = error;
        }
    }

    public class InvalidConditionEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" has invalid condition \"{Edge.Condition}\": {Error}";

        public readonly string Error;

        public InvalidConditionEdgeValidationError(Graph graph, Edge edge, string error) : base(graph, edge)
        {
            Error = error;
        }
    }

    public class InvalidDelayEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" has invalid delay \"{Edge.Delay}\": {Error}";

        public readonly string Error;

        public InvalidDelayEdgeValidationError(Graph graph, Edge edge, string error) : base(graph, edge)
        {
            Error = error;
        }
    }

    public class InvalidPriorityEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Graph.Edges.IndexOf(Edge)} from \"{Edge.Source?.Name}\" to \"{Edge.Target?.Name}\" has invalid priority \"{Edge.Priority}\": {Error}";

        public readonly string Error;

        public InvalidPriorityEdgeValidationError(Graph graph, Edge edge, string error) : base(graph, edge)
        {
            Error = error;
        }
    }

    #endregion

    #region Simulation Validation Errors

    public abstract class SimulationValidationError : ValidationError
    {
        public readonly Simulation Simulation;

        public SimulationValidationError(Simulation simulation) : base(simulation?.Graph)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }
    }

    public class StartingParametersRequiredValidationError : SimulationValidationError
    {
        public override string Message => $"Starting vertex #{Graph.Vertices.IndexOf(Graph.StartingVertex)} requires {Graph.StartingVertex.ParameterNames.Count} parameters to run a simulation.";

        public StartingParametersRequiredValidationError(Simulation simulation) : base(simulation) { }
    }

    public class InvalidStartingParametersValidationError : SimulationValidationError
    {
        public override string Message => $"Invalid number of starting parameters to run a simulation.";

        public InvalidStartingParametersValidationError(Simulation simulation) : base(simulation) { }
    }

    public class InvalidStartingParameterValidationError : SimulationValidationError
    {
        public override string Message => $"Invalid starting parameter \"{Parameter}\": {Error}";

        public readonly string Parameter;

        public readonly string Error;

        public InvalidStartingParameterValidationError(Simulation simulation, string parameter, string error) : base(simulation)
        {
            Parameter = parameter;
            Error = error;
        }
    }

    #endregion
}
