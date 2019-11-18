// 
// ObservableEdge.cs
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

#if TEGSUI

using System;

using GalaSoft.MvvmLight;

namespace TEGS.UI
{
    public class ObservableEdge : ObservableObject
    {
        public ObservableGraph Graph { get; private set; }

        public ObservableVertex Source
        {
            get
            {
                foreach (ObservableVertex ov in Graph.Verticies)
                {
                    if (ov.Vertex == Edge.Source)
                    {
                        return ov;
                    }
                }

                return null;
            }
        }

        public ObservableVertex Target
        {
            get
            {
                foreach (ObservableVertex ov in Graph.Verticies)
                {
                    if (ov.Vertex == Edge.Target)
                    {
                        return ov;
                    }
                }

                return null;
            }
        }

        public EdgeAction Action
        {
            get
            {
                return Edge.Action;
            }
            set
            {
                Edge.Action = value;
                RaisePropertyChanged(nameof(Action));
            }
        }

        public string Description
        {
            get
            {
                return Edge.Description;
            }
            set
            {
                Edge.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public string Condition
        {
            get
            {
                return Edge.Condition;
            }
            set
            {
                Edge.Condition = value;
                RaisePropertyChanged(nameof(Condition));
            }
        }

        public string Delay
        {
            get
            {
                return Edge.Delay;
            }
            set
            {
                Edge.Delay = value;
                RaisePropertyChanged(nameof(Delay));
            }
        }

        public string Priority
        {
            get
            {
                return Edge.Priority;
            }
            set
            {
                Edge.Priority = value;
                RaisePropertyChanged(nameof(Priority));
            }
        }

        public string Parameters
        {
            get
            {
                return Edge.Parameters;
            }
            set
            {
                Edge.Parameters = value;
                RaisePropertyChanged(nameof(Parameters));
            }
        }

        internal Edge Edge { get; private set; }

        public ObservableEdge(ObservableGraph graph, Edge edge)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Edge = edge ?? throw new ArgumentNullException(nameof(edge));
        }
    }
}

#endif
