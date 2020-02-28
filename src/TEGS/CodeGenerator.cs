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
            sb.AppendLine();

            // Usings
            AddUsings(sb);
            sb.AppendLine();

            // Namespace
            StartBlock(sb, "namespace TEGS", ref indent);

            // Program
            AddProgramClass(sb, ref indent);

            EndBlock(sb, ref indent); // namespace

            return sb.ToString();
        }

        #region Header

        private static void AddHeader(StringBuilder sb, Graph graph)
        {
            sb.AppendLine($"// Generated with { AppInfo.Name } v{ AppInfo.Version }");
            sb.AppendLine($"//");
            sb.AppendLine($"// Name: { graph.Name }");
            sb.AppendLine($"// Description: { graph.Description }");
            sb.AppendLine($"//");
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

        #region Program Class

        private static void AddProgramClass(StringBuilder sb, ref int indent)
        {
            WriteCode(sb, ProgramClass, ref indent);
        }

        private const string ProgramClass = @"
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
}";
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

        private static void StartBlock(StringBuilder sb, string code, ref int indent)
        {
            string padding = GetIndentPadding(indent);

            sb.Append(padding);
            sb.AppendLine(code);
            sb.Append(padding);
            sb.AppendLine("{");

            indent++;
        }

        private static void EndBlock(StringBuilder sb, ref int indent)
        {
            indent--;

            string padding = GetIndentPadding(indent);

            sb.Append(padding);
            sb.AppendLine("}");
        }

        #endregion
    }
}
