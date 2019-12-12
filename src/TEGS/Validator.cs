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

            // Verify verticies
            List<Vertex> startingVerticies = new List<Vertex>();

            Dictionary<string, List<Vertex>> uniqueVertexNames = new Dictionary<string, List<Vertex>>();

            foreach (Vertex v in graph.Verticies)
            {
                if (v.IsStartingVertex)
                {
                    startingVerticies.Add(v);
                }

                // Verify name
                if (string.IsNullOrWhiteSpace(v.Name))
                {
                    errors.Add(new BlankVertexNameValidationError(v));
                }
                else
                {
                    if (!uniqueVertexNames.ContainsKey(v.Name))
                    {
                        uniqueVertexNames[v.Name] = new List<Vertex>();
                    }

                    uniqueVertexNames[v.Name].Add(v);
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

            if (startingVerticies.Count == 0)
            {
                errors.Add(new NoStartingVertexValidationError());
            }
            else if (startingVerticies.Count > 1)
            {
                errors.Add(new MultipleStartingVertexValidationError(startingVerticies));
            }

            foreach (var kvp in uniqueVertexNames)
            {
                if (kvp.Value.Count > 1)
                {
                    errors.Add(new DuplicateVertexNamesValidationError(kvp.Value));
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
}
