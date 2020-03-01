// 
// CodeGenerator.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2020 Jon Thysell <http://jonthysell.com>
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
using System.Text;

namespace TEGS
{
    public static class CodeGenerator
    {
        public static string Generate(Graph graph)
        {
            if (null == graph)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            StringBuilder sb = new StringBuilder();
            int indent = 0;

            // Header
            AddHeader(sb, graph);

            // Usings
            sb.AppendLine();
            AddUsings(sb);

            // Namespace
            sb.AppendLine();
            StartBlock(sb, "namespace TEGS", ref indent);

            // Graph Code
            WriteGraphCode(sb, graph, ref indent);

            // Common Code
            sb.AppendLine();
            WriteCommonCode(sb, ref indent);

            EndBlock(sb, ref indent); // namespace

            return sb.ToString();
        }

        #region Header

        private static void AddHeader(StringBuilder sb, Graph graph)
        {
            sb.AppendLine($"// Generated with { AppInfo.Name } v{ AppInfo.Version }");
            sb.AppendLine("//");
            sb.AppendLine($"// Name: { graph.Name }");
            sb.AppendLine($"// Description: { graph.Description }");
            sb.AppendLine("//");
        }

        #endregion

        #region Usings

        private static void AddUsings(StringBuilder sb)
        {
            for (int i = 0; i < UsedNamespaces.Length; i++)
            {
                sb.AppendLine($"using { UsedNamespaces[i] };");
            }
        }

        private static readonly string[] UsedNamespaces = new[]
        {
            "System",
            "System.Collections.Generic",
            "System.Text"
        };

        #endregion

        #region Graph Code

        private static void WriteGraphCode(StringBuilder sb, Graph graph, ref int indent)
        {
            StartBlock(sb, "class Simulation : SimulationBase", ref indent);

            WriteCode(sb, $"protected override int StartingEventId => { graph.StartingVertex.Id };", ref indent);

            WriteCode(sb, "// State Variables", ref indent);

            for (int i = 0; i < graph.StateVariables.Count; i++)
            {
                StateVariable stateVariable = graph.StateVariables[i];

                string type = null;
                switch (stateVariable.Type)
                {
                    case VariableValueType.Boolean:
                        type = "bool";
                        break;
                    case VariableValueType.Integer:
                        type = "int";
                        break;
                    case VariableValueType.Double:
                        type = "double";
                        break;
                    case VariableValueType.String:
                        type = "string";
                        break;
                }

                WriteCode(sb, $"{ type } { stateVariable.Name } = default;", ref indent);
            }

            sb.AppendLine();
            WriteCode(sb, "public Simulation() { }", ref indent);

            sb.AppendLine();
            StartBlock(sb, "protected override void ProcessEvent(int eventId, int[] parameterValues)", ref indent);

            StartBlock(sb, "switch (eventId)", ref indent);

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                StartBlock(sb, $"case { i }: // Event { vertex.Name }", ref indent, false);

                WriteCode(sb, $"Event{ i }({ (vertex.ParameterNames.Count > 0 ? "parameterValues" : "") });", ref indent);

                WriteCode(sb, "break;", ref indent);

                EndBlock(sb, ref indent, false); // case
            }

            EndBlock(sb, ref indent); // switch

            EndBlock(sb, ref indent); // protected override void ProcessEvent

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                sb.AppendLine();

                WriteCode(sb, $"// Event { vertex.Name }", ref indent);

                int paramCount = vertex.ParameterNames.Count;

                StartBlock(sb, $"private void Event{ i }({ (paramCount > 0 ? "int[] parameterValues" : "") })", ref indent);

                bool addSpacing = false;

                if (paramCount > 0)
                {
                    WriteCode(sb, "// Parameters", ref indent);
                    for (int j = 0; j < paramCount; j++)
                    {
                        WriteCode(sb, $"{ vertex.ParameterNames[j] } = parameterValues[{ j }];", ref indent);
                    }

                    addSpacing = true;
                }

                string[] code = vertex.Code;
                if (null != code && code.Length > 0)
                {
                    if (addSpacing)
                    {
                        sb.AppendLine();
                    }

                    WriteCode(sb, "// Event Code", ref indent);
                    for (int j = 0; j < code.Length; j++)
                    {
                        WriteCode(sb, $"{ code[j] };", ref indent);
                    }

                    addSpacing = true;
                }

