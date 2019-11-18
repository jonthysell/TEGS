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

                    if (ProgramArgs.ShowOutput)
                    {
                        Console.WriteLine("TEGS.Run v{0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
                        Console.WriteLine();

                        simulation.VertexFired += MakeOutputToConsoleEventHandler();
                    }

                    if (null != ProgramArgs.OutputWriter)
                    {
                        simulation.VertexFired += MakeOutputToOutputFileEventHandler();
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

        static VertexFiredEventHandler MakeOutputToConsoleEventHandler()
        {
            bool hasHeader = false;
            int columnWidth = 8;

            List<object> headerItems = new List<object>()
            {
                "Clock", "Event"
            };

            return (sender, e) =>
            {
                List<object> dataItems = new List<object>()
                {
                    e.Clock, e.Vertex.Name,
                };

                if (null != e.TraceVariables)
                {
                    foreach (TraceVariable tv in e.TraceVariables)
                    {
                        if (!hasHeader)
                        {
                            headerItems.Add(tv.Name);
                        }
                        dataItems.Add(tv.Value);
                    }
                }

                if (!hasHeader)
                {
                    Console.WriteLine(GetLine(" ", columnWidth, headerItems));
                    hasHeader = true;
                }

                Console.WriteLine(GetLine(" ", columnWidth, dataItems));
            };
        }

        private static string GetLine(string seperator, int columnWidth, IList<object> items)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(seperator);
                }
                sb.Append(items[i].ToString().PadRight(columnWidth).Substring(0, columnWidth));
            }

            return sb.ToString();
        }

        static VertexFiredEventHandler MakeOutputToOutputFileEventHandler()
        {
            bool hasHeader = false;
            string outputHeaderLine = string.Join("\t", "Clock", "Event");

            return (sender, e) =>
            {
                string outputDataLine = string.Join("\t", e.Clock, e.Vertex);

                if (null != e.TraceVariables)
                {
                    foreach (TraceVariable tv in e.TraceVariables)
                    {
                        if (!hasHeader)
                        {
                            outputHeaderLine += "\t" + tv.Name;
                        }
                        outputDataLine += "\t" + tv.Value.ToString();
                    }
                }

                if (!hasHeader)
                {
                    ProgramArgs.OutputWriter.WriteLine(outputHeaderLine);
                    hasHeader = true;
                }

                ProgramArgs.OutputWriter.WriteLine(outputDataLine);
            };
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

            return programArgs;
        }
    }
}
