// 
// VariableValue.cs
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

        public double AsDouble()
        {
            switch (Type)
            {
                case VariableValueType.Boolean:
                    return Convert.ToDouble(_value.BooleanValue);
                case VariableValueType.Integer:
                    return _value.IntegerValue;
                case VariableValueType.Double:
                    return _value.DoubleValue;
                default:
                    return Convert.ToDouble(_objectValue);
            }
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

        public override bool Equals(object obj)
        {
            if (obj is VariableValue other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Type.GetHashCode();
            hash = hash * 31 + AsDouble().GetHashCode();
            return hash;
        }

        public bool Equals(VariableValue other)
        {
            if (Type == VariableValueType.Boolean && other.Type == VariableValueType.Boolean)
            {
                return BooleanValue == other.BooleanValue;
            }
            else if (Type == VariableValueType.Integer && other.Type == VariableValueType.Integer)
            {
                return IntegerValue == other.IntegerValue;
            }
            else if (Type == VariableValueType.Double && other.Type == VariableValueType.Double)
            {
                return DoubleValue == other.DoubleValue;
            }
            else if (Type == VariableValueType.String && other.Type == VariableValueType.String)
            {
                return StringValue == other.StringValue;
            }
            else if (Type == VariableValueType.Integer && other.Type == VariableValueType.Double)
            {
                return other.AsDouble() == other.DoubleValue;
            }
            else if (Type == VariableValueType.Double && other.Type == VariableValueType.Integer)
            {
                return DoubleValue == other.AsDouble();
            }

            return false;
        }

        public static bool operator ==(VariableValue a, VariableValue b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VariableValue a, VariableValue b)
        {
            return !a.Equals(b);
        }

        public static VariableValue operator -(VariableValue a)
        {
            switch (a.Type)
            {
                case VariableValueType.Integer:
                    return new VariableValue(-a.IntegerValue);
                case VariableValueType.Double:
                    return new VariableValue(-a.DoubleValue);
            }

            throw new ArithmeticException();
        }

        public static VariableValue operator !(VariableValue a)
        {
            switch (a.Type)
            {
                case VariableValueType.Boolean:
                    return new VariableValue(!a.BooleanValue);
            }

            throw new ArithmeticException();
        }

        public static VariableValue operator +(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue + b.IntegerValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.DoubleValue + b.DoubleValue);
            }
            else if (a.Type == VariableValueType.String && b.Type == VariableValueType.String)
            {
                return new VariableValue(a.StringValue + b.StringValue);
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.AsDouble() + b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.DoubleValue + b.AsDouble());
            }

            throw new ArithmeticException();
        }

        public static VariableValue operator -(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue - b.IntegerValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.DoubleValue - b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.AsDouble() - b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.DoubleValue - b.AsDouble());
            }

            throw new ArithmeticException();
        }

        public static VariableValue operator *(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue * b.IntegerValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.DoubleValue * b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.AsDouble() * b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.DoubleValue * b.AsDouble());
            }

            throw new ArithmeticException();
        }

        public static VariableValue operator /(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue / b.IntegerValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.DoubleValue / b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return new VariableValue(a.AsDouble() / b.DoubleValue);
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.DoubleValue / b.AsDouble());
            }

            throw new ArithmeticException();
        }

        public static bool operator <(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue < b.IntegerValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return a.DoubleValue < b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return a.AsDouble() < b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return a.DoubleValue < b.AsDouble();
            }

            throw new ArithmeticException();
        }

        public static bool operator >(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue > b.IntegerValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return a.DoubleValue > b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return a.AsDouble() > b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return a.DoubleValue > b.AsDouble();
            }

            throw new ArithmeticException();
        }

        public static bool operator <=(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue <= b.IntegerValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return a.DoubleValue <= b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return a.AsDouble() <= b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return a.DoubleValue <= b.AsDouble();
            }

            throw new ArithmeticException();
        }

        public static bool operator >=(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue >= b.IntegerValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Double)
            {
                return a.DoubleValue >= b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Double)
            {
                return a.AsDouble() >= b.DoubleValue;
            }
            else if (a.Type == VariableValueType.Double && b.Type == VariableValueType.Integer)
            {
                return a.DoubleValue >= b.AsDouble();
            }

            throw new ArithmeticException();
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
