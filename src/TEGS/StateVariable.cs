// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
