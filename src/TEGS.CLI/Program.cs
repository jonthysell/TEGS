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
using System.Linq;
using System.Reflection;
using System.Text;

namespace TEGS.CLI
{
    class Program
    {
        static ProgramArgs ProgramArgs { get; set; }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0)
            {
                ShowHelp();
            }
            else
            {
                try
                {
                    ProgramArgs = ParseArgs(args);

                    Simulation simulation = new Simulation(ProgramArgs.SimulationArgs);

                    int numTraceExpressions = ProgramArgs.SimulationArgs.TraceExpressions.Count;

                    if (ProgramArgs.ShowOutput)
                    {
                        Console.WriteLine("TEGS.CLI v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
                        Console.WriteLine();

                        int? columnWidth = (Console.WindowWidth / (numTraceExpressions + 2)) - 1; // 

                        simulation.VertexFired += MakeOutputEventHandler(Console.Write, numTraceExpressions, " ", columnWidth);
                    }

                    if (null != ProgramArgs.OutputWriter)
                    {
                        simulation.VertexFired += MakeOutputEventHandler(ProgramArgs.OutputWriter.Write, numTraceExpressions);
                    }

                    simulation.Run();
                }
                catch (ValidationException ex)
                {
                    Console.Error.WriteLine($"Graph has {ex.ValidationErrors.Count} errors:");

                    foreach (var error in ex.ValidationErrors)
                    {
                        Console.Error.WriteLine($"  {error.Message}");
                    }
                }
                catch (AggregateException ex)
                {
                    PrintException(ex);
                    foreach (Exception innerEx in ex.InnerExceptions)
                    {
                        PrintException(innerEx);
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
                finally
                {
                    ProgramArgs?.OutputWriter?.Flush();
                    ProgramArgs?.OutputWriter?.Close();
                }
            }
        }

        private static VertexFiredEventHandler MakeOutputEventHandler(Action<string> writer, int numTraceVariables, string seperator = "\t", int? columnWidth = null)
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

        static void ShowHelp()
        {
            Console.WriteLine("TEGS.CLI v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
            Console.WriteLine();

            Console.WriteLine("Usage:");
            Console.WriteLine("TEGS.CLI.exe [command] [options] graph.xml");
            Console.WriteLine();

            Console.WriteLine("Commands:");
            Console.WriteLine("run                                Run the simulation with the given graph.");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine("-o, --output-file [file]           Write the output to the given file.");
            Console.WriteLine("--seed [int]                       Set the starting seed for the simulation.");
            Console.WriteLine("--show-output                      Show simulation output to the console.");
            Console.WriteLine("--start-parameter [expression]     Add a simulation start parameter.");
            Console.WriteLine("--stop-condition [expression]      Stop the simulation if the given condition is met.");
            Console.WriteLine("--stop-event-count [name] [count]  Stop the simulation if the named event occurs count times.");
            Console.WriteLine("--stop-time [time]                 Stop the simulation if the clock passes the given time.");
            Console.WriteLine("--trace-variable [name]            Add a trace variable by name.");
            Console.WriteLine();
        }

        static void PrintException(Exception ex)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine();
            Console.Error.WriteLine("Error: {0}", ex.Message);
            Console.Error.WriteLine(ex.StackTrace);

            Console.ForegroundColor = oldColor;

            if (null != ex.InnerException)
            {
                PrintException(ex.InnerException);
            }
        }

        static ProgramArgs ParseArgs(string[] args)
        {
            Graph graph = null;

            try
            {
                string graphFile = args[args.Length - 1];

                using (FileStream fs = new FileStream(graphFile, FileMode.Open))
                {
                    graph = Graph.LoadXml(fs);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load graph.", ex);
            }

            IReadOnlyList<ValidationError> validationErrors = Validator.Validate(graph);

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(validationErrors);
            }

            try
            {
                switch (args[0].ToLower())
                {
                    case "run":
                        break;
                    default:
                        throw new Exception($"Did not recognize command \"{args[0]}\"");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse command.", ex);
            }

            string outputFile = null;
            bool showOutput = false;
            int? startingSeed = null;
            List<string> startParameters = new List<string>();
            StopCondition stopCondition = null;

            List<TraceExpression> traceExpressions = new List<TraceExpression>();

            try
            {
                for (int i = 1; i < args.Length - 1; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-o":
                        case "--output-file":
                            outputFile = args[++i];
                            break;
                        case "--seed":
                            startingSeed = int.Parse(args[++i]);
                            break;
                        case "--show-output":
                            showOutput = true;
                            break;
                        case "--start-parameter":
                            startParameters.Add(args[++i]);
                            break;
                        case "--stop-condition":
                            stopCondition = StopCondition.StopOnCondition(args[++i]);
                            break;
                        case "--stop-event-count":
                            stopCondition = StopCondition.StopAfterMaxEventCount(args[++i], int.Parse(args[++i]));
                            break;
                        case "--stop-time":
                            stopCondition = StopCondition.StopAfterMaxTime(int.Parse(args[++i]));
                            break;
                        case "--trace-variable":
                            string name = args[++i];
                            traceExpressions.Add(new StateVariableTraceExpression(graph.GetStateVariable(name)));
                            break;
                        default:
                            throw new Exception($"Did not recognize option \"{args[i]}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse options.", ex);
            }

            ProgramArgs programArgs = new ProgramArgs(graph)
            {
                ShowOutput = showOutput
            };

            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                programArgs.OutputWriter = new StreamWriter(new FileStream(outputFile, FileMode.Create), Encoding.UTF8);
            }

            programArgs.SimulationArgs.StartingSeed = startingSeed;
            programArgs.SimulationArgs.StartParameterExpressions = startParameters;
            programArgs.SimulationArgs.StopCondition = stopCondition;

            programArgs.SimulationArgs.TraceExpressions.AddRange(traceExpressions);

            return programArgs;
        }
    }
}
