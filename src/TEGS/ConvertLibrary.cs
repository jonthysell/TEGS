// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    [Library(Name = "Convert")]
    public static class ConvertLibrary
    {
        [LibraryFunction]
        public static VariableValue ToBoolean(VariableValue[] args)
        {
            if (args is not null && args.Length == 1)
            {
                switch (args[0].Type)
                {
                    case VariableValueType.Boolean:
                        return new VariableValue(Convert.ToBoolean(args[0].BooleanValue));
                    case VariableValueType.Integer:
                        return new VariableValue(Convert.ToBoolean(args[0].IntegerValue));
                    case VariableValueType.Double:
                        return new VariableValue(Convert.ToBoolean(args[0].DoubleValue));
                    case VariableValueType.String:
                        return new VariableValue(Convert.ToBoolean(args[0].StringValue));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public static VariableValue ToInteger(VariableValue[] args)
        {
            if (args is not null && args.Length == 1)
            {
                switch (args[0].Type)
                {
                    case VariableValueType.Boolean:
                        return new VariableValue(Convert.ToInt32(args[0].BooleanValue));
                    case VariableValueType.Integer:
                        return new VariableValue(Convert.ToInt32(args[0].IntegerValue));
                    case VariableValueType.Double:
                        return new VariableValue(Convert.ToInt32(args[0].DoubleValue));
                    case VariableValueType.String:
                        return new VariableValue(Convert.ToInt32(args[0].StringValue));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public static VariableValue ToDouble(VariableValue[] args)
        {
            if (args is not null && args.Length == 1)
            {
                switch (args[0].Type)
                {
                    case VariableValueType.Boolean:
                        return new VariableValue(Convert.ToDouble(args[0].BooleanValue));
                    case VariableValueType.Integer:
                        return new VariableValue(Convert.ToDouble(args[0].IntegerValue));
                    case VariableValueType.Double:
                        return new VariableValue(Convert.ToDouble(args[0].DoubleValue));
                    case VariableValueType.String:
                        return new VariableValue(Convert.ToDouble(args[0].StringValue));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        [LibraryFunction]
        public static VariableValue ToString(VariableValue[] args)
        {
            if (args is not null && args.Length == 1)
            {
                switch (args[0].Type)
                {
                    case VariableValueType.Boolean:
                        return new VariableValue(Convert.ToString(args[0].BooleanValue));
                    case VariableValueType.Integer:
                        return new VariableValue(Convert.ToString(args[0].IntegerValue));
                    case VariableValueType.Double:
                        return new VariableValue(Convert.ToString(args[0].DoubleValue));
                    case VariableValueType.String:
                        return new VariableValue(Convert.ToString(args[0].StringValue));
                }
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }
    }
}
