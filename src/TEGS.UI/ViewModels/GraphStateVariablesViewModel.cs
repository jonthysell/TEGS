// 
// GraphStateVariablesViewModel.cs
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
                SelectedStateVariableIndex = value != null ? StateVariables.IndexOf(value) : -1;
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
                return _addStateVariable ?? (_addStateVariable = new RelayCommand(() =>
                {
                    try
                    {
                        StateVariables.SortedInsert(ObservableStateVariable.CreateNew(ChildIsDirtyChanged));
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _addStateVariable;

        public RelayCommand RemoveStateVariable
        {
            get
            {
                return _removeStateVariable ?? (_removeStateVariable = new RelayCommand(() =>
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
                }));
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
