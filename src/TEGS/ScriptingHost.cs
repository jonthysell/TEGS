// 
// ScriptingHost.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
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

namespace TEGS
{
    public abstract class ScriptingHost
    {
        #region Execution

        public abstract void Execute(string code);

        public bool TryExecute(string code)
        {
            try
            {
                Execute(code);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        #endregion

        #region Creation

        public void Create(StateVariable stateVariable)
        {
            if (null == stateVariable)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            CreateInternal(stateVariable);
        }

        public bool TryCreate(StateVariable stateVariable)
        {
            try
            {
                Create(stateVariable);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        protected abstract void CreateInternal(StateVariable stateVariable);

        #endregion

        #region Assignment

        public void Assign(StateVariable stateVariable, VariableValue value)
        {
            if (null == stateVariable)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            if (stateVariable.Type != value.Type)
            {
                throw new StateVariableAssignmentException(stateVariable, value);
            }

            AssignInternal(stateVariable, value);
        }

        public bool TryAssign(StateVariable stateVariable, VariableValue value)
        {
            try
            {
                Assign(stateVariable, value);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        public void Assign(IReadOnlyList<StateVariable> stateVariables, IReadOnlyList<VariableValue> values)
        {
            if (null == stateVariables)
            {
                throw new ArgumentNullException(nameof(stateVariables));
            }

            if (null == values)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (stateVariables.Count != values.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(values));
            }

            for (int i = 0; i < stateVariables.Count; i++)
            {
                Assign(stateVariables[i], values[i]);
            }
        }

        protected abstract void AssignInternal(StateVariable stateVariable, VariableValue value);

        #endregion

        #region Getters

        public VariableValue Get(StateVariable stateVariable)
        {
            if (null == stateVariable)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            return GetInternal(stateVariable);
        }

        public bool TryGet(StateVariable stateVariable, out VariableValue value)
        {
            try
            {
                value = Get(stateVariable);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            value = default(VariableValue);
            return false;
        }

        protected abstract VariableValue GetInternal(StateVariable stateVariable);

        #endregion

        #region Evaluators

        public VariableValue Evaluate(string code, VariableValueType returnType)
        {
            if (null == code)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return EvaluateInternal(code, returnType);
        }

        public VariableValue Evaluate(string code, VariableValue defaultValue)
        {
            if (!string.IsNullOrEmpty(code) && TryEvaluate(code, defaultValue.Type, out VariableValue value))
            {
                return value;
            }

            return defaultValue;
        }

        public bool TryEvaluate(string code, VariableValueType returnType, out VariableValue value)
        {
            try
            {
                value = Evaluate(code, returnType);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            value = default(VariableValue);
            return false;
        }

        protected abstract VariableValue EvaluateInternal(string code, VariableValueType returnType);

        #endregion

        #region Delegates

        public abstract void AssignDelegate(string name, Delegate @delegate);

        #endregion

        #region Seed

        public abstract void SetSeed(int seed);

        public void SetSeed()
        {
            // Adapted from http://lua-users.org/wiki/MathLibraryTutorial
            char[] c = DateTime.UtcNow.Ticks.ToString().ToCharArray();
            Array.Reverse(c);
            SetSeed(int.Parse(new string(c).Substring(1, 6)));
        }

        #endregion
    }

    public class StateVariableAssignmentException : StateVariableException
    {
        public readonly VariableValue NewValue;

        public StateVariableAssignmentException(StateVariable stateVariable, VariableValue newValue) : base(stateVariable)
        {
            NewValue = newValue;
        }
    }

    public class StateVariableAlreadyExistsException : Exception
    {
        public readonly string Name;

        public StateVariableAlreadyExistsException(string name) : base()
        {
            Name = name;
        }
    }

    public class StateVariableNotFoundException : Exception
    {
        public readonly string Name;

        public StateVariableNotFoundException(string name) : base()
        {
            Name = name;
        }
    }
}
