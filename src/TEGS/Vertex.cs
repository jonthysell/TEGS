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
        public readonly Graph Graph;

        public int Id
        {
            get
            {
                for (int i = 0; i < Graph.Verticies.Count; i++)
                {
                    if (this == Graph.Verticies[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value ?? value.Trim();
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
                _description = value ?? value.Trim();
            }
        }
        private string _description = "";

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value ?? value.Trim();
            }
        }
        private string _code = "";

        public string Parameters
        {
            get
            {
                if (null == ParameterNames)
                {
                    return "";
                }

                return string.Join(", ", ParameterNames);
            }
            set
            {
                ParameterNames = value?.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string[] ParameterNames { get; private set; }

        public bool IsStartingVertex { get; set; }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public IEnumerable<Edge> Edges
        {
            get
            {
                for (int i = 0; i < Graph.Edges.Count; i++)
                {
                    if (Graph.Edges[i].Source == this)
                    {
                        yield return Graph.Edges[i];
                    }
                }
            }
        }

        public Vertex(Graph graph, string name, bool isStartingVertex = false)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Name = name;
            IsStartingVertex = isStartingVertex;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
