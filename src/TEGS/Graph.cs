// 
// Graph.cs
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TEGS
{
    public class Graph
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value?.Trim();
            }
        }
        private string _name = null;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value?.Trim();
            }
        }
        private string _description = null;

        public Vertex StartingVertex
        {
            get
            {
                return _startingVertex;
            }
            set
            {
                if (null != value && !_verticies.Contains(value))
                {
                    throw new VertexNotInGraphException();
                }
                _startingVertex = value;
            }
        }
        private Vertex _startingVertex = null;

        public IEnumerable<Vertex> Verticies => _verticies;
        private List<Vertex> _verticies = new List<Vertex>();

        public int VertexCount => _verticies.Count;

        public IEnumerable<Edge> Edges => _edges;
        private List<Edge> _edges = new List<Edge>();

        public int EdgeCount => _edges.Count;

        public Vertex AddVertex(string name)
        {
            Vertex item = new Vertex(this, name);
            AddVertex(item);
            return item;
        }

        private void AddVertex(Vertex item)
        {
            _verticies.Add(item);
        }

        public bool RemoveVertex(Vertex item, bool removeEdges = false)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_edges.Exists(edge => (edge.Source == item || edge.Target == item)))
            {
                if (!removeEdges)
                {
                    throw new CannotRemoveVertexWithEdgesException();
                }

                _edges.RemoveAll(edge => (edge.Source == item || edge.Target == item));
            }

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

        public static Graph LoadXml(Stream inputStream)
        {
            if (null == inputStream)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            Graph graph = new Graph();

            Dictionary<int, Vertex> verticies = new Dictionary<int, Vertex>();
            Dictionary<int, Edge> edges = new Dictionary<int, Edge>();

            int? startingVertexId = null;

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
                        else if (xmlReader.Name == "vertex")
                        {
                            int id = int.Parse(xmlReader.GetAttribute("id"));
                            string name = xmlReader.GetAttribute("name");

                            // Check for existing name
                            foreach (Vertex v in verticies.Values)
                            {
                                if (v.Name == name)
                                {
                                    throw new VertexNameAlreadyExistsException(name);
                                }
                            }

                            Vertex vertex = new Vertex(graph, name)
                            {
                                Description = xmlReader.GetAttribute("description"),
                                Code = xmlReader.GetAttribute("code"),
                                Parameters = xmlReader.GetAttribute("parameters")
                            };

                            if (int.TryParse(xmlReader.GetAttribute("x"), out int x))
                            {
                                vertex.X = x;
                            }

                            if (int.TryParse(xmlReader.GetAttribute("y"), out int y))
                            {
                                vertex.Y = y;
                            }

                            verticies.Add(id, vertex);

                            if (bool.TryParse(xmlReader.GetAttribute("starting"), out bool isStartingVertex) && isStartingVertex)
                            {
                                startingVertexId = id;
                            }
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
                                Priority = xmlReader.GetAttribute("priority"),
                                Parameters = xmlReader.GetAttribute("parameters")
                            };

                            edges.Add(id, edge);
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, Vertex> kvp in verticies)
            {
                graph.AddVertex(kvp.Value);
                if (startingVertexId.HasValue && kvp.Key == startingVertexId.Value)
                {
                    graph.StartingVertex = kvp.Value;
                }
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
            
            using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, outputSettings))
            {
                xmlWriter.WriteStartElement("graph");
                xmlWriter.WriteAttributeString("description", Description);

                xmlWriter.WriteStartElement("verticies");

                for (int i = 0; i < _verticies.Count; i++)
                {
                    xmlWriter.WriteStartElement("vertex");

                    xmlWriter.WriteAttributeString("id", i.ToString());
                    xmlWriter.WriteAttributeString("name", _verticies[i].Name);
                    xmlWriter.WriteAttributeString("description", _verticies[i].Description);
                    xmlWriter.WriteAttributeString("code", _verticies[i].Code);
                    xmlWriter.WriteAttributeString("parameters", _verticies[i].Parameters);

                    xmlWriter.WriteAttributeString("x", _verticies[i].X.ToString());
                    xmlWriter.WriteAttributeString("y", _verticies[i].Y.ToString());

                    if (_verticies[i].IsStartingVertex)
                    {
                        xmlWriter.WriteAttributeString("starting", _verticies[i].IsStartingVertex.ToString());
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
                    xmlWriter.WriteAttributeString("parameters", _edges[i].Parameters);

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement(); // edges

                xmlWriter.WriteEndElement(); // graph
            }
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

    [Serializable]
    public class GraphMismatchException : Exception
    {
        public GraphMismatchException() : base() { }
    }

    [Serializable]
    public class VertexNotInGraphException : Exception
    {
        public VertexNotInGraphException() : base() { }
    }

    [Serializable]
    public class CannotRemoveVertexWithEdgesException : Exception
    {
        public CannotRemoveVertexWithEdgesException() : base() { }
    }
}
