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

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace TEGS.UI.ViewModels
{
    public class ObservableGraph : ObservableObject<Graph>
    {
        #region Properties

        public string Name
        {
            get
            {
                return InternalObject.Name;
            }
            set
            {
                InternalObject.Name = value?.Trim() ?? "";
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public string Description
        {
            get
            {
                return InternalObject.Description;
            }
            set
            {
                InternalObject.Description = value?.Trim() ?? "";
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
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
                RaisePropertyChanged(nameof(IsDirty));
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
                RaisePropertyChanged(nameof(IsDirty));
            }
        }
        private string _fileName = "";

        public override bool IsDirty => _newGraphDirty || base.IsDirty;

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
                            TrySave();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
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

        private bool _newGraphDirty = false;

        #region Creation

        private ObservableGraph(): this(new Graph())
        {
            _newGraphDirty = true;
        }

        private ObservableGraph(Graph graph) : base(graph) { }

        public static ObservableGraph NewGraph() => new ObservableGraph();

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
            InternalObject.StateVariables.Clear();
            foreach (var observableStateVariable in observableStateVariables)
            {
                InternalObject.StateVariables.SortedInsert(observableStateVariable.InternalObject);
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
                        TrySave(filename);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionUtils.HandleException(ex);
                }
            }));
        }

        private void TrySave(string filename = null)
        {
            filename ??= FileName;

            using Stream outputStream = new FileStream(filename, FileMode.Create);

            InternalObject.SaveXml(outputStream);
            _newGraphDirty = false;
            FileName = filename;

            SaveToOriginal();
        }
    }
}
