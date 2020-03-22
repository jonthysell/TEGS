// 
// Edge.cs
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

namespace TEGS
{
    public class Edge : ICloneable<Edge>
    {
        public Vertex Source { get; set; }

        public Vertex Target { get; set; }

        public EdgeAction Action { get; set; } = EdgeAction.Schedule;

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

        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value?.Trim() ?? "";
            }
        }
        private string _condition = "";

        public string Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value?.Trim() ?? "";
            }
        }
        private string _delay = "";

        public string Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value?.Trim() ?? "";
            }
        }
        private string _priority = "";

        public readonly List<string> ParameterExpressions = new List<string>();

        public Edge() { }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Description))
            {
                return Description;
            }

            return base.ToString();
        }

        public Edge Clone()
        {
            var clone = new Edge()
            {
                Action = Action,
                Description = Description,
                Condition = Condition,
                Delay = Delay,
                Priority = Priority,
            };

            foreach (var paramExpression in ParameterExpressions)
            {
                clone.ParameterExpressions.Add(paramExpression);
            }

            return clone;
        }
    }

    public enum EdgeAction
    {
        Schedule,
        CancelNext,
        CancelAll
    }
}
