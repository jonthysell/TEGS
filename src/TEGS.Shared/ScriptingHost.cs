// 
// ScriptingHost.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019, 2020 Jon Thysell <http://jonthysell.com>
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

using TEGS.Expressions;
using TEGS.Libraries;

namespace TEGS
{
    public class ScriptingHost : IContext
    {
        private readonly Dictionary<StateVariable, VariableValue> _stateVariables = new Dictionary<StateVariable, VariableValue>();
        private readonly Dictionary<string, VariableValue> _stateVariablesByName = new Dictionary<string, VariableValue>();

        private readonly HashSet<string> _libraryNames = new HashSet<string>();
        private readonly Dictionary<string, VariableValue> _constants = new Dictionary<string, VariableValue>();
        private readonly Dictionary<string, CustomFunction> _customFunctions = new Dictionary<string, CustomFunction>();

        private readonly Dictionary<string, Node> _parsedNodes = new Dictionary<string, Node>();
        private readonly Dictionary<string, List<Node>> _parsedCode = new Dictionary<string, List<Node>>();

        public ScriptingHost() { }

        private Node GetCachedNode(string expression)
        {
            if (!_parsedNodes.TryGetValue(expression, out Node node))
            {
                node = Parser.Parse(expression);
                _parsedNodes[expression] = node;
            }

            return node;
        }

        private VariableValue ParseAndEvaluate(string expression)
        {
            return GetCachedNode(expression).Evaluate(this);
        }

        #region Execution

