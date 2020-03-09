// 
// MainViewModel.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
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
using System.ComponentModel;
using System.Text;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AppViewModel AppVM => AppViewModel.Instance;

        #region Properties

        public ObservableGraph Graph
        {
            get
            {
                return _graph;
            }
            private set
            {
                if (_graph != null)
                {
                    _graph.PropertyChanged -= Graph_PropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += Graph_PropertyChanged;
                }

                _graph = value;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Title));
            }
        }
        private ObservableGraph _graph;

        public string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (null != Graph)
                {
                    if (Graph.IsDirty)
                    {
                        sb.Append("*");
                    }

                    if (!string.IsNullOrWhiteSpace(Graph.FileName))
                    {
                        sb.Append(Graph.FileName);
                    }
                    else if (!string.IsNullOrWhiteSpace(Graph.Name))
                    {
                        sb.Append(Graph.Name);
                    }
                    else
                    {
                        sb.Append("Untitled");
                    }

                    sb.Append(" - ");
                }

                sb.Append("TEGS");

                return sb.ToString();
            }
        }

        #endregion

        #region Commands

        public RelayCommand NewGraph
        {
            get
            {
                return _newGraph ?? (_newGraph = new RelayCommand(() =>
                {
                    try
                    {
                        Graph = ObservableGraph.NewGraph();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _newGraph;

        public RelayCommand OpenGraph
        {
            get
            {
                return _openGraph ?? (_openGraph = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new OpenFileMessage("Open Graph", FileType.Graph, (filename) =>
                    {
                        try
                        {
                            Graph = ObservableGraph.OpenGraph(filename);
                        }
                        catch (Exception ex)
                        {
                            ExceptionUtils.HandleException(ex);
                        }
                    }));
                }));
            }
        }
        private RelayCommand _openGraph;

        public RelayCommand Close
        {
            get
            {
                return _close ?? (_close = new RelayCommand(() =>
                {
                }));
            }
        }
        private RelayCommand _close;

        #endregion

        public MainViewModel()
        {

        }

        private void Graph_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Graph.Name):
                case nameof(Graph.FileName):
                case nameof(Graph.IsDirty):
                    RaisePropertyChanged(nameof(Title));
                    break;
            }
        }
    }
}
