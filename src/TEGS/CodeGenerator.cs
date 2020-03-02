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
using System.Collections.Generic;
using System.Text;

namespace TEGS
{
    public static class CodeGenerator
    {
        public static string Generate(Graph graph, string targetNamespace = null)
        {
            if (null == graph)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (string.IsNullOrWhiteSpace(targetNamespace))
            {
                targetNamespace = ScriptingHost.IsValidSymbolName(graph.Name, true) ? graph.Name : "TegsGenerated";
            }
            else if (!ScriptingHost.IsValidSymbolName(targetNamespace, true))
            {
                throw new ArgumentOutOfRangeException(nameof(targetNamespace));
            }

            StringBuilder sb = new StringBuilder();
            int indent = 0;

            // Header
            AddHeader(sb, graph, ref indent);

            // Usings
            sb.AppendLine();
            AddUsings(sb);

            // Namespace
            sb.AppendLine();
            StartBlock(sb, $"namespace { targetNamespace }", ref indent);

            // Graph Code
            WriteGraphCode(sb, graph, ref indent);

            // Common Code
            sb.AppendLine();
            WriteCommonCode(sb, ref indent);

            EndBlock(sb, ref indent); // namespace

            return sb.ToString();
        }

        #region Header

        private static void AddHeader(StringBuilder sb, Graph graph, ref int indent)
        {
            WriteComment(sb, $"Generated with { AppInfo.Name } v{ AppInfo.Version }", ref indent);
            WriteComment(sb, "", ref indent);
            WriteComment(sb, $"Name: { graph.Name }", ref indent);
            WriteComment(sb, $"Description: { graph.Description }", ref indent);
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

            Vertex startingVertex = graph.StartingVertex;

            WriteCode(sb, $"protected override int StartingEventId => { startingVertex.Id };", ref indent);

            if (graph.StateVariables.Count > 0)
            {
                sb.AppendLine();
                WriteComment(sb, "State Variables", ref indent);

                for (int i = 0; i < graph.StateVariables.Count; i++)
                {
                    StateVariable stateVariable = graph.StateVariables[i];
                    WriteCode(sb, $"{ GetStateVariableType(stateVariable) } { stateVariable.Name } = default;", ref indent);
                }
            }

            sb.AppendLine();
            WriteCode(sb, "public Simulation() { }", ref indent);

            if (startingVertex.ParameterNames.Count > 0)
            {
                sb.AppendLine();

                StringBuilder tupleValuesSB = new StringBuilder();

                for (int i = 0; i < startingVertex.ParameterNames.Count; i++)
                {
                    if (i > 0)
                    {
                        tupleValuesSB.Append(", ");
                    }

                    string type = GetStateVariableType(graph.GetStateVariable(startingVertex.ParameterNames[i]));

                    switch (type)
                    {
                        case "bool":
                        case "int":
                        case "double":
                            tupleValuesSB.Append($"{ type }.Parse(startParameters[{ i }])");
                            break;
                        default:
                            tupleValuesSB.Append($"startParameters[{ i }]");
                            break;
                    }
                }

                WriteCode(sb, $"protected override object ParseStartParameters(string[] startParameters) => Tuple.Create({ tupleValuesSB.ToString() });", ref indent);
            }

            sb.AppendLine();
            StartBlock(sb, "protected override void ProcessEvent(int eventId, object parameterValues)", ref indent);

            StartBlock(sb, "switch (eventId)", ref indent);

            var eventParameterTypes = new Dictionary<Vertex, string>();

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                int paramCount = vertex.ParameterNames.Count;

                string eventParameterType = null;
                if (paramCount > 0 && !eventParameterTypes.TryGetValue(vertex, out eventParameterType))
                {
                    StringBuilder tupleTypesSB = new StringBuilder();

                    tupleTypesSB.Append("Tuple<");

                    for (int j = 0; j < paramCount; j++)
                    {
                        if (j > 0)
                        {
                            tupleTypesSB.Append(", ");
                        }

                        tupleTypesSB.Append(GetStateVariableType(graph.GetStateVariable(startingVertex.ParameterNames[i])));
                    }

                    tupleTypesSB.Append(">");

                    eventParameterType = tupleTypesSB.ToString();
                    eventParameterTypes[vertex] = eventParameterType;
                }

                StartBlock(sb, $"case { i }:", ref indent, false);

                WriteCode(sb, $"Event{ i }({ (paramCount > 0 ? $"({ eventParameterType })parameterValues" : "") });", ref indent);

                WriteCode(sb, "break;", ref indent);

                EndBlock(sb, ref indent, false); // case
            }

