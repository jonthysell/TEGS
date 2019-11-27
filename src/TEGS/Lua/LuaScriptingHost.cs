// 
// LuaScriptingHost.cs
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
using System.Diagnostics;
using System.IO;

using MoonSharp.Interpreter;

namespace TEGS.Lua
{
    public class LuaScriptingHost : ScriptingHost
    {
        private Script _script = new Script(CoreModules.Preset_HardSandbox);

        private int _paramCount = 0;

        private Dictionary<string, DynamicExpression> _cachedDynamicExpressions = new Dictionary<string, DynamicExpression>();

        private Dictionary<string, DynValue> _cachedFunctions = new Dictionary<string, DynValue>();

        private static Dictionary<string, string> _embeddedLuaScripts = new Dictionary<string, string>();

        static LuaScriptingHost()
        {
            foreach (string name in typeof(LuaScriptingHost).Assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (StreamReader sr = new StreamReader(typeof(LuaScriptingHost).Assembly.GetManifestResourceStream(name)))
                    {
                        string contents = sr.ReadToEnd();
                        _embeddedLuaScripts.Add(name, contents);
                    }
                }
            }
        }

        public LuaScriptingHost()
        {
            _script.DebuggerEnabled = false;
            LoadEmbeddedLuaScripts();
        }

        private void LoadEmbeddedLuaScripts()
        {
            Debug.WriteLine($"{nameof(LuaScriptingHost)}.{nameof(LoadEmbeddedLuaScripts)}():");
            Debug.Indent();

            foreach (var kvp in _embeddedLuaScripts)
            {
                Debug.WriteLine($"Loading {kvp.Key}");
                ExecuteInternal(kvp.Value, false);
            }

            Debug.Unindent();
        }

        #region Execution
        
        public override void Execute(string code)
        {
            ExecuteInternal(code, true);
        }

        private DynValue ExecuteInternal(string code, bool cache)
        {
            if (!string.IsNullOrEmpty(code))
            {
                if (!_cachedFunctions.TryGetValue(code, out DynValue func))
                {
                    func = _script.LoadFunction(string.Format("function() {0} end", code));
                    if (cache)
                    {
                        _cachedFunctions.Add(code, func);
                    }
                }

                return _script.Call(func);
            }

            return null;
        }

        #endregion

        #region Creation

        protected override void CreateInternal(StateVariable stateVariable)
        {
            string name = stateVariable.Name;

            if (_script.Globals.HasKey(name))
            {
                throw new StateVariableAlreadyExistsException(name);
            }

            switch (stateVariable.Type)
            {
                case VariableValueType.Boolean:
                    _script.Globals.Set(name, default(bool) ? DynValue.True : DynValue.False);
                    break;
                case VariableValueType.Integer:
                    _script.Globals.Set(name, DynValue.NewNumber(default(int)));
                    break;
                case VariableValueType.Double:
                    _script.Globals.Set(name, DynValue.NewNumber(default(double)));
                    break;
                case VariableValueType.String:
                    _script.Globals.Set(name, DynValue.NewString(default(string)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateVariable.Type));
            }
        }

        #endregion

        #region Assignment

