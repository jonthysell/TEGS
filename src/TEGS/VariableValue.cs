// 
// VariableValue.cs
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
using System.Runtime.InteropServices;

namespace TEGS
{
    public struct VariableValue : IEquatable<VariableValue>
    {
        public readonly VariableValueType Type;

        public bool BooleanValue => _value.BooleanValue;

        public int IntegerValue => _value.IntegerValue;

        public double DoubleValue => _value.DoubleValue;

        public string StringValue => (string)_objectValue;

        private readonly PrimitiveUnionValue _value;
        private readonly object _objectValue;

        public VariableValue(VariableValueType type)
        {
            Type = type;
            _value = new PrimitiveUnionValue();
            _objectValue = null;
        }

        public VariableValue(bool value) : this(VariableValueType.Boolean)
        {
            _value.BooleanValue = value;
        }

        public VariableValue(int value) : this(VariableValueType.Integer)
        {
            _value.IntegerValue = value;
        }

        public VariableValue(double value) : this(VariableValueType.Double)
        {
            _value.DoubleValue = value;
        }

        public VariableValue(string value) : this(VariableValueType.String)
        {
            _objectValue = value;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case VariableValueType.Boolean:
                    return _value.BooleanValue.ToString();
                case VariableValueType.Integer:
                    return _value.IntegerValue.ToString();
                case VariableValueType.Double:
                    return _value.DoubleValue.ToString();
                default:
                    return _objectValue?.ToString();
            }
        }

        public bool Equals(VariableValue other)
        {
            if (Type == other.Type)
            {
                switch (Type)
                {
                    case VariableValueType.Boolean:
                        return _value.BooleanValue == other._value.BooleanValue;
                    case VariableValueType.Integer:
                        return _value.IntegerValue == other._value.IntegerValue;
                    case VariableValueType.Double:
                        return _value.DoubleValue == other._value.DoubleValue;
                    default:
                        return _objectValue == other._objectValue;
                }
            }

            return false;
        }

        public static bool operator ==(VariableValue a, VariableValue b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VariableValue a, VariableValue b)
        {
            return a.Equals(b);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PrimitiveUnionValue
    {
        [FieldOffset(0)] public bool BooleanValue;
        [FieldOffset(0)] public int IntegerValue;
        [FieldOffset(0)] public double DoubleValue;
    }

    public enum VariableValueType
    {
        Boolean,
        Integer,
        Double,
        String
    }
}
