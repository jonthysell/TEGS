// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using GalaSoft.MvvmLight.Command;

namespace TEGS.UI.ViewModels
{
    public class GraphStateVariablesViewModel : EditorViewModelBase
    {
        #region Properties

        public ObservableCollection<ObservableStateVariable> StateVariables { get; private set; }

        public ObservableStateVariable SelectedStateVariable
        {
            get
            {
                return SelectedStateVariableIndex != -1 ? StateVariables[SelectedStateVariableIndex] : null;
            }
            set
            {
                SelectedStateVariableIndex = value is not null ? StateVariables.IndexOf(value) : -1;
            }
        }

        public int SelectedStateVariableIndex
        {
            get
            {
                return _selectedStateVariableIndex;
            }
            set
            {
                _selectedStateVariableIndex = value >= 0 && value < StateVariables.Count ? value : -1;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SelectedStateVariable));
                RemoveStateVariable.RaiseCanExecuteChanged();
            }
        }
        private int _selectedStateVariableIndex = -1;

        public override bool IsDirty => !StateVariables.EqualItems(Graph.StateVariables);

        #endregion

        #region Commands

        public RelayCommand AddStateVariable
        {
            get
            {
                return _addStateVariable ??= new RelayCommand(() =>
                {
                    try
                    {
                        StateVariables.SortedInsert(ObservableStateVariable.CreateNew(ChildIsDirtyChanged));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                });
            }
        }
        private RelayCommand _addStateVariable;

        public RelayCommand RemoveStateVariable
        {
            get
            {
                return _removeStateVariable ??= new RelayCommand(() =>
                {
                    try
                    {
                        var item = SelectedStateVariable;
                        StateVariables.RemoveAt(SelectedStateVariableIndex);
                        item.IsDirtyChanged -= ChildIsDirtyChanged;
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }, () =>
                {
                    return SelectedStateVariableIndex != -1;
                });
            }
        }
        private RelayCommand _removeStateVariable;

        #endregion

        public ObservableGraph Graph { get; private set; }

        public GraphStateVariablesViewModel(ObservableGraph graph) : base("State Variables")
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));

            StateVariables = ObservableStateVariable.MakeObservableCollection(Graph, true, ChildIsDirtyChanged);
            StateVariables.CollectionChanged += StateVariables_CollectionChanged;
        }

        private void StateVariables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SelectedStateVariableIndex = StateVariables.Count - 1;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                SelectedStateVariableIndex = Math.Min(SelectedStateVariableIndex, StateVariables.Count - 1);
            }
            RaisePropertyChanged(nameof(IsDirty));
        }

        protected override void ProcessAccept()
        {
            Graph.ReplaceStateVariables(StateVariables);
        }
    }
}
