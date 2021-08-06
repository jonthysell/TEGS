﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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

        #region XML

        public static Graph LoadXml(Stream inputStream)
        {
            if (null == inputStream)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            Graph graph = new Graph();

            Dictionary<int, Vertex> verticies = new Dictionary<int, Vertex>();
            Dictionary<int, Edge> edges = new Dictionary<int, Edge>();
            Dictionary<int, Tuple<int, int>> edgeToVerticies = new Dictionary<int, Tuple<int, int>>();

            object lastItem = null;

            using (XmlReader xmlReader = XmlReader.Create(inputStream))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        if (xmlReader.Name == "graph")
                        {
                            graph.Name = xmlReader.GetAttribute("name");
                            graph.Description = xmlReader.GetAttribute("description");
                        }
                        else if (xmlReader.Name == "variable")
                        {
                            string name = xmlReader.GetAttribute("name");
                            VariableValueType type = (VariableValueType)Enum.Parse(typeof(VariableValueType), xmlReader.GetAttribute("type"));
                            string description = xmlReader.GetAttribute("description");

                            graph.AddStateVariable(name, type, description);
                        }
                        else if (xmlReader.Name == "vertex")
                        {
                            int id = int.Parse(xmlReader.GetAttribute("id"));
                            string name = xmlReader.GetAttribute("name");

                            Vertex vertex = new Vertex()
                            {
                                Name = name,
                                IsStartingVertex = bool.TryParse(xmlReader.GetAttribute("starting"), out bool isStartingVertex) && isStartingVertex,
                                Description = xmlReader.GetAttribute("description"),
                            };

                            vertex.SetCode(xmlReader.GetAttribute("code"));

                            if (int.TryParse(xmlReader.GetAttribute("x"), out int x))
                            {
                                vertex.X = x;
                            }

                            if (int.TryParse(xmlReader.GetAttribute("y"), out int y))
                            {
                                vertex.Y = y;
                            }

                            verticies.Add(id, vertex);

                            lastItem = vertex;
                        }
                        else if (xmlReader.Name == "parameter" && lastItem is Vertex lastVertex)
                        {
                            string name = xmlReader.GetAttribute("name");
                            lastVertex.ParameterNames.Add(name);
                        }
                        else if (xmlReader.Name == "edge")
                        {
                            int id = int.Parse(xmlReader.GetAttribute("id"));

                            int sourceId = int.Parse(xmlReader.GetAttribute("source"));
                            int targetId = int.Parse(xmlReader.GetAttribute("target"));

                            Edge edge = new Edge()
                            {
                                Action = (EdgeAction)Enum.Parse(typeof(EdgeAction), xmlReader.GetAttribute("action")),

                                Description = xmlReader.GetAttribute("description"),
                                Condition = xmlReader.GetAttribute("condition"),
                                Delay = xmlReader.GetAttribute("delay"),
                                Priority = xmlReader.GetAttribute("priority")
                            };

                            edges.Add(id, edge);
                            edgeToVerticies.Add(id, new Tuple<int, int>(sourceId, targetId));

                            lastItem = edge;
                        }
                        else if (xmlReader.Name == "parameter" && lastItem is Edge lastEdge)
                        {
                            string expression = xmlReader.GetAttribute("expression");
                            lastEdge.ParameterExpressions.Add(expression);
                        }
                        else
                        {
                            lastItem = null;
                        }
                    }
                }
            }

            // Set the sources and targets if possible
            foreach (var kvp in edgeToVerticies)
            {
                edges[kvp.Key].Source = verticies.TryGetValue(kvp.Value.Item1, out Vertex source) ? source : null;
                edges[kvp.Key].Target = verticies.TryGetValue(kvp.Value.Item2, out Vertex target) ? target : null;
            }

            foreach (var kvp in verticies.OrderBy(kvp => kvp.Key))
            {
                graph.Verticies.Add(kvp.Value);
            }

            foreach (var kvp in edges.OrderBy(kvp => kvp.Key))
            {
                graph.Edges.Add(kvp.Value);
            }

            return graph;
        }

        public void SaveXml(Stream outputStream)
        {
            if (null == outputStream)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            XmlWriterSettings outputSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                Indent = true,
            };

            using XmlWriter xmlWriter = XmlWriter.Create(outputStream, outputSettings);

            xmlWriter.WriteComment($" Created with { AppInfo.Name } v{ AppInfo.Version }. ");

            xmlWriter.WriteStartElement("graph");
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("description", Description);

            xmlWriter.WriteStartElement("variables");

            foreach (StateVariable stateVariable in StateVariables)
            {
                xmlWriter.WriteStartElement("variable");

                xmlWriter.WriteAttributeString("name", stateVariable.Name);
                xmlWriter.WriteAttributeString("type", stateVariable.Type.ToString());
                xmlWriter.WriteAttributeString("description", stateVariable.Description.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // variables

            xmlWriter.WriteStartElement("verticies");

            for (int i = 0; i < Verticies.Count; i++)
            {
                xmlWriter.WriteStartElement("vertex");

                xmlWriter.WriteAttributeString("id", i.ToString());
                xmlWriter.WriteAttributeString("name", Verticies[i].Name);
                xmlWriter.WriteAttributeString("description", Verticies[i].Description);
                xmlWriter.WriteAttributeString("code", Verticies[i].GetCode());

                xmlWriter.WriteAttributeString("x", Verticies[i].X.ToString());
                xmlWriter.WriteAttributeString("y", Verticies[i].Y.ToString());

                if (Verticies[i].IsStartingVertex)
                {
                    xmlWriter.WriteAttributeString("starting", Verticies[i].IsStartingVertex.ToString());
                }

                if (Verticies[i].ParameterNames.Count > 0)
                {
                    for (int j = 0; j < Verticies[i].ParameterNames.Count; j++)
                    {
                        xmlWriter.WriteStartElement("parameter");
                        xmlWriter.WriteAttributeString("name", Verticies[i].ParameterNames[j]);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // verticies

            xmlWriter.WriteStartElement("edges");

            for (int i = 0; i < Edges.Count; i++)
            {
                xmlWriter.WriteStartElement("edge");

                xmlWriter.WriteAttributeString("id", i.ToString());

                xmlWriter.WriteAttributeString("source", Verticies.IndexOf(Edges[i].Source).ToString());
                xmlWriter.WriteAttributeString("target", Verticies.IndexOf(Edges[i].Target).ToString());

                xmlWriter.WriteAttributeString("action", Edges[i].Action.ToString());

                xmlWriter.WriteAttributeString("description", Edges[i].Description);
                xmlWriter.WriteAttributeString("condition", Edges[i].Condition);
                xmlWriter.WriteAttributeString("delay", Edges[i].Delay);
                xmlWriter.WriteAttributeString("priority", Edges[i].Priority);

                if (Edges[i].ParameterExpressions.Count > 0)
                {
                    for (int j = 0; j < Edges[i].ParameterExpressions.Count; j++)
                    {
                        xmlWriter.WriteStartElement("parameter");
                        xmlWriter.WriteAttributeString("expression", Edges[i].ParameterExpressions[j]);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // edges

            xmlWriter.WriteEndElement(); // graph
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
