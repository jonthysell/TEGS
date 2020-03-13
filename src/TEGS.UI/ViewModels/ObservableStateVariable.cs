﻿// 
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
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

namespace TEGS.UI.ViewModels
{
    public class ObservableStateVariable : ObservableObject
    {
        #region Properties

        public string Name
        {
            get
            {
                return StateVariable.Name;
            }
            set
            {
                StateVariable = new StateVariable(value, StateVariable.Type);
                RaisePropertyChanged();
            }
        }

        public string Type
        {
            get
            {
                return StateVariable.Type.ToString();
            }
            set
            {
                StateVariable = new StateVariable(StateVariable.Name, Enum.Parse<VariableValueType>(value));
                RaisePropertyChanged();
            }
        }

        #endregion

        internal StateVariable StateVariable { get; private set; }

        #region Creation

        public ObservableStateVariable(StateVariable stateVariable)
        {
            StateVariable = stateVariable ?? throw new ArgumentNullException(nameof(stateVariable));
        }

        public static ObservableStateVariable CreateNew() => new ObservableStateVariable(new StateVariable(default, default));

        public static ObservableCollection<ObservableStateVariable> MakeObservableStateVariables(ObservableGraph graph)
        {
            if (null == graph)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            var stateVariables = new ObservableCollection<ObservableStateVariable>();

            foreach (var stateVariable in graph.Graph.StateVariables)
            {
                stateVariables.Add(new ObservableStateVariable(stateVariable));
            }

            return stateVariables;
        }

        #endregion
    }
}