        public void Execute(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                if (!_parsedCode.TryGetValue(code, out List<Node> nodes))
                {
                    string[] lines = code.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);

                    nodes = new List<Node>(lines.Length);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        nodes.Add(GetCachedNode(lines[i]));
                    }
                    _parsedCode[code] = nodes;
                }
                
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Evaluate(this);
                }
            }
        }

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

        private readonly char[] LineSeparators = new char[] { '\r', '\n', ';' };

        #endregion

        #region Evaluators

        public VariableValue Evaluate(string code, VariableValueType returnType)
        {
            if (null == code)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return ParseAndEvaluate(code);
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

            value = default;
            return false;
        }

        #endregion

        #region State Variables

        public void Create(StateVariable stateVariable)
        {
            if (null == stateVariable)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            if (_stateVariables.ContainsKey(stateVariable))
            {
                throw new StateVariableAlreadyExistsException(stateVariable.Name);
            }

            if (ReservedKeywords.Contains(stateVariable.Name) ||
                !IsValidSymbolName(stateVariable.Name, false) ||
                _libraryNames.Contains(stateVariable.Name) ||
                _constants.ContainsKey(stateVariable.Name) ||
                _customFunctions.ContainsKey(stateVariable.Name))
            {
                throw new StateVariableInvalidNameException(stateVariable.Name);
            }

            _stateVariables[stateVariable] = new VariableValue(stateVariable.Type);
            _stateVariablesByName[stateVariable.Name] = new VariableValue(stateVariable.Type);
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

            if (!_stateVariables.ContainsKey(stateVariable))
            {
                throw new StateVariableNotFoundException(stateVariable.Name);
            }

            _stateVariables[stateVariable] = value;
            _stateVariablesByName[stateVariable.Name] = value;
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

        public VariableValue Get(StateVariable stateVariable)
        {
            if (null == stateVariable)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            if (!_stateVariables.ContainsKey(stateVariable))
            {
                throw new StateVariableNotFoundException(stateVariable.Name);
            }

            return _stateVariables[stateVariable];
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

            value = default;
            return false;
        }

        #endregion

        #region Library

        public void LoadLibrary(ILibrary library)
        {
            if (null == library)
            {
                throw new ArgumentNullException(nameof(library));
            }

            string libraryName = library.Name?.Trim() ?? "";

            if (libraryName != "")
            {
                _libraryNames.Add(libraryName);
                libraryName += ".";
            }

            foreach (var kvp in library.GetConstants())
            {
                DefineConstant(libraryName + kvp.Key, kvp.Value);
            }

            foreach (var kvp in library.GetCustomFunctions())
            {
                DefineCustomFunction(libraryName + kvp.Key, kvp.Value);
            }
        }

        #endregion

        #region Constants

        public void DefineConstant(string name, VariableValue value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (ReservedKeywords.Contains(name) ||
                !IsValidSymbolName(name, true))
            {
                throw new ConstantInvalidNameException(name);
            }

            _constants[name] = value;
        }

        #endregion

        #region Custom Functions

        public void DefineCustomFunction(string name, CustomFunction function)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (ReservedKeywords.Contains(name) ||
                !IsValidSymbolName(name, true))
            {
                throw new CustomFunctionInvalidNameException(name);
            }

            if (null == function && _customFunctions.ContainsKey(name))
            {
                _customFunctions.Remove(name);
            }
            else
            {
                _customFunctions[name] = function;
            }
        }

        #endregion

        #region IContext

        public VariableValue GetVariable(string name)
        {
            return _stateVariablesByName[name];
        }

        public void SetVariable(string name, VariableValue value)
        {
            StateVariable stateVariable = new StateVariable(name, value.Type);
            Assign(stateVariable, value);
        }

        public VariableValue CallFunction(string name, VariableValue[] arguments)
        {
            return _customFunctions[name](arguments);
        }

        #endregion

        #region Valid Symbols

        public static bool IsValidSymbolName(string name, bool allowDot)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    // First letter
                    if (char.IsLetter(name[i]) || name[i] == '_')
                    {
                        continue;
                    }
                }
                else if (i > 0 && i < name.Length - 1)
                {
                    // Middle of name
                    if (name[i-1] == '.')
                    {
                        // After a dot
                        if (char.IsLetter(name[i]) || name[i] == '_')
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (char.IsLetterOrDigit(name[i]) || name[i] == '_' || (allowDot && name[i] == '.'))
                        {
                            continue;
                        }
                    }
                }
                else if (i == name.Length - 1)
                {
                    // Last letter
                    if (name[i - 1] == '.')
                    {
                        // After a dot
                        if (char.IsLetter(name[i]) || name[i] == '_')
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (char.IsLetterOrDigit(name[i]) || name[i] == '_')
                        {
                            continue;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        public static readonly HashSet<string> ReservedKeywords = new HashSet<string>()
        {
            "abstract", "as", "base", "bool",
            "break", "byte", "case", "catch",
            "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate",
            "do", "double", "else", "enum",
            "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit",
            "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace",
            "new", "null", "object", "operator",
            "out", "override", "params", "private",
            "protected", "public", "readonly", "ref",
            "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint",
            "ulong", "unchecked", "unsafe", "ushort",
            "using", "using", "static", "virtual", "void",
            "volatile", "while",
        };

        #endregion

    }

    #region Exceptions

    public class StateVariableAlreadyExistsException : Exception
    {
        public readonly string Name;

        public StateVariableAlreadyExistsException(string name) : base()
        {
            Name = name;
        }
    }

    public class StateVariableAssignmentException : StateVariableException
    {
        public readonly VariableValue NewValue;

        public StateVariableAssignmentException(StateVariable stateVariable, VariableValue newValue) : base(stateVariable)
        {
            NewValue = newValue;
        }
    }

    public class StateVariableInvalidNameException : Exception
    {
        public readonly string Name;

        public StateVariableInvalidNameException(string name) : base()
        {
            Name = name;
        }
    }

    public class ConstantInvalidNameException : Exception
    {
        public readonly string Name;

        public ConstantInvalidNameException(string name) : base()
        {
            Name = name;
        }
    }

    public class CustomFunctionInvalidNameException : Exception
    {
        public readonly string Name;

        public CustomFunctionInvalidNameException(string name) : base()
        {
            Name = name;
        }
    }

    #endregion
}
