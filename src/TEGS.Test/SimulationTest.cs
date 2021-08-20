// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;

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
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void Simulation_RunTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_PauseTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(10000),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            s.Run();
            Assert.AreEqual(SimulationState.Running, s.State);

            s.Pause();
            Assert.AreEqual(SimulationState.Paused, s.State);

            s.Run();
            Assert.AreEqual(SimulationState.Running, s.State);

            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_StepTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

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
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

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
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_CompleteTraceTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(500)
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

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
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_RunWithCancelNextTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Breakdown)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(500),
            };

            args.StartParameterExpressions.Add("0");


            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }
    }
}
