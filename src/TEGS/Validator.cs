// 
// Validator.cs
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

using System;
using System.Collections.Generic;

namespace TEGS
{
    public static class Validator
    {
        public static IReadOnlyList<ValidationError> Validate(Graph graph)
        {
            if (null == graph)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            List<ValidationError> errors = new List<ValidationError>();

            // Verify starting vertex
            if (graph.Verticies.Count > 0 && null == graph.StartingVertex)
            {
                errors.Add(new NoStartingVertexValidationError());
            }

            // Verify verticies
            HashSet<string> vertexNames = new HashSet<string>();

            foreach (Vertex v in graph.Verticies)
            {
                // Verify name
                if (string.IsNullOrWhiteSpace(v.Name))
                {
                    errors.Add(new BlankVertexNameValidationError(v));
                }
                else if (!vertexNames.Add(v.Name))
                {
                    errors.Add(new DuplicateVertexNameValidationError(v));
                }

                // Verify parameters
                string[] parameterNames = v.ParameterNames;

                if (null != parameterNames)
                {
                    foreach (string parameterName in parameterNames)
                    {
                        if (!graph.HasStateVariable(parameterName))
                        {
                            errors.Add(new InvalidParameterNameVertexValidationError(v, parameterName));
                        }
                    }
                }
            }

            return errors;
        }
    }

    public class ValidationException : Exception
    {
        public readonly IReadOnlyList<ValidationError> ValidationErrors;

        public ValidationException(IReadOnlyList<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }

    public abstract class ValidationError
    {
        public abstract string Message { get; }
    }

    public class NoStartingVertexValidationError : ValidationError
    {
        public override string Message => "Graph has no starting vertex.";
    }

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

    public class DuplicateVertexNameValidationError : VertexValidationError
    {
        public override string Message => $"Vertex #{Vertex.Id} uses existing name \"{Vertex.Name}\".";

        public DuplicateVertexNameValidationError(Vertex vertex) : base(vertex) { }
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
}
