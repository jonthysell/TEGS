// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.Collections.Generic;

namespace TEGS
{
    public interface ILibrary
    {
        string Name { get; }
        IEnumerable<KeyValuePair<string, VariableValue>> GetConstants();
        IEnumerable<KeyValuePair<string, CustomFunction>> GetCustomFunctions();
    }
}
