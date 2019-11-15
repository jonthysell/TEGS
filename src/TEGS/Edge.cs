// 
// Edge.cs
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

namespace TEGS
{
    public class Edge
    {
        public Graph Graph { get; private set; }

        public Vertex Source { get; private set; }

        public Vertex Target { get; private set; }

        public EdgeAction Action { get; set; } = EdgeAction.Schedule;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _description = null;
                }
                else
                {
                    _description = value.Trim();
                }
            }
        }
        private string _description = null;

        public string Condition { get; set; } = null;

        public string Delay { get; set; } = null;

        public string Priority { get; set; } = null;

        public string Parameters { get; set; } = null;

        public Edge(Graph graph, Vertex source, Vertex target)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Description))
            {
                return Description;
            }

            return base.ToString();
        }
    }

    public enum EdgeAction
    {
        Schedule,
        CancelNext,
        CancelAll
    }
}
