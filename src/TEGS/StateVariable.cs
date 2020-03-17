// 
// StateVariable.cs
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

namespace TEGS
{
    public class StateVariable : IComparable<StateVariable>, IEquatable<StateVariable>, ICloneable<StateVariable>
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value?.Trim() ?? "";
            }
        }
        private string _name = "";

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value?.Trim() ?? "";
            }
        }
        private string _description = "";

        public VariableValueType Type { get; set; } = VariableValueType.Boolean;

        public StateVariable() { }

        public StateVariable Clone()
        {
            return new StateVariable()
            {
                Name = Name,
                Type = Type,
                Description = Description,
            };
        }

        public bool Equals(StateVariable other)
        {
            if (null == other)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return Name == other.Name
                && Type == other.Type
                && Description == other.Description;
        }

        public int CompareTo(StateVariable other)
        {
            int compareName = Name.CompareTo(other?.Name);

            if (compareName == 0)
            {
                int compareType = Type.CompareTo(other?.Type);

                if (compareType == 0)
                {
                    return Description.CompareTo(other?.Description);
                }

                return compareType;
            }

            return compareName;
        }

        public override bool Equals(object obj)
        {
            if (obj is StateVariable other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Name.GetHashCode();
            hash = hash * 31 + Type.GetHashCode();
            hash = hash * 31 + Description.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"{Name} ({Type.ToString()})";
        }
    }

    public abstract class StateVariableException : Exception
    {
        public readonly StateVariable StateVariable;

        public StateVariableException(StateVariable stateVariable) : base()
        {
            StateVariable = stateVariable;
        }
    }

    public class StateVariableNotFoundException : Exception
    {
        public readonly string Name;

        public readonly VariableValueType? Type;

        public StateVariableNotFoundException(string name) : base()
        {
            Name = name;
            Type = null;
        }

        public StateVariableNotFoundException(string name, VariableValueType type) : base()
        {
            Name = name;
            Type = type;
        }
    }
}
