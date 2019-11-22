// 
// TraceVariable.cs
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
using System.Runtime.InteropServices;

namespace TEGS
{
    public struct TraceVariable
    {
        public readonly string Name;

        public readonly TraceVariableType Type;

        public bool BooleanValue => _value.BooleanValue;

        public int IntegerValue => _value.IntegerValue;

        public double DoubleValue => _value.DoubleValue;

        public string StringValue => (string)_objectValue;

        private readonly PrimitiveUnionValue _value;
        private readonly object _objectValue;

        public TraceVariable(string name, TraceVariableType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name.Trim();
            Type = type;
            _value = new PrimitiveUnionValue();
            _objectValue = null;
        }

        public TraceVariable(string name, bool value) : this(name, TraceVariableType.Boolean)
        {
            _value.BooleanValue = value;
        }

        public TraceVariable(string name, int value) : this(name, TraceVariableType.Integer)
        {
            _value.IntegerValue = value;
        }

        public TraceVariable(string name, double value) : this(name, TraceVariableType.Double)
        {
            _value.DoubleValue = value;
        }

        public TraceVariable(string name, string value) : this(name, TraceVariableType.String)
        {
            _objectValue = value;
        }

        public string GetValueString()
        {
            switch (Type)
            {
                case TraceVariableType.Boolean:
                    return _value.BooleanValue.ToString();
                case TraceVariableType.Integer:
                    return _value.IntegerValue.ToString();
                case TraceVariableType.Double:
                    return _value.DoubleValue.ToString();
                default:
                    return _objectValue?.ToString();
            }
        }

        public override string ToString()
        {
            return $"{Name} = {GetValueString()}";
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PrimitiveUnionValue
        {
            [FieldOffset(0)] public bool BooleanValue;
            [FieldOffset(0)] public int IntegerValue;
            [FieldOffset(0)] public double DoubleValue;
        }
    }

    public enum TraceVariableType
    {
        Boolean,
        Integer,
        Double,
        String
    }
}