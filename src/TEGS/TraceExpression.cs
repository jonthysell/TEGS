// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public abstract class TraceExpression
    {
        public readonly string Label;

        public VariableValue Value;

        public TraceExpression(string label, VariableValueType type)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            Label = label.Trim();
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

        public override void Evaluate(ScriptingHost scriptingHost)
        {
            Value = scriptingHost.Get(StateVariable);
        }
    }
}
