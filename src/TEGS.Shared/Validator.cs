// 
// Validator.cs
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

using TEGS.Expressions;
using TEGS.Libraries;

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

            // Verify state variables

            Dictionary<string, List<StateVariable>> uniqueStateVariableNames= new Dictionary<string, List<StateVariable>>();

            HashSet<StateVariable> badStateVariables = new HashSet<StateVariable>();

            foreach (StateVariable sv in graph.StateVariables)
            {
                // Verify name
                if (string.IsNullOrWhiteSpace(sv.Name))
                {
                    errors.Add(new BlankStateVariableNameValidationError(sv));
                    badStateVariables.Add(sv);
                }
                else
                {
                    if (!uniqueStateVariableNames.ContainsKey(sv.Name))
                    {
                        uniqueStateVariableNames[sv.Name] = new List<StateVariable>();
                    }

                    uniqueStateVariableNames[sv.Name].Add(sv);

                    if (ScriptingHost.ReservedKeywords.Contains(sv.Name))
                    {
                        errors.Add(new ReservedKeywordStateVariableValidationError(sv));
                        badStateVariables.Add(sv);
                    }
                    else if (!ScriptingHost.IsValidSymbolName(sv.Name, false))
                    {
                        errors.Add(new InvalidStateVariableNameValidationError(sv));
                        badStateVariables.Add(sv);
                    }
                }
            }

            foreach (var kvp in uniqueStateVariableNames)
            {
                if (kvp.Value.Count > 1)
                {
                    errors.Add(new DuplicateStateVariableNamesValidationError(kvp.Value));
                    foreach (StateVariable sv in kvp.Value)
                    {
                        badStateVariables.Add(sv);
                    }
                }
            }

            ScriptingHost scriptingHost = MakeValidationScriptingHost();

            foreach (StateVariable sv in graph.StateVariables)
            {
                if (!badStateVariables.Contains(sv))
                {
                    scriptingHost.Create(sv);
                }
            }

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
                IReadOnlyList<string> parameterNames = v.ParameterNames;

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

                // Verify code
                string[] code = v.Code;
                if (null != code && code.Length > 0)
                {
                    for (int i = 0; i < code.Length; i++)
                    {
                        try
                        {
                            scriptingHost.Execute(code[i]);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(new InvalidCodeVertexValidationError(v, code[i], ex.Message));
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

            // Verify edges
            foreach (Edge e in graph.Edges)
            {
                if (e.Action == EdgeAction.Schedule)
                {
                    var parameterNames = e.Target.ParameterNames;
                    var parameterExpressions = e.ParameterExpressions;

                    if (parameterNames.Count > 0)
                    {
                        if (parameterExpressions.Count == 0)
                        {
                            errors.Add(new ParametersRequiredEdgeValidationError(e));
                        }
                        else if (parameterExpressions.Count != e.Target.ParameterNames.Count)
                        {
                            errors.Add(new InvalidParametersEdgeValidationError(e));
                        }
                    }

                    if (parameterExpressions.Count > 0)
                    {
                        for (int i = 0; i < parameterExpressions.Count; i++)
                        {
                            try
                            {
                                var result = scriptingHost.Evaluate(parameterExpressions[i]);
                                scriptingHost.SetVariable(parameterNames[i], result);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(new InvalidParameterEdgeValidationError(e, parameterExpressions[i], ex.Message));
                            }
                        }
                    }
                }

                // Verify condition
                try
                {
                    scriptingHost.Evaluate(e.Condition, VariableValue.True).AsBoolean();
                }
                catch (Exception ex)
                {
                    errors.Add(new InvalidConditionEdgeValidationError(e, ex.Message));
                }

                // Verify delay
                try
                {
                    scriptingHost.Evaluate(e.Delay, new VariableValue(Schedule.DefaultDelay)).AsNumber();
                }
                catch (Exception ex)
                {
                    errors.Add(new InvalidDelayEdgeValidationError(e, ex.Message));
                }

                // Verify priority
                try
                {
                    scriptingHost.Evaluate(e.Priority, new VariableValue(Schedule.DefaultPriority)).AsNumber();
                }
                catch (Exception ex)
                {
                    errors.Add(new InvalidPriorityEdgeValidationError(e, ex.Message));
                }
            }

            return errors;
        }

        public static ScriptingHost MakeValidationScriptingHost()
        {
            ScriptingHost scriptingHost = BaseLibraries.MakeBaseScriptingHost();

            scriptingHost.DefineCustomFunction(nameof(Simulation.Clock), (args) => new VariableValue(0.0));
            scriptingHost.DefineCustomFunction(nameof(Simulation.EventCount), (args) => new VariableValue(0));

            return scriptingHost;
        }
    }

    #region Exceptions

    public class ValidationException : Exception
    {
        public readonly IReadOnlyList<ValidationError> ValidationErrors;

        public ValidationException(IReadOnlyList<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }

    #endregion
}
