// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
