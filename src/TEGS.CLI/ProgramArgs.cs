// 
// ProgramArgs.cs
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
using System.IO;

namespace TEGS.CLI
{
    public abstract class ProgramArgs
    {
        public readonly Graph Graph;

        public ProgramArgs(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
    }

    public class BuildCommandArgs : ProgramArgs
    {
        public string OutputSourcePath { get; set; } = null;

        public BuildCommandArgs(Graph graph) : base(graph) { }
    }

    public class RunCommandArgs : ProgramArgs, IDisposable
    {
        public readonly SimulationArgs SimulationArgs;

        public bool ShowOutput { get; set; } = false;

        public bool SkipValidation { get; set; } = false;

        public StreamWriter OutputWriter { get; set; } = null;

        public RunCommandArgs(Graph graph) : base(graph)
        {
            SimulationArgs = new SimulationArgs(graph);
        }

        public void Dispose()
        {
            OutputWriter?.Flush();
            OutputWriter?.Close();
            OutputWriter?.Dispose();
        }
    }

    public class ValidateCommandArgs : ProgramArgs
    {
        public ValidateCommandArgs(Graph graph) : base(graph) { }
    }
}
