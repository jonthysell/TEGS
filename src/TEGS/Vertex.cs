// 
// Vertex.cs
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
                if (null == value)
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
            return null != Code ? string.Join(Environment.NewLine, Code) : null;
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
