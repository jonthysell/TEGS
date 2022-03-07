// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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

        public readonly static StopCondition Never = new StopCondition(sim => false);

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

            return new StopCondition(sim => sim.CurrentVertex.Name == eventName && sim.EventCount[sim.Graph.Vertices.IndexOf(sim.CurrentVertex)] >= maxEventCount);
        }

        public static StopCondition StopOnCondition(string code)
        {
            code = code?.Trim();
            return new StopCondition(sim => sim.ScriptingHost.Evaluate(code, VariableValue.False).BooleanValue);
        }
    }
}
