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

namespace TEGS
{
    public class TraceVariable
    {
        public string Name { get; private set; }

        public TraceVariableType Type { get; private set; }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (null == value)
                {
                    _value = null;
                }
                else
                {
                    if ((null != value as bool?) && (Type == TraceVariableType.Boolean))
                    {
                        _value = value;
                    }
                    else if ((null != value as double?) && (Type == TraceVariableType.Double))
                    {
                        _value = value;
                    }
                    else if ((null != value as string) && (Type == TraceVariableType.String))
                    {
                        _value = value;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }
        private object _value = null;

        TraceVariable(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name.Trim();
        }

        public TraceVariable(string name, TraceVariableType type, object value = null) : this(name)
        {
            Type = type;
            Value = value;
        }

        public TraceVariable(string name, bool value) : this(name, TraceVariableType.Boolean, value) { }

        public TraceVariable(string name, double value) : this(name, TraceVariableType.Double, value) { }

        public TraceVariable(string name, string value) : this(name, TraceVariableType.String, value) { }

        public TraceVariable Clone()
        {
            return new TraceVariable(Name, Type, Value);
        }
    }

    public enum TraceVariableType
    {
        Boolean,
        Double,
        String
    }
}
