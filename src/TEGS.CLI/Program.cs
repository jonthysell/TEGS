// 
// Program.cs
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
using System.IO;
using System.Text;

namespace TEGS.CLI
{
    public class Program
    {
        #region Main Statics

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.OutputEncoding = Encoding.UTF8;

            var p = new Program(args);
            p.Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is AggregateException ex)
            {
                PrintException(ex, true);
                foreach (Exception innerEx in ex.InnerExceptions)
                {
                    PrintException(innerEx);
                }
            }
            else
            {
                PrintException(e.ExceptionObject as Exception, true);
            }
        }

        private static void PrintException(Exception ex, bool isUnhandled = false)
        {
            var oldColor = StartConsoleError();

            if (isUnhandled)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("An unhandled exception has occured.");
            }

            if (!(ex is null))
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine($"Exception: { ex.Message }");

#if DEBUG
                Console.Error.WriteLine(ex.StackTrace);
#endif

                EndConsoleError(oldColor);

                if (null != ex.InnerException)
                {
                    PrintException(ex.InnerException);
                }
            }
        }

        private static ConsoleColor StartConsoleError()
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            return oldColor;
        }

        private static void EndConsoleError(ConsoleColor oldColor)
        {
            Console.ForegroundColor = oldColor;
        }

        #endregion

        #region Properties

        public readonly string[] Arguments;

        public ProgramArgs ProgramArgs { get; private set; }

        #endregion

        public Program(string[] args)
        {
            Arguments = args;
        }

        public void Run()
        {
            if (Arguments is null || Arguments.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = Arguments[0].ToLower();

            try
            {
                switch (command)
                {
                    case "build":
                        ParseBuildArgs();
                        ExecuteBuildCommand();
                        break;
                    case "run":
                        ParseRunArgs();
                        ExecuteRunCommand();
                        break;
                    case "validate":
                        ParseValidateArgs();
                        ExecuteValidateCommand();
                        break;
                    case "version":
                    case "--version":
                        ShowVersion();
                        return;
                    case "help":
                    case "--help":
                    default:
                        ShowHelp(Arguments.Length > 1 ? Arguments[1]?.ToLower() : null);
                        return;
                }
            }
            catch (ValidationException ex)
            {
                var oldColor = StartConsoleError();

                Console.Error.WriteLine($"Graph has {ex.ValidationErrors.Count} validation errors:");

                foreach (var error in ex.ValidationErrors)
                {
                    Console.Error.WriteLine($"  {error.Message}");
                }

                EndConsoleError(oldColor);
            }
            catch (ParseArgumentsException ex)
            {
                PrintException(ex);

                Console.WriteLine();

                ShowHelp(command);
            }
        }

        #region Build Command

        private static void ShowBuildHelp()
        {
            Console.WriteLine("Usage: tegs build graph.xml");
            Console.WriteLine();
        }

        private void ParseBuildArgs()
        {
            Graph graph = null;

            try
            {
                string graphFile = Arguments[^1];

                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.LoadXml(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            ProgramArgs = new BuildCommandArgs(graph);
        }

        private void ExecuteBuildCommand()
        {

        }

        #endregion

        #region Run Command

        private static void ShowRunHelp()
        {
            Console.WriteLine("Usage: tegs run [<options>] graph.xml");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("-o, --output-file [file]           Write the output to the given file");
            Console.WriteLine("--seed [int]                       Set the starting seed for the simulation (default: random)");
            Console.WriteLine("--show-output                      Show simulation output to the console (default: False)");
            Console.WriteLine("--start-parameter [expression]     Set a simulation start parameter");
            Console.WriteLine("--stop-condition [expression]      Stop the simulation if the given condition is met");
            Console.WriteLine("--stop-event-count [name] [count]  Stop the simulation if the named event occurs count times");
            Console.WriteLine("--stop-time [time]                 Stop the simulation if the clock passes the given time");
            Console.WriteLine("--trace-variable [name]            Add a trace variable by name");
            Console.WriteLine("--validate-graph [bool]            Validating the graph before running (default: True)");
            Console.WriteLine();
        }

        private void ParseRunArgs()
        {
            Graph graph = null;

            try
            {
                string graphFile = Arguments[^1];

                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.LoadXml(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            string outputFile = null;
            bool showOutput = false;
            int? startingSeed = null;
            List<string> startParameters = new List<string>();
            StopCondition stopCondition = null;
            bool validateGraph = true;

            List<TraceExpression> traceExpressions = new List<TraceExpression>();

            try
            {
                for (int i = 1; i < Arguments.Length - 1; i++)
                {
                    switch (Arguments[i].ToLower())
                    {
                        case "-o":
                        case "--output-file":
                            outputFile = Arguments[++i];
                            break;
                        case "--seed":
                            startingSeed = int.Parse(Arguments[++i]);
                            break;
                        case "--show-output":
                            showOutput = true;
                            break;
                        case "--start-parameter":
                            startParameters.Add(Arguments[++i]);
                            break;
                        case "--stop-condition":
                            stopCondition = StopCondition.StopOnCondition(Arguments[++i]);
                            break;
                        case "--stop-event-count":
                            stopCondition = StopCondition.StopAfterMaxEventCount(Arguments[++i], int.Parse(Arguments[++i]));
                            break;
                        case "--stop-time":
                            stopCondition = StopCondition.StopAfterMaxTime(int.Parse(Arguments[++i]));
                            break;
                        case "--trace-variable":
                            string name = Arguments[++i];
                            traceExpressions.Add(new StateVariableTraceExpression(graph.GetStateVariable(name)));
                            break;
                        case "--validate-graph":
                            validateGraph = bool.Parse(Arguments[++i]);
                            break;
                        default:
                            throw new Exception($"Did not recognize option \"{Arguments[i]}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to parse options.", ex);
            }

            var runCommandArgs = new RunCommandArgs(graph)
            {
                ShowOutput = showOutput,
                ValidateGraph = validateGraph,
            };

            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                runCommandArgs.OutputWriter = new StreamWriter(new FileStream(outputFile, FileMode.Create), Encoding.UTF8);
            }

            runCommandArgs.SimulationArgs.StartingSeed = startingSeed;
            runCommandArgs.SimulationArgs.StartParameterExpressions = startParameters;
            runCommandArgs.SimulationArgs.StopCondition = stopCondition;

            runCommandArgs.SimulationArgs.TraceExpressions.AddRange(traceExpressions);

            ProgramArgs = runCommandArgs;
        }

        private void ExecuteRunCommand()
        {
            var args = ProgramArgs as RunCommandArgs;

            if (args.ValidateGraph)
            {
                ValidateLoadedGraph();
            }

            Simulation simulation = new Simulation(args.SimulationArgs);

            int numTraceExpressions = args.SimulationArgs.TraceExpressions.Count;

            if (args.ShowOutput)
            {
                int? columnWidth = (Console.WindowWidth / (numTraceExpressions + 2)) - 1;

                simulation.VertexFired += MakeOutputEventHandler(Console.Write, " ", columnWidth);
            }

            if (null != args.OutputWriter)
            {
                simulation.VertexFired += MakeOutputEventHandler(args.OutputWriter.Write);
            }

            simulation.Run();
            simulation.Wait();
        }

        private static VertexFiredEventHandler MakeOutputEventHandler(Action<string> writer, string seperator = "\t", int? columnWidth = null)
        {
            bool hasHeader = false;

            return (sender, e) =>
            {
                if (!hasHeader)
                {
                    writer(Truncate("Clock", columnWidth));

                    writer(seperator);
                    writer(Truncate("Event", columnWidth));

                    for (int i = 0; i < e.TraceExpressions.Count; i++)
                    {
                        writer(seperator);
                        writer(Truncate(e.TraceExpressions[i].Value.ToString(), columnWidth));
                    }

                    writer(Environment.NewLine);
                    hasHeader = true;
                }

                writer(Truncate(e.Clock.ToString(), columnWidth));

                writer(seperator);
                writer(Truncate(e.Vertex.Name.ToString(), columnWidth));

                for (int i = 0; i < e.TraceExpressions.Count; i++)
                {
                    writer(seperator);
                    writer(Truncate(e.TraceExpressions[i].Value.ToString(), columnWidth));
                }

                writer(Environment.NewLine);
            };
        }

        private static string Truncate(string s, int? width)
        {
            if (width.HasValue)
            {
                if (s.Length > width.Value)
                {
                    return s.Substring(0, width.Value);
                }
                else if (s.Length < width.Value)
                {
                    return s.PadRight(width.Value);
                }
            }

            return s;
        }

        #endregion

        #region Validate Command

        private static void ShowValidateHelp()
        {
            Console.WriteLine("Usage: tegs validate graph.xml");
            Console.WriteLine();
        }

        private void ParseValidateArgs()
        {
            Graph graph = null;

            try
            {
                string graphFile = Arguments[^1];

                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.LoadXml(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            ProgramArgs = new ValidateCommandArgs(graph);
        }

        private void ExecuteValidateCommand()
        {
            ValidateLoadedGraph();

            Console.WriteLine($"Graph is valid.");
        }

        private void ValidateLoadedGraph()
        {
            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(ProgramArgs.Graph);

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(validationErrors);
            }
        }

        #endregion

        #region Version

        private void ShowVersion()
        {
            Console.WriteLine($"{ AppInfo.Name } v{ AppInfo.Version }");
            Console.WriteLine();
        }

        #endregion

        #region Help

        private void ShowHelp(string command = null)
        {
            switch (command)
            {
                case "build":
                    ShowBuildHelp();
                    break;
                case "run":
                    ShowRunHelp();
                    break;
                case "validate":
                    ShowValidateHelp();
                    break;
                default:
                    Console.WriteLine("Usage: tegs [--version] [--help]");
                    Console.WriteLine("            <command> [<args>]");
                    Console.WriteLine();

                    Console.WriteLine("Commands:");
                    Console.WriteLine("run       Run a simulation with a given graph");
                    Console.WriteLine("validate  Validate a given graph");
                    Console.WriteLine();

                    Console.WriteLine("See 'tegs help <command>' to see the arguments for that command.");
                    break;
            }
        }

        #endregion
    }

    #region Exceptions

    public class ParseArgumentsException : Exception
    {
        public ParseArgumentsException(string message) : base(message) { }

        public ParseArgumentsException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}
