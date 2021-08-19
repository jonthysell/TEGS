// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TEGS.UI.ViewModels
{
    public class ObservableStateVariable : ObservableObject<StateVariable>, IComparable<ObservableStateVariable>, IEquatable<ObservableStateVariable>
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
                InternalObject.Name = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public string Type
        {
            get
            {
                return InternalObject.Type.ToString();
            }
            set
            {
                InternalObject.Type = Enum.Parse<VariableValueType>(value);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public static ObservableCollection<string> Types => ObservableEnums.GetCollection<VariableValueType>();

        public string Description
        {
            get
            {
                return InternalObject.Description;
            }
            set
            {
                InternalObject.Description = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        #endregion

        #region Creation

        protected ObservableStateVariable(StateVariable stateVariable) : base(stateVariable) { }

        public static ObservableStateVariable Create(StateVariable item, IsDirtyChangedEventHandler onIsDirtyChanged = null)
        {
            return Create(item, item => new ObservableStateVariable(item), onIsDirtyChanged);
        }

        public static ObservableStateVariable CreateNew(IsDirtyChangedEventHandler onIsDirtyChanged = null)
        {
            return CreateNew<ObservableStateVariable, StateVariable>(() => new ObservableStateVariable(new StateVariable()), onIsDirtyChanged);
        }

        public static ObservableCollection<ObservableStateVariable> MakeObservableCollection(ObservableGraph graph, bool clone, IsDirtyChangedEventHandler onIsDirtyChanged = null)
        {
            if (graph is null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            return MakeObservableCollection(graph.InternalObject.StateVariables, item => new ObservableStateVariable(item), clone, onIsDirtyChanged);
        }

        public bool Equals(ObservableStateVariable other)
        {
            return Equals(other as ObservableObject<StateVariable>);
        }

        public int CompareTo(ObservableStateVariable other)
        {
            return CompareTo(other as ObservableObject<StateVariable>);
        }

        #endregion
    }
}
