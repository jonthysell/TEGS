// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

// Adapted from https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

namespace TEGS
{
    public interface IContext
    {
        VariableValue GetVariable(string name);

        void SetVariable(string name, VariableValue value);

        VariableValue CallFunction(string name, VariableValue[] arguments);
    }
}