            EndBlock(sb, ref indent); // switch

            EndBlock(sb, ref indent); // protected override void ProcessEvent

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                sb.AppendLine();

                WriteComment(sb, $"Event #{ i }", ref indent);
                WriteComment(sb, $"Name: { vertex.Name }", ref indent);
                WriteComment(sb, $"Description: { vertex.Description }", ref indent);

                int paramCount = vertex.ParameterNames.Count;

                StartBlock(sb, $"private void Event{ i }({ (paramCount > 0 ? $"{ eventParameterTypes[vertex] } parameterValues" : "") })", ref indent);

                bool addSpacing = false;

                if (paramCount > 0)
                {
                    WriteComment(sb, "Parameters", ref indent);
                    for (int j = 0; j < paramCount; j++)
                    {
                        WriteCode(sb, $"{ vertex.ParameterNames[j] } = parameterValues.Item{ j + 1 };", ref indent);
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

                    WriteComment(sb, "Event Code", ref indent);
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

                            WriteComment(sb, $"Edge #{ j }", ref indent);
                            WriteComment(sb, $"Action: { edge.Action.ToString() }", ref indent);
                            WriteComment(sb, $"Direction: { edge.Source.Name } to { edge.Target.Name }", ref indent);                            
                            WriteComment(sb, $"Description: { edge.Description }", ref indent);
                        }

                        bool hasCondition = !string.IsNullOrEmpty(edge.Condition);
                        if (hasCondition)
                        {
                            StartBlock(sb, $"if ({ edge.Condition })", ref indent);
                        }

                        string parameterValues = edge.ParameterExpressions.Count == 0 ? "null" : $"new { eventParameterTypes[vertex] }({ string.Join(", ", edge.ParameterExpressions) })";

                        switch(edge.Action)
                        {
                            case EdgeAction.Schedule:
                                WriteCode(sb, $"ScheduleEvent({ edge.Target.Id }, { (string.IsNullOrEmpty(edge.Delay) ? "0" : edge.Delay) }, { (string.IsNullOrEmpty(edge.Priority) ? "0" : edge.Priority) }, { parameterValues });", ref indent);
                                break;
                            case EdgeAction.CancelNext:
                                WriteCode(sb, $"CancelNextEvent({ edge.Target.Id }, { parameterValues });", ref indent);
                                break;
                            case EdgeAction.CancelAll:
                                WriteCode(sb, $"CancelAllEvents({ edge.Target.Id }, { parameterValues });", ref indent);
                                break;
                        }

                        if (hasCondition)
                        {
                            EndBlock(sb, ref indent); // if condition
                        }

                        addSpacing = true;
                    }
                }

