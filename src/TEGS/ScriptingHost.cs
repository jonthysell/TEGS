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

        #region Assignment

        public abstract void AssignBoolean(string name, bool value);

        public abstract void AssignInteger(string name, int value);

        public abstract void AssignDouble(string name, double value);

        public abstract void AssignString(string name, string value);

        public bool TryAssign(string name, bool value)
        {
            try
            {
                AssignBoolean(name, value);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        public bool TryAssign(string name, int value)
        {
            try
            {
                AssignInteger(name, value);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        public bool TryAssign(string name, double value)
        {
            try
            {
                AssignDouble(name, value);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        public bool TryAssign(string name, string value)
        {
            try
            {
                AssignString(name, value);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            return false;
        }

        #endregion

        #region Getters

        public abstract bool GetBoolean(string name);

        public abstract int GetInteger(string name);

        public abstract double GetDouble(string name);

        public abstract string GetString(string name);

        public bool TryGet(string name, out bool result)
        {
            try
            {
                result = GetBoolean(name);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(bool);
            return false;
        }

        public bool TryGet(string name, out int result)
        {
            try
            {
                result = GetInteger(name);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(int);
            return false;
        }

        public bool TryGet(string name, out double result)
        {
            try
            {
                result = GetDouble(name);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(double);
            return false;
        }

        public bool TryGet(string name, out string result)
        {
            try
            {
                result = GetString(name);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(string);
            return false;
        }

        public TraceVariable GetTraceVariable(TraceVariable traceVariable)
        {
            switch (traceVariable.Type)
            {
                case TraceVariableType.Boolean:
                    return new TraceVariable(traceVariable.Name, GetBoolean(traceVariable.Name));
                case TraceVariableType.Integer:
                    return new TraceVariable(traceVariable.Name, GetInteger(traceVariable.Name));
                case TraceVariableType.Double:
                    return new TraceVariable(traceVariable.Name, GetDouble(traceVariable.Name));
                case TraceVariableType.String:
                    return new TraceVariable(traceVariable.Name, GetString(traceVariable.Name));
            }

            throw new ArgumentOutOfRangeException(nameof(traceVariable));
        }

        #endregion

        #region Evaluators

        public abstract bool EvaluateBoolean(string code);

        public abstract int EvaluateInteger(string code);

        public abstract double EvaluateDouble(string code);

        public abstract string EvaluateString(string code);

        public bool EvaluateBoolean(string code, bool defaultValue)
        {
            if (!string.IsNullOrEmpty(code) && TryEvaluate(code, out bool result))
            {
                return result;
            }

            return defaultValue;
        }

        public double EvaluateInteger(string code, int defaultValue)
        {
            if (!string.IsNullOrEmpty(code) && TryEvaluate(code, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        public double EvaluateDouble(string code, double defaultValue)
        {
            if (!string.IsNullOrEmpty(code) && TryEvaluate(code, out double result))
            {
                return result;
            }

            return defaultValue;
        }

        public string EvaluateString(string code, string defaultValue)
        {
            if (!string.IsNullOrEmpty(code) && TryEvaluate(code, out string result))
            {
                return result;
            }

            return defaultValue;
        }

        public bool TryEvaluate(string code, out bool result)
        {
            try
            {
                result = EvaluateBoolean(code);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(bool);
            return false;
        }

        public bool TryEvaluate(string code, out int result)
        {
            try
            {
                result = EvaluateInteger(code);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(int);
            return false;
        }

        public bool TryEvaluate(string code, out double result)
        {
            try
            {
                result = EvaluateDouble(code);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(double);
            return false;
        }

        public bool TryEvaluate(string code, out string result)
        {
            try
            {
                result = EvaluateString(code);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }

            result = default(string);
            return false;
        }

        #endregion

        #region Parameters

        public abstract void AssignParameters(string lhs, string rhs);

        public abstract string EvaluateParameters(string parameters);

        public abstract bool CompareParameters(string a, string b);

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
}
