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
    public delegate VariableValue CustomFunction(VariableValue[] args);

    public struct VariableValue : IEquatable<VariableValue>
    {
        public readonly VariableValueType Type;

        public bool BooleanValue => _value.BooleanValue;

        public int IntegerValue => _value.IntegerValue;

        public double DoubleValue => _value.DoubleValue;

        public string StringValue => (string)_objectValue;

        public bool IsBoolean => Type == VariableValueType.Boolean;

        public bool IsNumber => Type == VariableValueType.Integer || Type == VariableValueType.Double;

        public bool IsString => Type == VariableValueType.String;

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

        public static VariableValue Parse(object value)
        {
            if (value is bool boolValue)
            {
                return new VariableValue(boolValue);
            }
            else if (value is int intValue)
            {
                return new VariableValue(intValue);
            }
            else if (value is double doubleValue)
            {
                return new VariableValue(doubleValue);
            }
            else if (value is string stringValue)
            {
                return new VariableValue(stringValue);
            }

            throw new ArgumentOutOfRangeException(nameof(value));
        }

        public static bool TryParse(object value, out VariableValue result)
        {
            try
            {
                result = Parse(value);
                return true;
            }
            catch (Exception) { }

            result = default;
            return false;
        }

        public double AsNumber()
        {
            switch (Type)
            {
                case VariableValueType.Integer:
                    return _value.IntegerValue;
                case VariableValueType.Double:
                    return _value.DoubleValue;
            }

            throw new ValueIsNotANumberException(this);
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
            hash = hash * 31 + _value.GetHashCode();
            if (null != _objectValue)
            {
                hash = hash * 31 + _objectValue.GetHashCode();
            }
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
                return IntegerValue == other.DoubleValue;
            }
            else if (Type == VariableValueType.Double && other.Type == VariableValueType.Integer)
            {
                return DoubleValue == other.IntegerValue;
            }

            throw new ArgumentOutOfRangeException(nameof(other));
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

            throw new ValueIsNotANumberException(a);
        }

        public static bool operator !(VariableValue a)
        {
            if (!a.IsBoolean)
            {
                throw new ValueIsNotABooleanException(a);
            }

            return !a.BooleanValue;
        }

        public static VariableValue operator +(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue + b.IntegerValue);
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return new VariableValue(a.AsNumber() + b.AsNumber());
            }
            else if (a.Type == VariableValueType.String && b.Type == VariableValueType.String)
            {
                return new VariableValue(a.StringValue + b.StringValue);
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static VariableValue operator -(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue - b.IntegerValue);
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return new VariableValue(a.AsNumber() - b.AsNumber());
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static VariableValue operator *(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue * b.IntegerValue);
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return new VariableValue(a.AsNumber() * b.AsNumber());
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static VariableValue operator /(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return new VariableValue(a.IntegerValue / b.IntegerValue);
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return new VariableValue(a.AsNumber() / b.AsNumber());
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static bool operator <(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue < b.IntegerValue;
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return a.AsNumber() < b.AsNumber();
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static bool operator >(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue > b.IntegerValue;
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return a.AsNumber() > b.AsNumber();
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static bool operator <=(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue <= b.IntegerValue;
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return a.AsNumber() <= b.AsNumber();
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static bool operator >=(VariableValue a, VariableValue b)
        {
            if (a.Type == VariableValueType.Integer && b.Type == VariableValueType.Integer)
            {
                return a.IntegerValue >= b.IntegerValue;
            }
            else if (a.IsNumber && b.IsNumber)
            {
                return a.AsNumber() >= b.AsNumber();
            }

            throw new VariableValueValueOperationException(a, b);
        }

        public static bool operator &(VariableValue a, VariableValue b)
        {
            if (!a.IsBoolean || !b.IsBoolean)
            {
                throw new VariableValueValueOperationException(a, b);
            }

            return a.BooleanValue & b.BooleanValue;
        }

        public static bool operator |(VariableValue a, VariableValue b)
        {
            if (!a.IsBoolean || !b.IsBoolean)
            {
                throw new VariableValueValueOperationException(a, b);
            }

            return a.BooleanValue | b.BooleanValue;
        }

        public static bool operator true(VariableValue a)
        {
            if (!a.IsBoolean)
            {
                throw new ValueIsNotABooleanException(a);
            }

            return a.BooleanValue;
        }

        public static bool operator false(VariableValue a)
        {
            if (!a.IsBoolean)
            {
                throw new ValueIsNotABooleanException(a);
            }

            return !a.BooleanValue;
        }

        public static readonly VariableValue True = new VariableValue(true);

        public static readonly VariableValue False = new VariableValue(false);
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

    #region Exceptions

    public abstract class VariableValueValueException : Exception
    {
        public readonly VariableValue Value;

        public VariableValueValueException(VariableValue value) : base() => Value = value;
    }

    public class ValueIsNotABooleanException : VariableValueValueException
    {
        public ValueIsNotABooleanException(VariableValue value) : base(value) { }
    }

    public class ValueIsNotANumberException : VariableValueValueException
    {
        public ValueIsNotANumberException(VariableValue value) : base(value) { }
    }

    public class VariableValueValueOperationException : Exception
    {
        public readonly VariableValue ValueA;
        public readonly VariableValue ValueB;

        public VariableValueValueOperationException(VariableValue valueA, VariableValue valueB) : base()
        {
            ValueA = valueA;
            ValueB = valueB;
        }
    }

    #endregion
}
