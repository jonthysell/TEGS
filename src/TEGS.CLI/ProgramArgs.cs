// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS.CLI
{
    public abstract class ProgramArgs
    {
        public readonly Graph Graph;
        public readonly string GraphFile;

        public ProgramArgs(Graph graph, string graphFile)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
            GraphFile = graphFile ?? throw new ArgumentNullException(nameof(graphFile));
        }
    }

    public class BuildCommandArgs : ProgramArgs
    {
        public string OutputPath { get; set; } = null;

        public string ProjectFile { get; set; } = null;

        public string SourceFile { get; set; } = null;

        public bool SourceOnly { get; set; } = false;

        public BuildCommandArgs(Graph graph, string graphFile) : base(graph, graphFile) { }
    }

    public class RunCommandArgs : ProgramArgs
    {
        public readonly SimulationArgs SimulationArgs;

        public bool Silent { get; set; } = false;

        public bool SkipValidation { get; set; } = false;

        public string OutputFile { get; set; } = null;

        public RunCommandArgs(Graph graph, string graphFile) : base(graph, graphFile)
        {
            SimulationArgs = new SimulationArgs(graph);
        }
    }

    public class ValidateCommandArgs : ProgramArgs
    {
        public ValidateCommandArgs(Graph graph, string graphFile) : base(graph, graphFile) { }
    }
}
