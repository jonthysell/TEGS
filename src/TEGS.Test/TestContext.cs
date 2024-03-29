﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS.Test
{
    public class TestContext : IContext
    {
        public Dictionary<string, VariableValue> Constants { get; private set; } = new Dictionary<string, VariableValue>();

        public Dictionary<string, VariableValue> Variables { get; private set; } = new Dictionary<string, VariableValue>();

        public Dictionary<string, CustomFunction> Functions { get; private set; } = new Dictionary<string, CustomFunction>();

        public VariableValue GetValue(string name)
        {
            return Constants.TryGetValue(name, out var constantValue) ? constantValue : Variables[name];
        }

        public void SetValue(string name, VariableValue value)
        {
            Variables[name] = value;
        }

        public VariableValue CallFunction(string name, VariableValue[] args)
        {
            return Functions[name](args);
        }
    }
}
