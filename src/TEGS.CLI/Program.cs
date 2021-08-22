// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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

                if (ex.InnerException is not null)
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

                Console.Error.WriteLine(ex.ValidationErrors.Count == 1 ? "There was a validation error:" : $"There were {ex.ValidationErrors.Count} validation errors:");
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
            Console.WriteLine("Usage: tegs-cli build [<options>] graph.json");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("--output-path [path]    Output generated files into the given path (Default: Current)");
            Console.WriteLine("--project-file [name]   Specify the name of the generated project file");
            Console.WriteLine("--source-file [name]    Specify the name of the generated C# source file");
            Console.WriteLine("--source-only           Generate just the C# source file, directly into the output path");
            Console.WriteLine("--trace-variable [name] Add a trace variable by name");
            Console.WriteLine();
        }

        private void ParseBuildArgs()
        {
            Graph graph;
            string graphFile;

            try
            {
                graphFile = Arguments[^1];

                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.Load(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            string outputPath = null;
            string projectFile = null;
            string sourceFile = null;
            bool sourceOnly = false;
            List<string> traceExpressions = new List<string>();

            try
            {
                for (int i = 1; i < Arguments.Length - 1; i++)
                {
                    switch (Arguments[i].ToLower())
                    {
                        case "--output-path":
                            outputPath = Arguments[++i];
                            break;
                        case "--project-file":
                            projectFile = Arguments[++i];
                            break;
                        case "--source-file":
                            sourceFile = Arguments[++i];
                            break;
                        case "--source-only":
                            sourceOnly = true;
                            break;
                        case "--trace-variable":
                            string name = Arguments[++i];
                            traceExpressions.Add(graph.GetStateVariable(name).Name);
                            break;
                        default:
                            throw new Exception($"Did not recognize option \"{ Arguments[i] }\".");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to parse options.", ex);
            }

            var buildCommandArgs = new BuildCommandArgs(graph, graphFile)
            {
                OutputPath = outputPath,
                ProjectFile = projectFile,
                SourceFile = sourceFile,
                SourceOnly = sourceOnly,
            };

            buildCommandArgs.TraceExpressions.AddRange(traceExpressions);

            ProgramArgs = buildCommandArgs;
        }

        private void ExecuteBuildCommand()
        {
            var args = ProgramArgs as BuildCommandArgs;

            ValidateLoadedGraph();

            if (!ScriptingHost.TrySymbolify(args.Graph.Name, false, out string targetNamespace))
            {
                if (!ScriptingHost.TrySymbolify(args.GraphFile, false, out targetNamespace))
                {
                    targetNamespace = "TegsGenerated";
                }
            }

            string code = CodeGenerator.GenerateSource(args.Graph, targetNamespace, args.TraceExpressions);

            string rootPath = args.OutputPath ?? ".";
            string sourceFile = args.SourceFile ?? "Program.cs";

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            File.WriteAllText(Path.Combine(rootPath, sourceFile), code, Encoding.UTF8);

            if (!args.SourceOnly)
            {
                string projectXml = CodeGenerator.GenerateProject(targetNamespace);

                string projectFile = args.ProjectFile ?? $"{ targetNamespace }.csproj";

                File.WriteAllText(Path.Combine(rootPath, projectFile), projectXml, Encoding.UTF8);
            }
        }

        #endregion

        #region Run Command

        private static void ShowRunHelp()
        {
            Console.WriteLine("Usage: tegs-cli run [<options>] graph.json");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("-o, --output-file [file]           Write simulation output to the given file");
            Console.WriteLine("--seed [int]                       Set the starting seed for the simulation (default: random)");
            Console.WriteLine("--silent                           Do not write simulation output to the console");
            Console.WriteLine("--skip-validation                  Improve start-up performance by skipping graph validation");
            Console.WriteLine("--start-parameter [expression]     Set a simulation start parameter");
            Console.WriteLine("--stop-condition [expression]      Stop the simulation if the given condition is met");
            Console.WriteLine("--stop-event-count [name] [count]  Stop the simulation if the named event occurs count times");
            Console.WriteLine("--stop-time [time]                 Stop the simulation if the clock passes the given time");
            Console.WriteLine("--trace-variable [name]            Add a trace variable by name");
            Console.WriteLine();
        }

        private void ParseRunArgs()
        {
            Graph graph;
            string graphFile;

            try
            {
                graphFile = Arguments[^1];
                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.Load(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            string outputFile = null;
            bool silent = false;
            int? startingSeed = null;
            List<string> startParameters = new List<string>();
            StopCondition stopCondition = null;
            bool skipValidation = false;

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
                        case "--silent":
                            silent = true;
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
                        case "--skip-validation":
                            skipValidation = true;
                            break;
                        default:
                            throw new Exception($"Did not recognize option \"{ Arguments[i] }\".");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to parse options.", ex);
            }

            var runCommandArgs = new RunCommandArgs(graph, graphFile)
            {
                OutputFile = outputFile,
                Silent = silent,
                SkipValidation = skipValidation,
            };

            runCommandArgs.SimulationArgs.StartingSeed = startingSeed;
            runCommandArgs.SimulationArgs.StartParameterExpressions.AddRange(startParameters);
            runCommandArgs.SimulationArgs.StopCondition = stopCondition;

            runCommandArgs.SimulationArgs.TraceExpressions.AddRange(traceExpressions);

            ProgramArgs = runCommandArgs;
        }

        private void ExecuteRunCommand()
        {
            var args = ProgramArgs as RunCommandArgs;

            if (!args.SkipValidation)
            {
                ValidateLoadedGraph();
            }

            Simulation simulation = new Simulation(args.SimulationArgs);

            if (!args.SkipValidation)
            {
                ValidateSimulation(simulation);
            }

            using StreamWriter outputWriter = args.OutputFile is not null ? new StreamWriter(new FileStream(args.OutputFile, FileMode.Create), Encoding.UTF8) : null;

            int numTraceExpressions = args.SimulationArgs.TraceExpressions.Count;

            if (!args.Silent)
            {
                int? columnWidth = (Console.WindowWidth / (numTraceExpressions + 2)) - 1;

                simulation.VertexFired += MakeOutputEventHandler(Console.Write, " ", columnWidth);
            }

            if (outputWriter is not null)
            {
                simulation.VertexFired += MakeOutputEventHandler(outputWriter.Write);
            }

            simulation.Run();
            simulation.Wait();
        }

        private static VertexFiredEventHandler MakeOutputEventHandler(Action<string> writer, string separator = "\t", int? columnWidth = null)
        {
            bool hasHeader = false;

            return (sender, e) =>
            {
                if (!hasHeader)
                {
                    writer(Truncate("Clock", columnWidth));

                    writer(separator);
                    writer(Truncate("Event", columnWidth));

                    for (int i = 0; i < e.TraceExpressions.Count; i++)
                    {
                        writer(separator);
                        writer(Truncate(e.TraceExpressions[i].Label, columnWidth));
                    }

                    writer(Environment.NewLine);
                    hasHeader = true;
                }

                writer(Truncate(e.Clock.ToString(), columnWidth));

                writer(separator);
                writer(Truncate(e.Vertex.Name.ToString(), columnWidth));

                for (int i = 0; i < e.TraceExpressions.Count; i++)
                {
                    writer(separator);
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
            Console.WriteLine("Usage: tegs-cli validate graph.json");
            Console.WriteLine();
        }

        private void ParseValidateArgs()
        {
            Graph graph;
            string graphFile;

            try
            {
                graphFile = Arguments[^1];

                using FileStream fs = new FileStream(graphFile, FileMode.Open);
                graph = Graph.Load(fs);
            }
            catch (Exception ex)
            {
                throw new ParseArgumentsException("Unable to load graph.", ex);
            }

            ProgramArgs = new ValidateCommandArgs(graph, graphFile);
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

        private static void ValidateSimulation(Simulation simulation)
        {
            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(simulation);

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(validationErrors);
            }
        }

        #endregion

        #region Version

        private static void ShowVersion()
        {
            Console.WriteLine($"{ AppInfo.Name } v{ AppInfo.Version }");
            Console.WriteLine();
        }

        #endregion

        #region Help

        private static void ShowHelp(string command = null)
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
                    Console.WriteLine("Usage: tegs-cli [--version] [--help]");
                    Console.WriteLine("                <command> [<args>]");
                    Console.WriteLine();

                    Console.WriteLine("Commands:");
                    Console.WriteLine("build     Build stand-alone code for a given graph");
                    Console.WriteLine("run       Run a simulation with a given graph");
                    Console.WriteLine("validate  Validate a given graph");
                    Console.WriteLine();

                    Console.WriteLine("See 'tegs-cli help <command>' to see the arguments for that command.");
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