                EndBlock(sb, ref indent); // private void Event
            }

            EndBlock(sb, ref indent); // class Simulation
        }

        private static string GetStateVariableType(StateVariable stateVariable)
        {
            switch (stateVariable.Type)
            {
                case VariableValueType.Boolean:
                    return "bool";
                case VariableValueType.Integer:
                    return "int";
                case VariableValueType.Double:
                    return "double";
                case VariableValueType.String:
                    return "string";
                default:
                    return "object";
            }
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
        var startValues = new List<string>();

        for (int i = 0; i < args.Length - 1; i++)
        {
            switch (args[i].ToLower())
            {
                case ""--seed"":
                    simArgs.Seed = int.Parse(args[++i]);
                    break;
                case ""--start-parameter"":
                    startValues.Add(args[++i]);
                    break;
                case ""--stop-time"":
                    simArgs.StopCondition.MaxTime = double.Parse(args[++i]);
                    break;
                default:
                    throw new Exception($""Did not recognize option \""{args[i]}\"""");
            }
        }

        if (startValues.Count > 0)
        {
            simArgs.StartParameterValues = startValues.ToArray();
        }

        return simArgs;
    }
}

struct ScheduleEntry : IComparable<ScheduleEntry>
{
    public double Time;
    public double Priority;
    public int EventId;
    public object ParameterValues;

    public int CompareTo(ScheduleEntry other)
    {
        int timeCompare = Time.CompareTo(other.Time);

        if (timeCompare != 0)
        {
            return timeCompare;
        }

        return Priority.CompareTo(other.Priority);
    }
}

struct StopCondition
{
    public double MaxTime;
}

struct SimulationArgs
{
    public int Seed;
    public string[] StartParameterValues;
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

        ScheduleEvent(StartingEventId, 0, 0, ParseStartParameters(args.StartParameterValues));

        while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
        {
            var entry = _schedule[0];
            _schedule.RemoveAt(0);

            _clock = entry.Time;

            ProcessEvent(entry.EventId, entry.ParameterValues);
        }
    }

    protected virtual object ParseStartParameters(string[] startParameters) => null;

    protected abstract void ProcessEvent(int eventId, object parameterValues);

    protected void ScheduleEvent(int eventId, double delay, double priority, object parameterValues)
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

    protected void CancelNextEvent(int eventId, object parameterValues)
    {
        for (int i = 0; i < _schedule.Count; i++)
        {
            if (CancelPredicate(_schedule[i], eventId, parameterValues))
            {
                _schedule.RemoveAt(i);
                break;
            }
        }
    }

    protected void CancelAllEvents(int eventId, object parameterValues)
    {
        _schedule.RemoveAll(entry => CancelPredicate(entry, eventId, parameterValues));
    }

    private static bool CancelPredicate(ScheduleEntry match, int eventId, object parameterValues)
    {
        return match.EventId == eventId &&
            (null == parameterValues || (null != match.ParameterValues && match.ParameterValues.Equals(parameterValues)));
    }

    protected double Clock() => _clock;
}

public static class RandomExtensions
{
    public static double UniformVariate(this Random random, double alpha, double beta)
    {
        return alpha + (beta - alpha) * random.NextDouble();
    }

    public static double ExponentialVariate(this Random random, double lambda)
    {
        return -Math.Log(1.0 - random.NextDouble()) / lambda;
    }

    public static double NormalVariate(this Random random, double mu, double sigma)
    {
        double z = 0.0;

        while (true)
        {
            double u1 = random.NextDouble();
            double u2 = 1.0 - random.NextDouble();

            z = (4 * Math.Exp(-0.5) / Math.Sqrt(2.0)) * (u1 - 0.5) / u2;

            if ((z * z / 4.0) <= -Math.Log(u2))
            {
                break;
            }
        }

        return mu + z * sigma;
    }

    public static double LogNormalVariate(this Random random, double mu, double sigma)
    {
        return Math.Exp(random.NormalVariate(mu, sigma));
    }
}

public static class StringExtensions
{
    public static int Length(this string str) => str.Length;
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

        private static void WriteComment(StringBuilder sb, string[] comments, ref int indent)
        {
            string padding = GetIndentPadding(indent);

            for (int i = 0; i < comments.Length; i++)
            {
                sb.Append(padding);
                if (string.IsNullOrWhiteSpace(comments[i]))
                {
                    sb.AppendLine("//");
                }
                else
                {
                    sb.Append("// ");
                    sb.AppendLine(comments[i]);
                }
            }
        }

        private static void WriteComment(StringBuilder sb, string comment, ref int indent)
        {
            string[] commentLines = comment.Trim().Split(LineSeperators, StringSplitOptions.None);

            WriteComment(sb, commentLines, ref indent);
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

            if (!string.IsNullOrWhiteSpace(code))
            {
                sb.Append(padding);
                sb.AppendLine(code);
            }

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
