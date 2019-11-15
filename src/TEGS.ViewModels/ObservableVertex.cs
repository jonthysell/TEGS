// 
// ObservableVertex.cs
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
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

namespace TEGS.ViewModels
{
    public class ObservableVertex : ObservableObject
    {
        public ObservableGraph Graph { get; private set; }

        public string Name
        {
            get
            {
                return Vertex.Name;
            }
            set
            {
                Vertex.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get
            {
                return Vertex.Description;
            }
            set
            {
                Vertex.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public string Code
        {
            get
            {
                return Vertex.Code;
            }
            set
            {
                Vertex.Code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }

        public string Parameters
        {
            get
            {
                return Vertex.Parameters;
            }
            set
            {
                Vertex.Parameters = value;
                RaisePropertyChanged(nameof(Parameters));
            }
        }

        public bool IsStartingVertex
        {
            get
            {
                return Vertex.IsStartingVertex;
            }
        }

        public int X
        {
            get
            {
                return Vertex.X;
            }
            set
            {
                Vertex.X = value;
                RaisePropertyChanged(nameof(X));
            }
        }

        public int Y
        {
            get
            {
                return Vertex.Y;
            }
            set
            {
                Vertex.Y = value;
                RaisePropertyChanged(nameof(Y));
            }
        }

        public ReadOnlyObservableCollection<ObservableEdge> Edges
        {
            get
            {
                ObservableCollection<ObservableEdge> edges = new ObservableCollection<ObservableEdge>();
                foreach (ObservableEdge edge in Graph.Edges)
                {
                    if (edge.Source == this)
                    {
                        edges.Add(edge);
                    }
                }
                return new ReadOnlyObservableCollection<ObservableEdge>(edges);
            }
        }

        internal Vertex Vertex { get; private set; }

        public ObservableVertex(ObservableGraph graph, Vertex vertex)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
        }
    }
}
