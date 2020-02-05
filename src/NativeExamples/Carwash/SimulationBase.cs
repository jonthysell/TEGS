// 
// SimulationBase.cs
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

namespace Carwash
{
    struct ScheduleEntry : IComparable<ScheduleEntry>
    {
        public double Time;
        public double Priority;
        public EventType EventType;
        public int[] ParameterValues;

        public int CompareTo(ScheduleEntry other)
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
            return $"{EventType.ToString()} @ {Time:f3}";
        }
    }

    struct StopCondition
    {
        public double MaxTime;
    }

    struct SimulationArgs
    {
        public int Seed;
        public int[] ParameterValues;
        public StopCondition StopCondition;
    }

    abstract class SimulationBase
    {
        private double _clock = 0.0;

        private List<ScheduleEntry> _schedule = new List<ScheduleEntry>();

        protected Random Random;

        protected abstract EventType StartingEvent { get; }

        public void Run(SimulationArgs args)
        {
            Random = new Random(args.Seed);

            ScheduleEvent(StartingEvent, delay: 0, priority: 0, parameterValues: args.ParameterValues);

            while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
            {
                var entry = _schedule[0];
                _schedule.RemoveAt(0);

                _clock = entry.Time;

                ProcessEvent(entry.EventType, entry.ParameterValues);
            }
        }

        protected abstract void ProcessEvent(EventType eventType, int[] parameterValues);

        protected void ScheduleEvent(EventType eventType, double delay = 0.0, double priority = 0.0, int[] parameterValues = null)
        {
            var entry = new ScheduleEntry()
            {
                Time = _clock + delay,
                Priority = priority,
                EventType = eventType,
                ParameterValues = parameterValues
            };

            int index = _schedule.BinarySearch(entry);

            if (index < 0)
            {
                index = ~index;
            }

            if (index == _schedule.Count)
            {
                _schedule.Add(entry);
            }
            else
            {
                _schedule.Insert(index, entry);
            }
        }

        protected double Clock() => _clock;
    }

    public static class RandomExtensions
    {
        public static double UniformVariate(this Random random, double alpha, double beta)
        {
            return alpha + (beta - alpha) * random.NextDouble();
        }
    }
}
