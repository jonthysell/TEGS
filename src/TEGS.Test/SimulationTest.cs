// 
// SimulationTest.cs
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class SimulationTest
    {
        [TestMethod]
        public void Simulation_NewTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            Simulation s = new Simulation(args);
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void Simulation_RunTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            s.Run();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_PauseTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(10000),
            };

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            Task t1 = s.RunAsync();
            Assert.AreEqual(SimulationState.Running, s.State);

            s.Pause();
            Assert.AreEqual(SimulationState.Paused, s.State);
            t1.Wait();

            Task t2 = s.RunAsync();
            Thread.Sleep(0);
            Assert.AreEqual(SimulationState.Running, s.State);

            t2.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_StepTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            while(s.State != SimulationState.Complete)
            {
                s.Step();
            }

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_BasicTraceTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            bool hasHeader = false;
            s.VertexFired += (sender, e) =>
            {
                if (!hasHeader)
                {
                    Trace.WriteLine(string.Join("\t", "Clock", "Event"));
                    hasHeader = true;
                }

                Trace.WriteLine(string.Join("\t", e.Clock, e.Vertex));
            };

            s.Run();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_CompleteTraceTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "5", "3" },
                StopCondition = StopCondition.StopAfterMaxTime(500)
            };

            args.TraceExpressions.Add(new StateVariableTraceExpression(args.Graph.GetStateVariable("QUEUE")));
            args.TraceExpressions.Add(new StateVariableTraceExpression(args.Graph.GetStateVariable("SERVERS")));

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            bool hasHeader = false;
            string headerLine = string.Join("\t", "Clock", "Event");

            s.VertexFired += (sender, e) =>
            {
                string line = string.Join("\t", e.Clock, e.Vertex);

                for (int i = 0; i < e.TraceExpressions.Count; i++)
                {
                    if (!hasHeader)
                    {
                        headerLine += "\t" + e.TraceExpressions[i].Label;
                    }
                    line += "\t" + e.TraceExpressions[i].Value.ToString();
                }

                if (!hasHeader)
                {
                    Trace.WriteLine(headerLine);
                    hasHeader = true;
                }

                Trace.WriteLine(line);
            };

            s.Run();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_RunWithCancelNextTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Breakdown)
            {
                StartingSeed = 12345,
                StartParameterExpressions = new List<string>() { "0" },
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            s.Run();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }
    }
}
