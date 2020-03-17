// 
// ObservableStateVariable.cs
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

        public ObservableCollection<string> Types => ObservableEnums.GetCollection<VariableValueType>();

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
            if (null == graph)
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
