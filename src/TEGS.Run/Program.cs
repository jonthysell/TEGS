// 
// Program.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
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
using System.Reflection;
using System.Text;
using System.Xml;

namespace TEGS.Run
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

                    int numTraceVariables = ProgramArgs.SimulationArgs.TraceVariables.Count;

                    if (ProgramArgs.ShowOutput)
                    {
                        Console.WriteLine("TEGS.Run v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
                        Console.WriteLine();

                        int? columnWidth = (Console.WindowWidth / (numTraceVariables + 2)) - 1; // 

                        simulation.VertexFired += MakeOutputEventHandler(Console.WriteLine, numTraceVariables, " ", columnWidth);
                    }

                    if (null != ProgramArgs.OutputWriter)
                    {
                        simulation.VertexFired += MakeOutputEventHandler(ProgramArgs.OutputWriter.WriteLine, numTraceVariables);
                    }

                    simulation.Run();
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

        private static VertexFiredEventHandler MakeOutputEventHandler(Action<string> writeLine, int numTraceVariables, string seperator = "\t", int? columnWidth = null)
        {
            bool hasHeader = false;

            string[] headerItems = new string[numTraceVariables + 2];
            headerItems[0] = "Clock";
            headerItems[1] = "Event";

            string[] dataItems = new string[numTraceVariables + 2];

            return (sender, e) =>
            {
                dataItems[0] = e.Clock.ToString();
                dataItems[1] = e.Vertex.Name.ToString();

                if (null != e.TraceVariables)
                {

                    for (int i = 0; i < e.TraceVariables.Count; i++)
                    {
                        if (!hasHeader)
                        {
                            headerItems[i + 2] = e.TraceVariables[i].Name;
                        }
                        dataItems[i + 2] = e.TraceVariables[i].Value.ToString();
                    }
                }

                if (!hasHeader)
                {
                    writeLine(GetLine(headerItems, seperator, columnWidth));
                    hasHeader = true;
                }

                writeLine(GetLine(dataItems, seperator, columnWidth));
            };
        }

        private static string GetLine(string[] items, string seperator, int? columnWidth)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < items.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(seperator);
                }

                string value = columnWidth.HasValue ? items[i].PadRight(columnWidth.Value).Substring(0, columnWidth.Value) : items[i];
                sb.Append(value);
            }

            return sb.ToString();
        }

        static void ShowHelp()
        {
            Console.WriteLine("TEGS.Run v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
            Console.WriteLine();

            Console.WriteLine("Usage:");
            Console.WriteLine("TEGS.Run.exe [options] graph.xml");
            Console.WriteLine();

            Console.WriteLine("Options:");

            Console.WriteLine("-o, --output-file [file]     Write simulation output to the given file.");
            Console.WriteLine("--seed [int]                 The starting seed for the simulation.");
            Console.WriteLine("--show-output                Show simulation output to the console.");
            Console.WriteLine("--start-parameters [string]  The simulation start parameters.");
            Console.WriteLine("--stop-condition [string]    The simulation stop condition.");
            Console.WriteLine("--trace-boolean [string]     Adds a boolean trace variable by name.");
            Console.WriteLine("--trace-double [string]      Adds a double trace variable by name.");
            Console.WriteLine("--trace-string [string]      Adds a string trace variable by name.");
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

                using (XmlReader reader = XmlReader.Create(graphFile))
                {
                    graph = Graph.LoadXml(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load graph.", ex);
            }

            
            string outputFile = null;
            bool showOutput = false;
            int? startingSeed = null;
            string startParameters = null;
            string stopCondition = null;

            List<TraceVariable> traceVariables = new List<TraceVariable>();

            try
            {
                for (int i = 0; i < args.Length - 1; i++)
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
                        case "--start-parameters":
                            startParameters = args[++i];
                            break;
                        case "--stop-condition":
                            stopCondition = args[++i];
                            break;
                        case "--trace-boolean":
                            traceVariables.Add(new TraceVariable(args[++i], TraceVariableType.Boolean));
                            break;
                        case "--trace-double":
                            traceVariables.Add(new TraceVariable(args[++i], TraceVariableType.Double));
                            break;
                        case "--trace-string":
                            traceVariables.Add(new TraceVariable(args[++i], TraceVariableType.String));
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

            ProgramArgs programArgs = new ProgramArgs(graph);

            programArgs.ShowOutput = showOutput;

            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                programArgs.OutputWriter = new StreamWriter(new FileStream(outputFile, FileMode.Create), Encoding.UTF8);
            }

            programArgs.SimulationArgs.StartingSeed = startingSeed;
            programArgs.SimulationArgs.StartParameters = startParameters;
            programArgs.SimulationArgs.StopCondition = stopCondition;

            programArgs.SimulationArgs.TraceVariables.AddRange(traceVariables);

            return programArgs;
        }
    }
}