                for (int j = 0; j < graph.Edges.Count; j++)
                {
                    Edge edge = graph.Edges[j];

                    if (vertex == edge.Source)
                    {
                        if (addSpacing)
                        {
                            sb.AppendLine();

                            WriteCode(sb, $"// Edge { j } to { edge.Target.Name }", ref indent);
                        }

                        bool hasCondition = !string.IsNullOrEmpty(edge.Condition);
                        if (hasCondition)
                        {
                            StartBlock(sb, $"if ({ edge.Condition })", ref indent);
                        }

                        if (edge.Action == EdgeAction.Schedule)
                        {
                            string parameterValues = edge.ParameterExpressions.Count == 0 ? "null" : $"new int[] {{ { string.Join(", ", edge.ParameterExpressions ) } }}";
                            WriteCode(sb, $"ScheduleEvent({ edge.Target.Id }, delay: ({ edge.Delay }), priority: ({ edge.Priority }), parameterValues: { parameterValues });", ref indent);
                        }

                        if (hasCondition)
                        {
                            EndBlock(sb, ref indent); // if condition
                        }
                    }
                }

                EndBlock(sb, ref indent); // private void Event
            }

            EndBlock(sb, ref indent); // class Simulation
        }

        #endregion

        #region Common Code

        private static void WriteCommonCode(StringBuilder sb, ref int indent)
        {
            WriteCode(sb, CommonCode, ref indent);
        }

        private const string CommonCode = @"
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var simArgs = ParseArgs(args);

        var sim = new Simulation();
        sim.Run(simArgs);
    }

    static SimulationArgs ParseArgs(string[] args)
    {
        var simArgs = new SimulationArgs();
        var startValues = new List<int>()

        for (int i = 0; i < args.Length - 1; i++)
        {
            switch (args[i].ToLower())
            {
                case ""--seed"":
                    simArgs.Seed = int.Parse(args[++i]);
                    break;
                case ""--start-parameter"":
                    startValues.Add(int.Parse(args[++i]));
                    break;
                case ""--stop-time"":
                    simArgs.StopCondition.MaxTime = double.Parse(args[++i]);
                    break;
                default:
                    throw new Exception($""Did not recognize option \""{args[i]}\"""");
            }
        }

        simArgs.ParameterValues = startValues.ToArray();

        return simArgs;
    }
}

struct ScheduleEntry : IComparable<ScheduleEntry>
{
    public double Time;
    public double Priority;
    public int EventId;
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
        return $""Event { EventId } @ { Time:f3 }"";
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

    protected abstract int StartingEventId { get; }

    public void Run(SimulationArgs args)
    {
        Random = new Random(args.Seed);

        ScheduleEvent(StartingEvent, delay: 0, priority: 0, parameterValues: args.ParameterValues);

        while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
        {
            var entry = _schedule[0];
            _schedule.RemoveAt(0);

            _clock = entry.Time;

            ProcessEvent(entry.EventId, entry.ParameterValues);
        }
    }

    protected abstract void ProcessEvent(int eventId, int[] parameterValues);

    protected void ScheduleEvent(int eventId, double delay = 0.0, double priority = 0.0, int[] parameterValues = null)
    {
        var entry = new ScheduleEntry()
        {
            Time = _clock + delay,
            Priority = priority,
            EventId = eventId,
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
";

        #endregion

        #region Helpers

        private static void WriteCode(StringBuilder sb, string[] code, ref int indent)
        {
            string padding = GetIndentPadding(indent);

            for (int i = 0; i < code.Length; i++)
            {
                sb.Append(padding);
                sb.AppendLine(code[i]);
            }
        }

        private static void WriteCode(StringBuilder sb, string code, ref int indent)
        {
            string[] codeLines = code.Trim().Split(LineSeperators, StringSplitOptions.None);

            WriteCode(sb, codeLines, ref indent);
        }

        private static readonly string[] LineSeperators = new[] { "\r\n", "\r", "\n" };

        private static string GetIndentPadding(int indent)
        {
            return indent > 0 ? new string(' ', indent * SpacesPerIndent) : "";
        }

        private const int SpacesPerIndent = 4;

        private static void StartBlock(StringBuilder sb, string code, ref int indent, bool addBrace = true)
        {
            string padding = GetIndentPadding(indent);

            sb.Append(padding);
            sb.AppendLine(code);

            if (addBrace)
            {
                sb.Append(padding);
                sb.AppendLine("{");
            }

            indent++;
        }

        private static void EndBlock(StringBuilder sb, ref int indent, bool addBrace = true)
        {
            indent--;

            if (addBrace)
            {
                string padding = GetIndentPadding(indent);
                sb.Append(padding);
                sb.AppendLine("}");
            }
        }

        #endregion
    }
}
