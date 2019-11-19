// 
// ScheduledEvent.cs
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
    public class ScheduledEvent : IComparable<ScheduledEvent>
    {
        public Vertex Target { get; private set; }

        public double Time { get; private set; }

        public double Priority { get; private set; }

        public string ParameterValues { get; private set; }

        public ScheduledEvent(Vertex target, double time, double priority, string parameterValues)
        {
            Time = time;
            Target = target;
            Priority = priority;
            ParameterValues = parameterValues;
        }

        public int CompareTo(ScheduledEvent other)
        {
            if (null == other)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
