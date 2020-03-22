// 
// StopCondition.cs
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
    public class StopCondition
    {
        public bool ShouldStop(Simulation simulation)
        {
            return _stopCondition(simulation);
        }

        protected Func<Simulation, bool> _stopCondition;

        protected StopCondition(Func<Simulation, bool> stopCondition)
        {
            _stopCondition = stopCondition ?? throw new ArgumentNullException(nameof(stopCondition));
        }

        public static StopCondition Never = _never ?? (_never = new StopCondition(sim => false));
        private static StopCondition _never;

        public static StopCondition StopAfterMaxTime(double maxTime)
        {
            if (maxTime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTime));
            }

            return new StopCondition(sim => sim.Clock >= maxTime);
        }

        public static StopCondition StopAfterMaxEventCount(string eventName, int maxEventCount)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentOutOfRangeException(nameof(eventName));
            }

            if (maxEventCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxEventCount));
            }

            return new StopCondition(sim => sim.CurrentVertex.Name == eventName && sim.EventCount[sim.Graph.Verticies.IndexOf(sim.CurrentVertex)] >= maxEventCount);
        }

        public static StopCondition StopOnCondition(string code)
        {
            code = code?.Trim();
            return new StopCondition(sim => sim.ScriptingHost.Evaluate(code, VariableValue.False).BooleanValue);
        }
    }
}
