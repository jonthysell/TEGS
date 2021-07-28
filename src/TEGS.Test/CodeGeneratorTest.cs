// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    [DeploymentItem("TestGraphs")]
    public class CodeGeneratorTest
    {
        [TestMethod]
        public void CodeGenerator_CarwashGenerateSourceValidTest()
        {
            CodeGenerator_GenerateSourceValidTest(TestGraph.Carwash);
        }

        [TestMethod]
        public void CodeGenerator_BreakdownGenerateSourceValidTest()
        {
            CodeGenerator_GenerateSourceValidTest(TestGraph.Breakdown);
        }

        [TestMethod]
        public void CodeGenerator_CarwashXMLGenerateSourceValidTest()
        {
            CodeGenerator_GenerateSourceValidTest(TestGraph.LoadXml("carwash.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CodeGenerator_GenerateSourceNullInvalidTest()
        {
            CodeGenerator.GenerateSource(null, "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CodeGenerator_GenerateProjectNullInvalidTest()
        {
            CodeGenerator.GenerateProject(null);
        }

        private static void CodeGenerator_GenerateSourceValidTest(Graph graph)
        {
            string code = CodeGenerator.GenerateSource(graph, graph.Name);
            Assert.IsNotNull(code);

            byte[] compiledCode = CompileCode(code, $"{ graph.Name }.exe");
            Assert.IsNotNull(compiledCode);
        }

        private static byte[] CompileCode(string code, string assemblyName)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(code, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            };

            Assembly.GetEntryAssembly().GetReferencedAssemblies()
            .ToList()
            .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            var compilation = CSharpCompilation.Create(assemblyName,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release));

            using var memoryStream = new MemoryStream();

            var result = compilation.Emit(memoryStream);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                Trace.WriteLine("Compilation Failures:");

                Trace.Indent();

                foreach (var diagnostic in failures)
                {
                    Trace.WriteLine($"{ diagnostic.Id }: { diagnostic.GetMessage() }");
                }

                Trace.Unindent();

                return null;
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream.ToArray();
        }
    }
}
