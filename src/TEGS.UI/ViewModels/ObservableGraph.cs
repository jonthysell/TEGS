// 
// ObservableGraph.cs
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
using System.Collections.ObjectModel;
using System.IO;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public class ObservableGraph : ObservableObject
    {
        #region Properties

        public string Name
        {
            get
            {
                return Graph.Name;
            }
            set
            {
                value = value?.Trim() ?? "";

                if (value != Graph.Name)
                {
                    Graph.Name = value;
                    RaisePropertyChanged();
                    IsDirty = true;
                }
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
                value = value?.Trim() ?? "";

                if (value != Graph.Description)
                {
                    Graph.Description = value;
                    RaisePropertyChanged();
                    IsDirty = true;
                }
            }
        }

        public ReadOnlyObservableCollection<ObservableStateVariable> StateVariables
        {
            get
            {
                return _stateVariables ?? (_stateVariables = new ReadOnlyObservableCollection<ObservableStateVariable>(ObservableStateVariable.MakeObservableStateVariables(this, false)));
            }
            private set
            {
                _stateVariables = value;
                RaisePropertyChanged();
                IsDirty = true;
            }
        }
        private ReadOnlyObservableCollection<ObservableStateVariable> _stateVariables;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            private set
            {
                _fileName = value?.Trim() ?? "";
                RaisePropertyChanged();
            }
        }
        private string _fileName = "";

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            private set
            {
                _isDirty = value;
                RaisePropertyChanged();
                Save.RaiseCanExecuteChanged();
            }
        }
        private bool _isDirty = false;

        #endregion

        #region Commands

        public RelayCommand Save
        {
            get
            {
                return _save ?? (_save = new RelayCommand(() =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(FileName))
                        {
                            TrySaveAs();
                        }
                        else
                        {
                            using Stream outputStream = new FileStream(FileName, FileMode.Create);
                            Graph.SaveXml(outputStream);
                            IsDirty = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () =>
                {
                    return IsDirty;
                }));
            }
        }
        private RelayCommand _save;

        public RelayCommand SaveAs
        {
            get
            {
                return _saveAs ?? (_saveAs = new RelayCommand(() =>
                {
                    try
                    {
                        TrySaveAs();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _saveAs;

        public RelayCommand ShowProperties
        {
            get
            {
                return _showProperties ?? (_showProperties = new RelayCommand(() =>
                {
                    try
                    {
                        Messenger.Default.Send(new ShowGraphPropertiesMessage(this));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _showProperties;

        public RelayCommand ShowStateVariables
        {
            get
            {
                return _showStateVariables ?? (_showStateVariables = new RelayCommand(() =>
                {
                    try
                    {
                        Messenger.Default.Send(new ShowGraphStateVariablesMessage(this));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _showStateVariables;

        #endregion

        internal Graph Graph { get; private set; }

        #region Creation

        private ObservableGraph(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public static ObservableGraph NewGraph() => new ObservableGraph(new Graph()) { IsDirty = true };

        public static ObservableGraph OpenGraph(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            filename = Path.GetFullPath(filename);

            using var fs = new FileStream(filename, FileMode.Open);

            return new ObservableGraph(Graph.LoadXml(fs))
            {
                FileName = filename
            };
        }

        #endregion

        #region Setters

        public void ReplaceStateVariables(ObservableCollection<ObservableStateVariable> observableStateVariables)
        {
            Graph.StateVariables.Clear();
            foreach (var observableStateVariable in observableStateVariables)
            {
                Graph.StateVariables.Add(observableStateVariable.StateVariable);
            }

            StateVariables = new ReadOnlyObservableCollection<ObservableStateVariable>(observableStateVariables);
        }

        #endregion

        private void TrySaveAs()
        {
            Messenger.Default.Send(new SaveFileMessage("Save Graph As...", FileType.Graph, (filename) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(filename))
                    {
                        using Stream outputStream = new FileStream(filename, FileMode.Create);
                        Graph.SaveXml(outputStream);

                        FileName = filename;
                        IsDirty = false;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionUtils.HandleException(ex);
                }
            }));
        }
    }
}
