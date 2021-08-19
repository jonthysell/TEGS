// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS
{
    public class Vertex : ICloneable<Vertex>
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

        public string[] Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (value is null)
                {
                    _code = null;
                }
                else
                {
                    _code = new string[value.Length];
                    for (int i = 0; i < _code.Length; i++)
                    {
                        _code[i] = value[i].Trim();
                    }
                }
            }
        }
        private string[] _code = null;

        public readonly List<string> ParameterNames = new List<string>();

        public bool IsStartingVertex { get; set; }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public Vertex() { }

        public string GetCode()
        {
            return Code is not null ? string.Join(Environment.NewLine, Code) : null;
        }

        public void SetCode(string code)
        {
            Code = code?.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        public override string ToString()
        {
            return Name;
        }

        public Vertex Clone()
        {
            var clone = new Vertex()
            {
                Name = Name,
                Description = Description,
                IsStartingVertex = IsStartingVertex,
                X = X,
                Y = Y,
            };

            clone.SetCode(GetCode());

            foreach (var param in ParameterNames)
            {
                clone.ParameterNames.Add(param);
            }

            return clone;
        }

        private readonly char[] LineSeparators = new char[] { '\r', '\n', ';' };
    }
}
