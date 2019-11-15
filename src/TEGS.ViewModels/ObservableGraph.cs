// 
// ObservableGraph.cs
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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace TEGS.ViewModels
{
    public class ObservableGraph : ObservableObject
    {
        public IDialogService DialogService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<IDialogService>();
            }
        }

        public string Name
        {
            get
            {
                return Graph.Name;
            }
            set
            {
                Graph.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get
            {
                return Graph.Description;
            }
            set
            {
                Graph.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public ObservableVertex StartingVertex
        {
            get
            {
                foreach (ObservableVertex ov in Verticies)
                {
                    if (ov.Vertex == Graph.StartingVertex)
                    {
                        return ov;
                    }
                }

                return null;
            }
        }

        public ReadOnlyObservableCollection<ObservableVertex> Verticies
        {
            get
            {
                return _verticiesReadOnly ?? (_verticiesReadOnly = new ReadOnlyObservableCollection<ObservableVertex>(_verticies));
            }
        }
        private ReadOnlyObservableCollection<ObservableVertex> _verticiesReadOnly;

        private ObservableCollection<ObservableVertex> _verticies = new ObservableCollection<ObservableVertex>();

        public ReadOnlyObservableCollection<ObservableEdge> Edges
        {
            get
            {
                return _edgesReadOnly ?? (_edgesReadOnly = new ReadOnlyObservableCollection<ObservableEdge>(_edges));
            }
        }
        private ReadOnlyObservableCollection<ObservableEdge> _edgesReadOnly;

        private ObservableCollection<ObservableEdge> _edges = new ObservableCollection<ObservableEdge>();

        public RelayCommand AddVertexAsync
        {
            get
            {
                return _addVertexAsync ?? (_addVertexAsync = new RelayCommand(async () =>
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowExceptionAsync(ex);
                    }
                }));
            }
        }
        private RelayCommand _addVertexAsync;

        public RelayCommand EditVertexAsync
        {
            get
            {
                return _editVertexAsync ?? (_editVertexAsync = new RelayCommand(async () =>
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowExceptionAsync(ex);
                    }
                }));
            }
        }
        private RelayCommand _editVertexAsync;

        internal Graph Graph { get; private set; }

        public ObservableGraph(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));

            foreach (Vertex vertex in graph.Verticies)
            {
                _verticies.Add(new ObservableVertex(this, vertex));
            }

            foreach (Edge edge in graph.Edges)
            {
                _edges.Add(new ObservableEdge(this, edge));
            }
        }
    }
}
