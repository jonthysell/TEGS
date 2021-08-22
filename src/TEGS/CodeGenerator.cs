// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace TEGS
{
    public static class CodeGenerator
    {
        public static string GenerateProject(string targetNamespace)
        {
            if (string.IsNullOrWhiteSpace(targetNamespace))
            {
                throw new ArgumentNullException(nameof(targetNamespace));
            }

            return string.Format(ProjectTemplate.TrimStart(), targetNamespace);
        }

        private const string ProjectTemplate = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <StartupObject>{0}.Program</StartupObject>
  </PropertyGroup>

</Project>
";

        public static string GenerateSource(Graph graph, string targetNamespace, IEnumerable<string> traceExpressions = null)
        {
            if (graph is null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (string.IsNullOrWhiteSpace(targetNamespace))
            {
                throw new ArgumentNullException(nameof(targetNamespace));
            }

            StringBuilder sb = new StringBuilder();
            int indent = 0;

            // Header
            AddHeader(sb, graph, traceExpressions, ref indent);

            // Usings
            sb.AppendLine();
            AddUsings(sb);

            // Namespace
            sb.AppendLine();
            StartBlock(sb, $"namespace { targetNamespace }", ref indent);

            // Graph Code
            WriteGraphCode(sb, graph, traceExpressions, ref indent);

            // Common Code
            sb.AppendLine();
            WriteCommonCode(sb, ref indent);

            EndBlock(sb, ref indent); // namespace

            return sb.ToString();
        }

        public static string RewriteExpression(Graph graph, string expression, bool rewriteSymbols = true)
        {
            StringBuilder sb = new StringBuilder();

            TokenReader tr = new TokenReader(expression);
            while (tr.CurrentToken != TokenType.End)
            {
                sb.Append(RewriteToken(tr, graph, rewriteSymbols));
                tr.ReadNext();
            }

            return sb.ToString();
        }

        private static string RewriteToken(TokenReader tokenReader, Graph graph, bool rewriteSymbol)
        {
            switch (tokenReader.CurrentToken)
            {
                case TokenType.Add:
                    return " + ";
                case TokenType.Subtract:
                    return " - ";
                case TokenType.Multiply:
                    return " * ";
                case TokenType.Divide:
                    return " / ";
                case TokenType.OpenParens:
                    return "(";
                case TokenType.CloseParens:
                    return ")";
                case TokenType.Comma:
                    return ", ";
                case TokenType.Assign:
                    return " = ";
                case TokenType.LessThan:
                    return " < ";
                case TokenType.GreaterThan:
                    return " > ";
                case TokenType.LessThanEquals:
                    return " <= ";
                case TokenType.GreaterThanEquals:
                    return " >= ";
                case TokenType.Not:
                    return "!";
                case TokenType.Equals:
                    return " == ";
                case TokenType.NotEquals:
                    return " != ";
                case TokenType.And:
                    return " & ";
                case TokenType.Or:
                    return " | ";
                case TokenType.ConditionalAnd:
                    return " && ";
                case TokenType.ConditionalOr:
                    return " || ";
                case TokenType.Symbol:
                    string symbol = tokenReader.CurrentSymbol;
                    if (graph.HasStateVariable(symbol))
                    {
                        return rewriteSymbol ? $"{ StateVariableRewritePrefix }{ symbol }" : symbol;
                    }
                    else
                    {
                        return rewriteSymbol && FunctionRewriteMap.TryGetValue(symbol, out string result) ? result : symbol;
                    }
                case TokenType.Value:
                    return tokenReader.CurrentSymbol; // Contains the literal value read by the parser
            }

            return null;
        }

        private const string StateVariableRewritePrefix = "StateVariable_";

        private static readonly Dictionary<string, string> FunctionRewriteMap = new Dictionary<string, string>()
        {
            {  "String.Length", "String_Length" },
        };

        #region Header

        private static void AddHeader(StringBuilder sb, Graph graph, IEnumerable<string> traceExpressions, ref int indent)
        {
            WriteComment(sb, $"Generated with { AppInfo.Name } v{ AppInfo.Version }", ref indent);
            WriteComment(sb, "", ref indent);

            if (!string.IsNullOrWhiteSpace(graph.Name))
            {
                WriteComment(sb, $"Name: { graph.Name }", ref indent);
            }

            if (!string.IsNullOrWhiteSpace(graph.Description))
            {
                WriteComment(sb, $"Description: { graph.Description }", ref indent);
            }

            WriteComment(sb, "", ref indent);

            WriteComment(sb, $"Outputs:", ref indent);
            WriteComment(sb, $"Clock", ref indent);
            WriteComment(sb, $"Event", ref indent);

            if (traceExpressions is not null)
            {
                foreach (var traceExpression in traceExpressions)
                {
                    WriteComment(sb, RewriteExpression(graph, traceExpression, false), ref indent);
                }
            }
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
            "System.IO",
            "System.Text"
        };

        #endregion

        #region Graph Code

        private static void WriteGraphCode(StringBuilder sb, Graph graph, IEnumerable<string> traceExpressions, ref int indent)
        {
            StartBlock(sb, "enum EventType", ref indent);

            var eventNames = new Dictionary<Vertex, string>();
            var eventParameterTypes = new Dictionary<Vertex, string>();

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                int paramCount = vertex.ParameterNames.Count;
                if (paramCount > 0 && !eventParameterTypes.ContainsKey(vertex))
                {
                    StringBuilder tupleTypesSB = new StringBuilder();

                    tupleTypesSB.Append("Tuple<");

                    for (int j = 0; j < paramCount; j++)
                    {
                        if (j > 0)
                        {
                            tupleTypesSB.Append(", ");
                        }

                        tupleTypesSB.Append(GetStateVariableType(graph.GetStateVariable(vertex.ParameterNames[i])));
                    }

                    tupleTypesSB.Append('>');

                    eventParameterTypes[vertex] = tupleTypesSB.ToString();
                }

                if (!eventNames.TryGetValue(vertex, out string eventName))
                {
                    if (!ScriptingHost.TrySymbolify(vertex.Name, false, out string result))
                    {
                        result = i.ToString();
                    }
                    eventName = $"{ EventNamePrefix }{ result }";
                    eventNames[vertex] = eventName;
                }

                WriteCode(sb, $"{ eventName } = { i },", ref indent);
            }

            EndBlock(sb, ref indent); // enum EventType

            sb.AppendLine();
            StartBlock(sb, "class Simulation : SimulationBase", ref indent);

            Vertex startingVertex = graph.StartingVertex;

            WriteCode(sb, $"protected override EventType StartingEventType => EventType.{ eventNames[startingVertex] };", ref indent);

            if (graph.StateVariables.Count > 0)
            {
                sb.AppendLine();
                WriteComment(sb, "State Variables", ref indent);

                for (int i = 0; i < graph.StateVariables.Count; i++)
                {
                    StateVariable stateVariable = graph.StateVariables[i];
                    WriteCode(sb, $"{ GetStateVariableType(stateVariable) } { RewriteExpression(graph, stateVariable.Name) } = default;", ref indent);
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

                WriteCode(sb, $"protected override object ParseStartParameters(string[] startParameters) => Tuple.Create({ tupleValuesSB});", ref indent);
            }

            sb.AppendLine();
            StartBlock(sb, "protected override void ProcessEvent(EventType eventType, object parameterValues)", ref indent);

            StartBlock(sb, "switch (eventType)", ref indent);

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                StartBlock(sb, $"case EventType.{ eventNames[vertex] }:", ref indent, false);

                WriteCode(sb, $"{ eventNames[vertex] }({ (vertex.ParameterNames.Count > 0 ? $"({ eventParameterTypes[vertex] })parameterValues" : "") });", ref indent);

                WriteCode(sb, "break;", ref indent);

                EndBlock(sb, ref indent, false); // case
            }

            EndBlock(sb, ref indent); // switch

            EndBlock(sb, ref indent); // protected override void ProcessEvent

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                sb.AppendLine();

                WriteComment(sb, $"Event #{ i }: { vertex.Name }", ref indent);
                if (!string.IsNullOrWhiteSpace(vertex.Description))
                {
                    WriteComment(sb, $"Description: { vertex.Description }", ref indent);
                }

                int paramCount = vertex.ParameterNames.Count;

                StartBlock(sb, $"private void { eventNames[vertex] }({ (paramCount > 0 ? $"{ eventParameterTypes[vertex] } parameterValues" : "") })", ref indent);

                bool addSpacing = false;

                if (paramCount > 0)
                {
                    WriteComment(sb, "Parameters", ref indent);
                    for (int j = 0; j < paramCount; j++)
                    {
                        WriteCode(sb, $"{ RewriteExpression(graph, vertex.ParameterNames[j]) } = parameterValues.Item{ j + 1 };", ref indent);
                    }

                    addSpacing = true;
                }

                string[] code = vertex.Code;
                if (code is not null && code.Length > 0)
                {
                    if (addSpacing)
                    {
                        sb.AppendLine();
                    }

                    WriteComment(sb, "Event Code", ref indent);
                    for (int j = 0; j < code.Length; j++)
                    {
                        WriteCode(sb, $"{ RewriteExpression(graph, code[j]) };", ref indent);
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

                            WriteComment(sb, $"Edge #{ j }: { edge.Action} { edge.Source.Name } to { edge.Target.Name }", ref indent);

                            if (!string.IsNullOrWhiteSpace(edge.Description))
                            {
                                WriteComment(sb, $"Description: { edge.Description }", ref indent);
                            }
                        }

                        bool hasCondition = !string.IsNullOrEmpty(edge.Condition);
                        if (hasCondition)
                        {
                            StartBlock(sb, $"if ({ RewriteExpression(graph, edge.Condition) })", ref indent);
                        }

                        StringBuilder parameterValuesSB = new StringBuilder();

                        if (edge.ParameterExpressions.Count == 0)
                        {
                            parameterValuesSB.Append("null");
                        }
                        else
                        {
                            parameterValuesSB.Append($"new { eventParameterTypes[vertex] }(");
                            for (int k = 0; k < edge.ParameterExpressions.Count; k++)
                            {
                                if (k > 0)
                                {
                                    parameterValuesSB.Append(", ");
                                }

                                parameterValuesSB.Append(RewriteExpression(graph, edge.ParameterExpressions[k]));
                            }
                            parameterValuesSB.Append(')');
                        }

                        switch(edge.Action)
                        {
                            case EdgeAction.Schedule:
                                WriteCode(sb, $"ScheduleEvent(EventType.{ eventNames[edge.Target] }, { (string.IsNullOrEmpty(edge.Delay) ? "0" : RewriteExpression(graph, edge.Delay)) }, { (string.IsNullOrEmpty(edge.Priority) ? "0" : RewriteExpression(graph, edge.Priority)) }, { parameterValuesSB});", ref indent);
                                break;
                            case EdgeAction.CancelNext:
                                WriteCode(sb, $"CancelNextEvent(EventType.{ eventNames[edge.Target] }, { parameterValuesSB});", ref indent);
                                break;
                            case EdgeAction.CancelAll:
                                WriteCode(sb, $"CancelAllEvents(EventType.{ eventNames[edge.Target] }, { parameterValuesSB});", ref indent);
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

            sb.AppendLine();
            StartBlock(sb, "protected override string GetEventName(EventType eventType)", ref indent);

            StartBlock(sb, "switch (eventType)", ref indent);

            for (int i = 0; i < graph.Verticies.Count; i++)
            {
                Vertex vertex = graph.Verticies[i];

                StartBlock(sb, $"case EventType.{ eventNames[vertex] }:", ref indent, false);

                WriteCode(sb, $"return \"{ eventNames[vertex].Substring(EventNamePrefix.Length) }\";", ref indent);

                EndBlock(sb, ref indent, false); // case
            }

            EndBlock(sb, ref indent); // switch

            WriteCode(sb, $"return \"\";", ref indent);

            EndBlock(sb, ref indent); // protected override string GetEventName

            sb.AppendLine();
            StartBlock(sb, "protected override void TraceExpressionHeaders(bool traceToConsole, StreamWriter outputWriter)", ref indent);

            if (traceExpressions is not null)
            {
                foreach (string traceExpression in traceExpressions)
                {
                    WriteCode(sb, $"Trace(traceToConsole, outputWriter, \"\\t{ RewriteExpression(graph, traceExpression, false) }\");", ref indent);
                }
            }

            EndBlock(sb, ref indent); // protected override void TraceExpressionHeaders

            sb.AppendLine();
            StartBlock(sb, "protected override void TraceExpressionValues(bool traceToConsole, StreamWriter outputWriter)", ref indent);

            if (traceExpressions is not null)
            {
                foreach (string traceExpression in traceExpressions)
                {
                    WriteCode(sb, $"Trace(traceToConsole, outputWriter, $\"\\t{{ { RewriteExpression(graph, traceExpression) } }}\");", ref indent);
                }
            }

            EndBlock(sb, ref indent); // protected override void TraceExpressionValues

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

        private const string EventNamePrefix = "Event_";

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
                case ""-o"":
                case ""--output-file"":
                    simArgs.OutputFile = args[++i];
                    break;
                case ""--seed"":
                    simArgs.Seed = int.Parse(args[++i]);
                    break;
                case ""--silent"":
                    simArgs.Silent = true;
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
    public EventType EventType;
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
    public string OutputFile;
    public int Seed;
    public bool Silent;
    public string[] StartParameterValues;
    public StopCondition StopCondition;
}

abstract class SimulationBase
{
    private double _clock = 0.0;

    private readonly List<ScheduleEntry> _schedule = new List<ScheduleEntry>();

    protected Random Random;

    protected abstract EventType StartingEventType { get; }

    public void Run(SimulationArgs args)
    {
        Random = new Random(args.Seed);

        ScheduleEvent(StartingEventType, 0, 0, ParseStartParameters(args.StartParameterValues));

        bool traceToConsole = !args.Silent;
        using StreamWriter outputWriter = args.OutputFile is not null ? new StreamWriter(new FileStream(args.OutputFile, FileMode.Create), Encoding.UTF8) : null;

        StartTraceHeader(traceToConsole, outputWriter);
        TraceExpressionHeaders(traceToConsole, outputWriter);
        EndTraceLine(traceToConsole, outputWriter);

        while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
        {
            var entry = _schedule[0];
            _schedule.RemoveAt(0);

            _clock = entry.Time;

            ProcessEvent(entry.EventType, entry.ParameterValues);

            StartTrace(traceToConsole, outputWriter, entry.EventType);
            TraceExpressionValues(traceToConsole, outputWriter);
            EndTraceLine(traceToConsole, outputWriter);
        }
    }

    protected virtual object ParseStartParameters(string[] startParameters) => null;

    protected abstract void ProcessEvent(EventType eventType, object parameterValues);

    protected abstract string GetEventName(EventType eventType);

    protected void Trace(bool traceToConsole, StreamWriter outputWriter, string str)
    {
        if (traceToConsole)
        {
            Console.Write(str);
        }
        outputWriter?.Write(str);
    }

    private void StartTraceHeader(bool traceToConsole, StreamWriter outputWriter)
    {
        Trace(traceToConsole, outputWriter, ""Clock\tEvent"");
    }

    private void StartTrace(bool traceToConsole, StreamWriter outputWriter, EventType eventType)
    {
        Trace(traceToConsole, outputWriter, $""{ _clock }\t{ GetEventName(eventType) }"");
    }

    protected abstract void TraceExpressionHeaders(bool traceToConsole, StreamWriter outputWriter);

    protected abstract void TraceExpressionValues(bool traceToConsole, StreamWriter outputWriter);

    private void EndTraceLine(bool traceToConsole, StreamWriter outputWriter)
    {
        if (traceToConsole)
        {
            Console.WriteLine();
        }

        outputWriter?.WriteLine();
    }

    protected void ScheduleEvent(EventType eventType, double delay, double priority, object parameterValues)
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

    protected void CancelNextEvent(EventType eventType, object parameterValues)
    {
        for (int i = 0; i < _schedule.Count; i++)
        {
            if (CancelPredicate(_schedule[i], eventType, parameterValues))
            {
                _schedule.RemoveAt(i);
                break;
            }
        }
    }

    protected void CancelAllEvents(EventType eventType, object parameterValues)
    {
        _schedule.RemoveAll(entry => CancelPredicate(entry, eventType, parameterValues));
    }

    private static bool CancelPredicate(ScheduleEntry match, EventType eventType, object parameterValues)
    {
        return match.EventType == eventType &&
            (parameterValues is null || (match.ParameterValues is not null && match.ParameterValues.Equals(parameterValues)));
    }

    protected double Clock() => _clock;

    protected static int String_Length(string str) => str.Length;
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
        double z;
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
