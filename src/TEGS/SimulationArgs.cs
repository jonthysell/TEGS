// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace TEGS
{
    public class SimulationArgs
    {
        public Graph Graph { get; private set; }

        public int? StartingSeed { get; set; } = null;

        public List<string> StartParameterExpressions { get; private set; } = new List<string>();

        public StopCondition StopCondition { get; set; } = StopCondition.Never;

        public List<TraceExpression> TraceExpressions { get; private set; } = new List<TraceExpression>();

        public SimulationArgs(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
    }
}
