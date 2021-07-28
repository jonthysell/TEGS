// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS
{
    public struct ScheduledEvent : IComparable<ScheduledEvent>
    {
        public readonly Vertex Target;

        public readonly double Time;

        public readonly double Priority;

        public readonly IReadOnlyList<VariableValue> ParameterValues;

        public ScheduledEvent(Vertex target, double time, double priority, IReadOnlyList<VariableValue> parameterValues)
        {
            Time = time;
            Target = target;
            Priority = priority;
            ParameterValues = parameterValues;
        }

        public int CompareTo(ScheduledEvent other)
        {
            int timeCompare = Time.CompareTo(other.Time);

            if (timeCompare != 0)
            {
                return timeCompare;
            }

            return Priority.CompareTo(other.Priority);
        }

        public override string ToString()
        {
            return $"{Target.Name} @ {Time:f3}";
        }
    }
}
