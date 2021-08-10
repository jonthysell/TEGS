// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
                return _stateVariables ??= new ReadOnlyObservableCollection<ObservableStateVariable>(ObservableStateVariable.MakeObservableCollection(this, false));
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
                return _save ??= new RelayCommand(() =>
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
                });
            }
        }
        private RelayCommand _save;

        public RelayCommand SaveAs
        {
            get
            {
                return _saveAs ??= new RelayCommand(() =>
                {
                    try
                    {
                        TrySaveAs();
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
            }
        }
        private RelayCommand _saveAs;

        public RelayCommand ShowProperties
        {
            get
            {
                return _showProperties ??= new RelayCommand(() =>
                {
                    try
                    {
                        Messenger.Default.Send(new ShowGraphPropertiesMessage(this));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
            }
        }
        private RelayCommand _showProperties;

        public RelayCommand ShowStateVariables
        {
            get
            {
                return _showStateVariables ??= new RelayCommand(() =>
                {
                    try
                    {
                        Messenger.Default.Send(new ShowGraphStateVariablesMessage(this));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
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

        public static ObservableGraph NewGraph()
        {
            return new ObservableGraph();
        }

        public static ObservableGraph OpenGraph(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            filename = Path.GetFullPath(filename);

            using var fs = new FileStream(filename, FileMode.Open);

            return new ObservableGraph(Graph.Load(fs))
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

            InternalObject.Save(outputStream);
            _newGraphDirty = false;
            FileName = filename;

            SaveToOriginal();
        }
    }
}
