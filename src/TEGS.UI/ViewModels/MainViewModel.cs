// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Text;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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

        public override string Title
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
                            if (!string.IsNullOrWhiteSpace(filename))
                            {
                                Graph = ObservableGraph.OpenGraph(filename);
                            }
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
                    try
                    {
                        RequestClose?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _close;

        #endregion

        public MainViewModel() : base() { }

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
