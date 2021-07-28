// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    [Library(Name = "String")]
    public static class StringLibrary
    {
        [LibraryFunction]
        public static VariableValue Length(VariableValue[] args)
        {
            if (args != null && args.Length == 1 && args[0].Type == VariableValueType.String)
            {
                return new VariableValue(args[0].StringValue.Length);
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }
    }
}
