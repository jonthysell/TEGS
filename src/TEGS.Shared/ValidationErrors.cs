// 
// ValidationErrors.cs
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

namespace TEGS
{
    public abstract class ValidationError
    {
        public abstract string Message { get; }
    }

    public class NoStartingVertexValidationError : ValidationError
    {
        public override string Message => "Graph has no starting vertex.";
    }

    #region State Variable Validation Errors

    public abstract class StateVariableValidationError : ValidationError
    {
        public readonly StateVariable StateVariable;

        public StateVariableValidationError(StateVariable stateVariable)
        {
            StateVariable = stateVariable ?? throw new ArgumentNullException(nameof(stateVariable));
        }
    }

    public class BlankStateVariableNameValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable has a blank name.";

        public BlankStateVariableNameValidationError(StateVariable stateVariable) : base(stateVariable) { }
    }

    public class InvalidStateVariableNameValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable cannot be named \"{StateVariable.Name}\".";

        public InvalidStateVariableNameValidationError(StateVariable stateVariable) : base(stateVariable) { }
    }

    public class ReservedKeywordStateVariableValidationError : StateVariableValidationError
    {
        public override string Message => $"StateVariable cannot be named after reserved keyword \"{StateVariable.Name}\".";

        public ReservedKeywordStateVariableValidationError(StateVariable stateVariable) : base(stateVariable) { }
    }

    #endregion

    #region State Variables Validation Errors

    public abstract class StateVariablesValidationError : ValidationError
    {
        public readonly IReadOnlyList<StateVariable> StateVariables;

        public StateVariablesValidationError(IReadOnlyList<StateVariable> stateVariables)
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

        public DuplicateStateVariableNamesValidationError(IReadOnlyList<StateVariable> stateVariables) : base(stateVariables) { }
    }

    #endregion

    #region Vertex Validation Errors

    public abstract class VertexValidationError : ValidationError
    {
        public readonly Vertex Vertex;

        public VertexValidationError(Vertex vertex)
        {
            Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
        }
    }

    public class BlankVertexNameValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Vertex.Id} has a blank name.";

        public BlankVertexNameValidationError(Vertex vertex) : base(vertex) { }
    }

    public class InvalidParameterNameVertexValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Vertex.Id} has invalid parameter name \"{ParameterName}\".";

        public readonly string ParameterName;

        public InvalidParameterNameVertexValidationError(Vertex vertex, string parameterName) : base(vertex)
        {
            ParameterName = parameterName;
        }
    }

    #endregion

    #region Verticies Validation Errors

    public abstract class VerticiesValidationError : ValidationError
    {
        public readonly IReadOnlyList<Vertex> Verticies;

        public VerticiesValidationError(IReadOnlyList<Vertex> verticies)
        {
            Verticies = verticies ?? throw new ArgumentNullException(nameof(verticies));
        }

        protected string[] GetNames()
        {
            List<string> names = new List<string>();

            foreach (Vertex v in Verticies)
            {
                names.Add(v.Name);
            }

            return names.ToArray();
        }
    }

    public class DuplicateVertexNamesValidationError : VerticiesValidationError
    {
        public override string Message
        {
            get
            {
                string[] names = GetNames();

                return $"Graph has {names.Length} verticies named \"{names[0]}\".";
            }
        }

        public DuplicateVertexNamesValidationError(IReadOnlyList<Vertex> verticies) : base(verticies) { }
    }

    public class MultipleStartingVertexValidationError : VerticiesValidationError
    {
        public override string Message
        {
            get
            {
                string[] names = GetNames();

                return $"Graph has {names.Length} starting verticies: \"{string.Join("\", \"", names)}\".";
            }
        }

        public MultipleStartingVertexValidationError(IReadOnlyList<Vertex> verticies) : base(verticies) { }
    }

    #endregion

    #region Edge Validation Errors

    public abstract class EdgeValidationError : ValidationError
    {
        public readonly Edge Edge;

        public EdgeValidationError(Edge edge)
        {
            Edge = edge ?? throw new ArgumentNullException(nameof(edge));
        }
    }

    public class ParametersRequiredEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Edge.Id} from \"{Edge.Source.Name}\" to \"{Edge.Target.Name}\" requires parameters.";

        public ParametersRequiredEdgeValidationError(Edge edge) : base(edge) { }
    }

    public class InvalidParametersEdgeValidationError : EdgeValidationError
    {
        public override string Message => $"Edge #{Edge.Id} from \"{Edge.Source.Name}\" to \"{Edge.Target.Name}\" has invalid parameters.";

        public InvalidParametersEdgeValidationError(Edge edge) : base(edge) { }
    }

    #endregion
}
