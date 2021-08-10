// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace TEGS
{
    public class Graph : IComparable<Graph>, IEquatable<Graph>, ICloneable<Graph>
    {
        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value?.Trim() ?? "";
            }
        }
        private string _name = "";

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value?.Trim() ?? "";
            }
        }
        private string _description = "";

        public readonly List<StateVariable> StateVariables = new List<StateVariable>();

        public Vertex StartingVertex
        {
            get
            {
                foreach (Vertex v in Verticies)
                {
                    if (v.IsStartingVertex)
                    {
                        return v;
                    }
                }

                return null;
            }
        }

        public readonly List<Vertex> Verticies = new List<Vertex>();

        public readonly List<Edge> Edges = new List<Edge>();

        #endregion

        #region Variables

        public StateVariable AddStateVariable(string name, VariableValueType type, string description = "")
        {
            StateVariable item = new StateVariable()
            {
                Name = name,
                Type = type,
                Description = description,
            };

            StateVariables.Add(item);

            return item;
        }

        public bool HasStateVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (StateVariable sv in StateVariables)
            {
                if (sv.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public StateVariable GetStateVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (StateVariable sv in StateVariables)
            {
                if (sv.Name == name)
                {
                    return sv;
                }
            }

            throw new StateVariableNotFoundException(name);
        }

        #endregion

        #region Verticies

        public Vertex AddVertex(string name, bool isStarting = false)
        {
            Vertex item = new Vertex()
            {
                Name = name,
                IsStartingVertex = isStarting,
            };

            Verticies.Add(item);

            return item;
        }

        #endregion

        #region Edges

        public Edge AddEdge(Vertex source, Vertex target)
        {
            Edge item = new Edge()
            {
                Source = source,
                Target = target,
            };
            Edges.Add(item);
            return item;
        }

        #endregion

        #region Load / Save

        public static Graph Load(Stream inputStream)
        {
            if (inputStream is null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            Graph graph = new Graph();

            Dictionary<int, Vertex> verticiesMap = new Dictionary<int, Vertex>();
            Dictionary<int, Edge> edgesMap = new Dictionary<int, Edge>();
            Dictionary<int, Tuple<int, int>> edgeToVerticiesMap = new Dictionary<int, Tuple<int, int>>();

            var options = new JsonDocumentOptions()
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            };

            var doc = JsonDocument.Parse(inputStream, options);

            graph.Name = doc.RootElement.TryGetProperty("name", out var graphName) ? graphName.GetString() : "";

            graph.Description = doc.RootElement.TryGetProperty("description", out var graphDescription) ? graphDescription.GetString() : "";

            if (doc.RootElement.TryGetProperty("variables", out var variableArray) && variableArray.ValueKind != JsonValueKind.Null)
            {
                foreach (var variable in variableArray.EnumerateArray())
                {
                    var name = variable.GetProperty("name").GetString();
                    var type = Enum.Parse<VariableValueType>(variable.GetProperty("type").GetString());
                    var description = variable.TryGetProperty("description", out var varDescription) ? varDescription.GetString() : "";
                    graph.AddStateVariable(name, type, description);
                }
            }

            if (doc.RootElement.TryGetProperty("verticies", out var vertexArray) && vertexArray.ValueKind != JsonValueKind.Null)
            {
                foreach (var vertex in vertexArray.EnumerateArray())
                {
                    var id = vertex.GetProperty("id").GetInt32();

                    var v = new Vertex();

                    if (vertex.TryGetProperty("name", out var vertexName))
                    {
                        v.Name = vertexName.GetString();
                    }

                    if (vertex.TryGetProperty("description", out var vertexDescription))
                    {
                        v.Description = vertexDescription.GetString();
                    }

                    if (vertex.TryGetProperty("starting", out var vertexStarting))
                    {
                        v.IsStartingVertex = vertexStarting.GetBoolean();
                    }

                    if (vertex.TryGetProperty("x", out var vertexX))
                    {
                        v.X = vertexX.GetInt32();
                    }

                    if (vertex.TryGetProperty("y", out var vertexY))
                    {
                        v.Y = vertexY.GetInt32();
                    }

                    if (vertex.TryGetProperty("code", out var vertexCodeArray) && vertexCodeArray.ValueKind != JsonValueKind.Null)
                    {
                        var code = new List<string>();
                        foreach (var codeLine in vertexCodeArray.EnumerateArray())
                        {
                            code.Add(codeLine.GetString());
                        }
                        v.Code = code.ToArray();
                    }

                    if (vertex.TryGetProperty("parameters", out var parametersArray) && parametersArray.ValueKind != JsonValueKind.Null)
                    {
                        foreach (var parameter in parametersArray.EnumerateArray())
                        {
                            v.ParameterNames.Add(parameter.GetString());
                        }
                    }

                    verticiesMap.Add(id, v);
                }
            }

            if (doc.RootElement.TryGetProperty("edges", out var edgeArray) && edgeArray.ValueKind != JsonValueKind.Null)
            {
                foreach (var edge in edgeArray.EnumerateArray())
                {
                    var id = edge.GetProperty("id").GetInt32();

                    var e = new Edge();

                    var sourceId = edge.TryGetProperty("source", out var edgeSource) ? edgeSource.GetInt32() : -1;
                    var targetId = edge.TryGetProperty("target", out var edgeTarget) ? edgeTarget.GetInt32() : -1;

                    if (edge.TryGetProperty("action", out var edgeAction))
                    {
                        e.Action = Enum.Parse<EdgeAction>(edgeAction.GetString());
                    }

                    if (edge.TryGetProperty("description", out var edgeDescription))
                    {
                        e.Description = edgeDescription.GetString();
                    }

                    if (edge.TryGetProperty("condition", out var edgeCondition))
                    {
                        e.Condition = edgeCondition.GetString();
                    }

                    if (edge.TryGetProperty("delay", out var edgeDelay))
                    {
                        e.Delay = edgeDelay.GetString();
                    }

                    if (edge.TryGetProperty("priority", out var edgePriority))
                    {
                        e.Priority = edgePriority.GetString();
                    }

                    if (edge.TryGetProperty("parameters", out var parametersArray) && parametersArray.ValueKind != JsonValueKind.Null)
                    {
                        foreach (var parameter in parametersArray.EnumerateArray())
                        {
                            e.ParameterExpressions.Add(parameter.GetString());
                        }
                    }

                    edgesMap.Add(id, e);
                    edgeToVerticiesMap.Add(id, new Tuple<int, int>(sourceId, targetId));
                }
            }

            // Set the sources and targets if possible
            foreach (var kvp in edgeToVerticiesMap)
            {
                edgesMap[kvp.Key].Source = verticiesMap.TryGetValue(kvp.Value.Item1, out Vertex source) ? source : null;
                edgesMap[kvp.Key].Target = verticiesMap.TryGetValue(kvp.Value.Item2, out Vertex target) ? target : null;
            }

            foreach (var kvp in verticiesMap.OrderBy(kvp => kvp.Key))
            {
                graph.Verticies.Add(kvp.Value);
            }

            foreach (var kvp in edgesMap.OrderBy(kvp => kvp.Key))
            {
                graph.Edges.Add(kvp.Value);
            }

            return graph;
        }

        public void Save(Stream outputStream)
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            var options = new JsonWriterOptions()
            {
                Indented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            using var jsonWriter = new Utf8JsonWriter(outputStream, options);

            jsonWriter.WriteStartObject();

            jsonWriter.WriteString("name", Name);
            jsonWriter.WriteString("description", Description);

            jsonWriter.WriteStartArray("variables");

            for (int i = 0; i < StateVariables.Count; i++)
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteString("name", StateVariables[i].Name);
                jsonWriter.WriteString("type", StateVariables[i].Type.ToString());
                jsonWriter.WriteString("description", StateVariables[i].Description);

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray(); // variables

            jsonWriter.WriteStartArray("verticies");

            for (int i = 0; i < Verticies.Count; i++)
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteNumber("id", i);
                jsonWriter.WriteString("name", Verticies[i].Name);
                jsonWriter.WriteString("description", Verticies[i].Description);

                if (Verticies[i].Code is null)
                {
                    jsonWriter.WriteNull("code");
                }
                else
                {
                    jsonWriter.WriteStartArray("code");

                    for (int j = 0; j < Verticies[i].Code.Length; j++)
                    {
                        jsonWriter.WriteStringValue(Verticies[i].Code[j]);
                    }
                    jsonWriter.WriteEndArray();
                }

                jsonWriter.WriteNumber("x", Verticies[i].X);
                jsonWriter.WriteNumber("y", Verticies[i].Y);

                if (Verticies[i].IsStartingVertex)
                {
                    jsonWriter.WriteBoolean("starting", Verticies[i].IsStartingVertex);
                }

                if (Verticies[i].ParameterNames.Count > 0)
                {
                    jsonWriter.WriteStartArray("parameters");
                    for (int j = 0; j < Verticies[i].ParameterNames.Count; j++)
                    {
                        jsonWriter.WriteStringValue(Verticies[i].ParameterNames[j]);
                    }
                    jsonWriter.WriteEndArray();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray();

            jsonWriter.WriteStartArray("edges");

            for (int i = 0; i < Edges.Count; i++)
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteNumber("id", i);

                jsonWriter.WriteNumber("source", Verticies.IndexOf(Edges[i].Source));
                jsonWriter.WriteNumber("target", Verticies.IndexOf(Edges[i].Target));

                jsonWriter.WriteString("action", Edges[i].Action.ToString());

                jsonWriter.WriteString("description", Edges[i].Description);
                jsonWriter.WriteString("condition", Edges[i].Condition);
                jsonWriter.WriteString("delay", Edges[i].Delay);
                jsonWriter.WriteString("priority", Edges[i].Priority);

                if (Edges[i].ParameterExpressions.Count > 0)
                {
                    jsonWriter.WriteStartArray("parameters");
                    for (int j = 0; j < Edges[i].ParameterExpressions.Count; j++)
                    {
                        jsonWriter.WriteStringValue(Edges[i].ParameterExpressions[j]);
                    }
                    jsonWriter.WriteEndArray();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray(); // edges

            jsonWriter.WriteEndObject(); // graph
        }

        #endregion

        public Graph Clone()
        {
            Graph clone = new Graph()
            {
                Name = Name,
                Description = Description,
            };

            foreach (var stateVariable in StateVariables)
            {
                clone.StateVariables.Add(stateVariable.Clone());
            }

            foreach (var vertex in Verticies)
            {
                clone.Verticies.Add(vertex.Clone());
            }

            foreach (var edge in Edges)
            {
                var clonedEdge = edge.Clone();

                int sourceId = Verticies.IndexOf(edge.Source);
                clonedEdge.Source = sourceId > 0 ? clone.Verticies[sourceId] : null;

                int targetId = Verticies.IndexOf(edge.Target);
                clonedEdge.Target = targetId > 0 ? clone.Verticies[targetId] : null;

                clone.Edges.Add(clonedEdge);
            }

            return clone;
        }

        public bool Equals(Graph other)
        {
            if (null == other)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return Name == other.Name
                && Description == other.Description
                && StateVariables.EqualItems(other.StateVariables);
        }

        public int CompareTo(Graph other)
        {
            return Name.CompareTo(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Graph);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                return Name;
            }

            return base.ToString();
        }
    }
}
