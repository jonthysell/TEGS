// 
// Vertex.cs
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
    public class Vertex
    {
        public Graph Graph { get; private set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                value = value.Trim();

                if (Graph.HasVertex(value))
                {
                    throw new VertexNameAlreadyExistsException(value);
                }

                _name = value;
            }
        }
        private string _name;

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

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value?.Trim();
            }
        }
        private string _code = null;

        public string Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value?.Trim();
            }
        }
        private string _parameters = null;

        public bool IsStartingVertex => (this == Graph.StartingVertex);

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public IEnumerable<Edge> Edges
        {
            get
            {
                foreach (Edge edge in Graph.Edges)
                {
                    if (edge.Source == this)
                    {
                        yield return edge;
                    }
                }
            }
        }

        public Vertex(Graph graph, string name)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class VertexNameAlreadyExistsException : Exception
    {
        public string Name { get; private set; }

        public VertexNameAlreadyExistsException(string name) : base()
        {
            Name = name;
        }
    }
}
