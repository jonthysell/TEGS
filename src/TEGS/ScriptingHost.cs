// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace TEGS
{
    public class ScriptingHost : IContext
    {
        private readonly Dictionary<string, StateVariable> _stateVariablesByName = new Dictionary<string, StateVariable>();
        private readonly Dictionary<StateVariable, VariableValue> _stateVariables = new Dictionary<StateVariable, VariableValue>();        

        private readonly HashSet<string> _libraryNames = new HashSet<string>();
        private readonly Dictionary<string, VariableValue> _constants = new Dictionary<string, VariableValue>();
        private readonly Dictionary<string, CustomFunction> _customFunctions = new Dictionary<string, CustomFunction>();

        private readonly Dictionary<string, Node> _parsedNodes = new Dictionary<string, Node>();
        private readonly Dictionary<string[], Node[]> _parsedCode = new Dictionary<string[], Node[]>();

        public ScriptingHost() { }

        private Node GetCachedNode(string expression)
        {
            if (!_parsedNodes.TryGetValue(expression, out Node node))
            {
                node = Parser.Parse(expression).Reduce();
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
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            ParseAndEvaluate(code);
        }

        public void Execute(string[] code)
        {
            if (code is not null)
            {
                if (!_parsedCode.TryGetValue(code, out Node[] nodes))
                {
                    nodes = new Node[code.Length];
                    for (int i = 0; i < code.Length; i++)
                    {
                        nodes[i] = GetCachedNode(code[i]);
                    }
                    _parsedCode[code] = nodes;
                }

                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Evaluate(this);
                }
            }
        }

        #endregion

        #region Evaluators

        public VariableValue Evaluate(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            return ParseAndEvaluate(code);
        }

        public VariableValue Evaluate(string code, VariableValue defaultValue)
        {
            return string.IsNullOrEmpty(code) ? defaultValue : Evaluate(code);
        }

        public bool TryEvaluate(string code, out VariableValue value)
        {
            try
            {
                value = Evaluate(code);
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
            if (stateVariable is null)
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
            _stateVariablesByName[stateVariable.Name] = stateVariable;
        }

        public void AssignVariable(StateVariable stateVariable, VariableValue value)
        {
            if (stateVariable is null)
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
        }

        public VariableValue GetVariable(StateVariable stateVariable)
        {
            if (stateVariable is null)
            {
                throw new ArgumentNullException(nameof(stateVariable));
            }

            if (!_stateVariables.ContainsKey(stateVariable))
            {
                throw new StateVariableNotFoundException(stateVariable.Name);
            }

            return _stateVariables[stateVariable];
        }

        #endregion

        #region Library

        public void LoadLibrary(ILibrary library, string name = null)
        {
            if (library is null)
            {
                throw new ArgumentNullException(nameof(library));
            }

            string libraryName = name?.Trim() ?? library.Name?.Trim() ?? "";

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

            if (function is null && _customFunctions.ContainsKey(name))
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

        public VariableValue GetValue(string name)
        {
            if (_constants.TryGetValue(name, out var constantValue))
            {
                return constantValue;
            }
            else if (_stateVariablesByName.TryGetValue(name, out var stateVariable))
            {
                return GetVariable(stateVariable);
            }

            throw new ArgumentException($"\"{name}\" is not a valid constant or state variable.");

        }

        public void SetValue(string name, VariableValue value)
        {
            if (_constants.ContainsKey(name))
            {
                throw new ArgumentException($"\"{name}\" is a constant and cannot be assigned.");
            }

            if (!_stateVariablesByName.TryGetValue(name, out var stateVariable))
            {
                throw new ArgumentException($"\"{name}\" is not a valid state variable and cannot be assigned.");
            }

            AssignVariable(stateVariable, value);
        }

        public VariableValue CallFunction(string name, VariableValue[] arguments)
        {
            if (!_customFunctions.TryGetValue(name, out var customFunction))
            {
                throw new ArgumentException($"\"{name}\" is not a valid function and cannot be called.");
            }

            return customFunction(arguments);
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

        public static bool TrySymbolify(string name, bool allowDot, out string result)
        {
            try
            {
                result = Symbolify(name, allowDot);
                return true;
            }
            catch (Exception) { }

            result = default;
            return false;
        }

        public static string Symbolify(string name, bool allowDot)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (IsValidSymbolName(name, allowDot))
            {
                return name;
            }

            StringBuilder sb = new StringBuilder();

            bool lastWhitespace = false;

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    if (char.IsLetter(name[i]) || name[i] == '_')
                    {
                        sb.Append(name[i]);
                        lastWhitespace = false;
                    }
                }
                else if (i > 0 && i < name.Length - 1)
                {
                    if (char.IsLetterOrDigit(name[i]) || name[i] == '_' || (allowDot && name[i] == '.'))
                    {
                        sb.Append(name[i]);
                        lastWhitespace = false;
                    }
                    else if (!lastWhitespace && char.IsWhiteSpace(name[i]))
                    {
                        sb.Append('_');
                        lastWhitespace = true;
                    }
                }
                else
                {
                    if (name[i - 1] == '.')
                    {
                        // After a dot
                        if (char.IsLetter(name[i]) || name[i] == '_')
                        {
                            sb.Append(name[i]);
                            lastWhitespace = false;
                        }
                    }
                    else
                    {
                        if (char.IsLetterOrDigit(name[i]) || name[i] == '_')
                        {
                            sb.Append(name[i]);
                            lastWhitespace = false;
                        }
                    }
                }
            }

            string result = sb.ToString();

            return !string.IsNullOrEmpty(result) ? result : throw new ArgumentException($"Unable to symbolify \"{ name }\".", nameof(name));
        }

        public static readonly HashSet<string> ReservedKeywords = new HashSet<string>()
        {
            // Reserved C# keywords
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
            "using", "virtual", "void", "volatile",
            "while",
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
