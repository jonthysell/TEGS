﻿// 
// Graph.cs
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TEGS
{
    public class Graph
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

        public IReadOnlyList<StateVariable> StateVariables => _stateVariables;
        private readonly List<StateVariable> _stateVariables = new List<StateVariable>();

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

        public IReadOnlyList<Vertex> Verticies => _verticies;
        private readonly List<Vertex> _verticies = new List<Vertex>();

        public IReadOnlyList<Edge> Edges => _edges;
        private readonly List<Edge> _edges = new List<Edge>();

        #endregion

        #region Variables

        public StateVariable AddStateVariable(string name, VariableValueType type)
        {
            StateVariable item = new StateVariable(name, type);
            AddStateVariable(item);
            return item;
        }

        private void AddStateVariable(StateVariable item)
        {
            _stateVariables.Add(item);
        }

        public bool RemoveStateVariable(StateVariable item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _stateVariables.Remove(item);
        }

        public bool HasStateVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (StateVariable sv in _stateVariables)
            {
                if (sv.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStateVariable(string name, VariableValueType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (StateVariable sv in _stateVariables)
            {
                if (sv.Name == name & sv.Type == type)
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

            foreach (StateVariable sv in _stateVariables)
            {
                if (sv.Name == name)
                {
                    return sv;
                }
            }

            throw new StateVariableNotFoundException(name);
        }

        public StateVariable GetStateVariable(string name, VariableValueType type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (StateVariable sv in _stateVariables)
            {
                if (sv.Name == name && sv.Type == type)
                {
                    return sv;
                }
            }

            throw new StateVariableNotFoundException(name, type);
        }

        #endregion

        #region Verticies

        public Vertex AddVertex(string name, bool isStarting = false)
        {
            Vertex item = new Vertex(this, name, isStarting);
            AddVertex(item);
            return item;
        }

        private void AddVertex(Vertex item)
        {
            _verticies.Add(item);
        }

        public bool RemoveVertex(Vertex item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _edges.RemoveAll(edge => (edge.Source == item || edge.Target == item));

            return _verticies.Remove(item);
        }

        public bool HasVertex(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (Vertex v in _verticies)
            {
                if (v.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Edges

        public Edge AddEdge(Vertex source, Vertex target)
        {
            if (null == source)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (null == target)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Edge item = new Edge(this, source, target);
            _edges.Add(item);
            return item;
        }

        private void AddEdge(Edge item)
        {
            _edges.Add(item);
        }

        public bool RemoveEdge(Edge item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _edges.Remove(item);
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

                            graph.AddStateVariable(name, type);
                        }
                        else if (xmlReader.Name == "vertex")
                        {
                            int id = int.Parse(xmlReader.GetAttribute("id"));
                            string name = xmlReader.GetAttribute("name");

                            bool.TryParse(xmlReader.GetAttribute("starting"), out bool isStartingVertex);

                            Vertex vertex = new Vertex(graph, name, isStartingVertex)
                            {
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
                            lastVertex.AddParameter(name);
                        }
                        else if (xmlReader.Name == "edge")
                        {
                            int id = int.Parse(xmlReader.GetAttribute("id"));

                            int sourceId = int.Parse(xmlReader.GetAttribute("source"));
                            int targetId = int.Parse(xmlReader.GetAttribute("target"));

                            Edge edge = new Edge(graph, verticies[sourceId], verticies[targetId])
                            {
                                Action = (EdgeAction)Enum.Parse(typeof(EdgeAction), xmlReader.GetAttribute("action")),

                                Description = xmlReader.GetAttribute("description"),
                                Condition = xmlReader.GetAttribute("condition"),
                                Delay = xmlReader.GetAttribute("delay"),
                                Priority = xmlReader.GetAttribute("priority")
                            };

                            edges.Add(id, edge);

                            lastItem = edge;
                        }
                        else if (xmlReader.Name == "parameter" && lastItem is Edge lastEdge)
                        {
                            string expression = xmlReader.GetAttribute("expression");
                            lastEdge.AddParameter(expression);
                        }
                        else
                        {
                            lastItem = null;
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, Vertex> kvp in verticies)
            {
                graph.AddVertex(kvp.Value);
            }

            foreach (Edge edge in edges.Values)
            {
                graph.AddEdge(edge);
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
                NewLineHandling = NewLineHandling.Entitize
            };

            using XmlWriter xmlWriter = XmlWriter.Create(outputStream, outputSettings);

            xmlWriter.WriteStartElement("graph");
            xmlWriter.WriteAttributeString("name", Name);
            xmlWriter.WriteAttributeString("description", Description);

            xmlWriter.WriteStartElement("variables");

            foreach (StateVariable stateVariable in _stateVariables)
            {
                xmlWriter.WriteStartElement("variable");

                xmlWriter.WriteAttributeString("name", stateVariable.Name);
                xmlWriter.WriteAttributeString("type", stateVariable.Type.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // variables

            xmlWriter.WriteStartElement("verticies");

            for (int i = 0; i < _verticies.Count; i++)
            {
                xmlWriter.WriteStartElement("vertex");

                xmlWriter.WriteAttributeString("id", i.ToString());
                xmlWriter.WriteAttributeString("name", _verticies[i].Name);
                xmlWriter.WriteAttributeString("description", _verticies[i].Description);
                xmlWriter.WriteAttributeString("code", _verticies[i].GetCode());

                xmlWriter.WriteAttributeString("x", _verticies[i].X.ToString());
                xmlWriter.WriteAttributeString("y", _verticies[i].Y.ToString());

                if (_verticies[i].IsStartingVertex)
                {
                    xmlWriter.WriteAttributeString("starting", _verticies[i].IsStartingVertex.ToString());
                }

                if (_verticies[i].ParameterNames.Count > 0)
                {
                    for (int j = 0; j < _verticies[i].ParameterNames.Count; j++)
                    {
                        xmlWriter.WriteStartElement("parameter");
                        xmlWriter.WriteAttributeString("name", _verticies[i].ParameterNames[j]);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // verticies

            xmlWriter.WriteStartElement("edges");

            for (int i = 0; i < _edges.Count; i++)
            {
                xmlWriter.WriteStartElement("edge");

                xmlWriter.WriteAttributeString("id", i.ToString());

                xmlWriter.WriteAttributeString("source", _verticies.IndexOf(_edges[i].Source).ToString());
                xmlWriter.WriteAttributeString("target", _verticies.IndexOf(_edges[i].Target).ToString());

                xmlWriter.WriteAttributeString("action", _edges[i].Action.ToString());

                xmlWriter.WriteAttributeString("description", _edges[i].Description);
                xmlWriter.WriteAttributeString("condition", _edges[i].Condition);
                xmlWriter.WriteAttributeString("delay", _edges[i].Delay);
                xmlWriter.WriteAttributeString("priority", _edges[i].Priority);

                if (_edges[i].ParameterExpressions.Count > 0)
                {
                    for (int j = 0; j < _edges[i].ParameterExpressions.Count; j++)
                    {
                        xmlWriter.WriteStartElement("parameter");
                        xmlWriter.WriteAttributeString("expression", _edges[i].ParameterExpressions[j]);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement(); // edges

            xmlWriter.WriteEndElement(); // graph
        }

        #endregion

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
