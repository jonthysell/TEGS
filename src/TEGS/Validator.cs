// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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

            // Verify state variables

            Dictionary<string, List<StateVariable>> uniqueStateVariableNames= new Dictionary<string, List<StateVariable>>();

            HashSet<StateVariable> badStateVariables = new HashSet<StateVariable>();

            foreach (StateVariable sv in graph.StateVariables)
            {
                // Verify name
                if (string.IsNullOrWhiteSpace(sv.Name))
                {
                    errors.Add(new BlankStateVariableNameValidationError(graph, sv));
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
                        errors.Add(new ReservedKeywordStateVariableValidationError(graph, sv));
                        badStateVariables.Add(sv);
                    }
                    else if (!ScriptingHost.IsValidSymbolName(sv.Name, false))
                    {
                        errors.Add(new InvalidStateVariableNameValidationError(graph, sv));
                        badStateVariables.Add(sv);
                    }
                }
            }

            foreach (var kvp in uniqueStateVariableNames)
            {
                if (kvp.Value.Count > 1)
                {
                    errors.Add(new DuplicateStateVariableNamesValidationError(graph, kvp.Value));
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
                    errors.Add(new BlankVertexNameValidationError(graph, v));
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
                            errors.Add(new InvalidParameterNameVertexValidationError(graph, v, parameterName));
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
                            errors.Add(new InvalidCodeVertexValidationError(graph, v, code[i], ex.Message));
                        }
                    }
                }
            }

            if (startingVerticies.Count == 0)
            {
                errors.Add(new NoStartingVertexValidationError(graph));
            }
            else if (startingVerticies.Count > 1)
            {
                errors.Add(new MultipleStartingVertexValidationError(graph, startingVerticies));
            }

            foreach (var kvp in uniqueVertexNames)
            {
                if (kvp.Value.Count > 1)
                {
                    errors.Add(new DuplicateVertexNamesValidationError(graph, kvp.Value));
                }
            }

            // Verify edges
            foreach (Edge e in graph.Edges)
            {
                bool validVerticies = true;
                if (e.Source is null)
                {
                    errors.Add(new SourceMissingEdgeValidationError(graph, e));
                    validVerticies = false;
                }

                if (e.Target is null)
                {
                    errors.Add(new TargetMissingEdgeValidationError(graph, e));
                    validVerticies = false;
                }

                if (validVerticies)
                {
                    if (e.Action == EdgeAction.Schedule)
                    {
                        var parameterNames = e.Target.ParameterNames;
                        var parameterExpressions = e.ParameterExpressions;

                        if (parameterNames.Count > 0)
                        {
                            if (parameterExpressions.Count == 0)
                            {
                                errors.Add(new ParametersRequiredEdgeValidationError(graph, e));
                            }
                            else if (parameterExpressions.Count != e.Target.ParameterNames.Count)
                            {
                                errors.Add(new InvalidParametersEdgeValidationError(graph, e));
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
                                    errors.Add(new InvalidParameterEdgeValidationError(graph, e, parameterExpressions[i], ex.Message));
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
                        errors.Add(new InvalidConditionEdgeValidationError(graph, e, ex.Message));
                    }

                    // Verify delay
                    try
                    {
                        scriptingHost.Evaluate(e.Delay, new VariableValue(Schedule.DefaultDelay)).AsNumber();
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new InvalidDelayEdgeValidationError(graph, e, ex.Message));
                    }

                    // Verify priority
                    try
                    {
                        scriptingHost.Evaluate(e.Priority, new VariableValue(Schedule.DefaultPriority)).AsNumber();
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new InvalidPriorityEdgeValidationError(graph, e, ex.Message));
                    }
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
