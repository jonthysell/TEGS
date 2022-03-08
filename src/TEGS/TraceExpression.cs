// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public abstract class TraceExpression
    {
        public readonly string Label;

        public VariableValue Value;

        public TraceExpression(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            Label = label.Trim();
        }

        public TraceExpression(string label, VariableValueType type) : this(label)
        {
            Value = new VariableValue(type);
        }

        public abstract void Evaluate(ScriptingHost scriptingHost);
    }

    public class StateVariableTraceExpression : TraceExpression
    {
        public readonly StateVariable StateVariable;

        public StateVariableTraceExpression(StateVariable stateVariable) : base(stateVariable.Name, stateVariable.Type)
        {
            StateVariable = stateVariable ?? throw new ArgumentNullException(nameof(stateVariable));
        }

        public StateVariableTraceExpression(string label, StateVariable stateVariable) : base(label)
        {
            StateVariable = stateVariable ?? throw new ArgumentNullException(nameof(stateVariable));
            Value = new VariableValue(stateVariable.Type);
        }

        public override void Evaluate(ScriptingHost scriptingHost)
        {
            Value = scriptingHost.GetVariable(StateVariable);
        }
    }

    public class CodeTraceExpression : TraceExpression
    {
        public readonly string Code;

        public CodeTraceExpression(string label, string code) : base(label)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            Code = code.Trim();
        }

        public override void Evaluate(ScriptingHost scriptingHost)
        {
            Value = scriptingHost.Evaluate(Code);
        }
    }
}
