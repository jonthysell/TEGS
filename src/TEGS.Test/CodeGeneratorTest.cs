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
        public void CodeGenerator_CarwashFileGenerateSourceValidTest()
        {
            CodeGenerator_GenerateSourceValidTest(TestGraph.Load("carwash.json"));
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

        static CodeGeneratorTest()
        {
            var assemblyLocations = new HashSet<string>()
            {
                typeof(object).Assembly.Location,
                typeof(Console).Assembly.Location,
                typeof(Math).Assembly.Location,
                typeof(Convert).Assembly.Location,
            };

            typeof(Console).Assembly.GetReferencedAssemblies()
            .ToList()
            .ForEach(a => assemblyLocations.Add(Assembly.Load(a).Location));

            foreach (var assemblyLocation in assemblyLocations)
            {
                _metadataReferences.Add(MetadataReference.CreateFromFile(assemblyLocation));
            }
        }

        private static readonly CSharpParseOptions _parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        private static readonly CSharpCompilationOptions _compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication, optimizationLevel: OptimizationLevel.Release);
        private static readonly List<MetadataReference> _metadataReferences = new List<MetadataReference>();

        private static byte[] CompileCode(string code, string assemblyName)
        {
            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(code, _parseOptions);

            var compilation = CSharpCompilation.Create(assemblyName,
                new[] { parsedSyntaxTree },
                references: _metadataReferences,
                options: _compilationOptions);

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