        public void Assign(string name, object value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // LHS is not null

                if (null == value)
                {
                    // RHS is null, assign nil to LHS
                    ExecuteInternal(string.Format("{0} = nil", name), false);
                }
                else
                {
                    // RHS is not null
                    string srhs = value.ToString();

                    if (string.IsNullOrWhiteSpace(srhs))
                    {
                        // string of RHS is still null, assign nil to LHS
                        ExecuteInternal(string.Format("{0} = nil", name), false);
                    }
                    else
                    {
                        // string of RHS is valid, assign to LHS
                        ExecuteInternal(string.Format("{0} = {1}", name, srhs.Trim()), false);
                    }
                }
            }
        }

        protected override void AssignInternal(StateVariable stateVariable, VariableValue value)
        {
            string name = stateVariable.Name;

            if (!_script.Globals.HasKey(name))
            {
                throw new StateVariableNotFoundException(name);
            }

            switch (stateVariable.Type)
            {
                case VariableValueType.Boolean:
                    _script.Globals.Set(name, value.BooleanValue ? DynValue.True : DynValue.False);
                    break;
                case VariableValueType.Integer:
                    _script.Globals.Set(name, DynValue.NewNumber(value.IntegerValue));
                    break;
                case VariableValueType.Double:
                    _script.Globals.Set(name, DynValue.NewNumber(value.DoubleValue));
                    break;
                case VariableValueType.String:
                    _script.Globals.Set(name, DynValue.NewString(value.StringValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateVariable.Type));
            }
        }

        #endregion

        #region Getters

        protected override VariableValue GetInternal(StateVariable stateVariable)
        {
            string name = stateVariable.Name;

            DynValue value = _script.Globals.RawGet(name);

            if (null == value)
            {
                throw new StateVariableNotFoundException(name);
            }

            switch (stateVariable.Type)
            {
                case VariableValueType.Boolean:
                    return new VariableValue(value.Boolean);
                case VariableValueType.Integer:
                    return new VariableValue((int)value.Number);
                case VariableValueType.Double:
                    return new VariableValue(value.Number);
                case VariableValueType.String:
                    return new VariableValue(value.String);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateVariable.Type));
            }

        }

        #endregion

        #region Evaluators

        public override bool EvaluateBoolean(string code)
        {
            return Evaluate(code).Boolean;
        }

        public override int EvaluateInteger(string code)
        {
            return (int)Evaluate(code).Number;
        }

        public override double EvaluateDouble(string code)
        {
            return Evaluate(code).Number;
        }

        public override string EvaluateString(string code)
        {
            return Evaluate(code).String;
        }

        private DynValue Evaluate(string code, bool cache = true)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }

            try
            {
                if (_cachedDynamicExpressions.TryGetValue(code, out DynamicExpression exp))
                {
                    return exp.Evaluate();
                }

                exp = _script.CreateDynamicExpression(code);

                DynValue result = exp.Evaluate();
                if (cache)
                {
                    _cachedDynamicExpressions.Add(code, exp);
                }
                return result;
            }
            catch (DynamicExpressionException)
            {
                return ExecuteInternal($"return {code}", cache);
            }
        }

        #endregion

        #region Delegates

        public override void AssignDelegate(string name, Delegate @delegate)
        {
            _script.Globals.Set(name, DynValue.NewCallback(CallbackFunction.FromDelegate(_script, @delegate)));
        }

        #endregion

        #region Parameters

        public override void AssignParameters(string lhs, string rhs)
        {
            if (null != rhs && (rhs = rhs.Trim()).StartsWith(ParamPrefix))
            {
                // RHS is a temporary param table

                // Unpack RHS and assign to LHS
                Assign(lhs, string.Format("table.unpack({0})", rhs));

                // Delete the temporary param table
                Assign(rhs, null);
            }
            else
            {
                // RHS is either null or not a temporary param table
                Assign(lhs, rhs);
            }
        }

        public override string EvaluateParameters(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                return null;
            }

            // Evaluate the parameters by assigning them to a temporary param table
            string lhs = string.Format("{0}{1}", ParamPrefix, _paramCount++);
            string rhs = string.Format(ParamWrapInTable, parameters.Trim());
            Assign(lhs, rhs);

            // Return the name of the the temporary param table
            return lhs;
        }

        public override bool CompareParameters(string a, string b)
        {
            // Trim if necessary
            a = a?.Trim();
            b = b?.Trim();

            if (a == b)
            {
                // Quick compare
                return true;
            }
            else if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                // Null/empty string compare
                return true;
            }

            if (!string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b))
            {
                // Both have valid values, pack into table if necessary

                a = a.StartsWith(ParamPrefix) ? a : string.Format(ParamWrapInTable, a);
                b = b.StartsWith(ParamPrefix) ? b : string.Format(ParamWrapInTable, b);

                return EvaluateBoolean(string.Format("t_comparetables({0}, {1})", a, b));
            }

            return false;
        }

        #endregion

        #region Seed

        public override void SetSeed(int value)
        {
            Execute(string.Format("math.randomseed({0})", value));
        }

        #endregion

        private const string ParamPrefix = @"t_param";

        private const string ParamWrapInTable = @"{{ {0} }}";
    }
}
