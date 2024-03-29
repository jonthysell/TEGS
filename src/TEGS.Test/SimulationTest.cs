﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
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

        [TestMethod]
        public void Simulation_StopAfterMaxTimeTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxTime(double.Epsilon),
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            Assert.AreEqual(0.0, s.Clock);

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);

            Assert.IsTrue(s.Clock >= double.Epsilon);
        }

        [TestMethod]
        public void Simulation_StopAfterMaxEventCountTest()
        {
            SimulationArgs args = new SimulationArgs(TestGraph.Carwash)
            {
                StartingSeed = 12345,
                StopCondition = StopCondition.StopAfterMaxEventCount(TestGraph.Carwash.StartingVertex.Name, 1)
            };

            args.StartParameterExpressions.Add("5");
            args.StartParameterExpressions.Add("3");

            Simulation s = new Simulation(args);
            Assert.AreEqual(SimulationState.None, s.State);

            Assert.AreEqual(0, s.EventCount[args.Graph.Vertices.IndexOf(args.Graph.StartingVertex)]);

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);

            Assert.AreEqual(1, s.EventCount[args.Graph.Vertices.IndexOf(args.Graph.StartingVertex)]);
        }

        [TestMethod]
        public void Simulation_ClockTest()
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

            double expectedClock = 0.0;

            s.VertexFiring += (_, e) =>
            {
                expectedClock = e.Clock;
                Assert.AreEqual(expectedClock, s.Clock);
            };

            s.VertexFired += (_, _) =>
            {
                Assert.AreEqual(expectedClock, s.Clock);
            };

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }

        [TestMethod]
        public void Simulation_EventCountTest()
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

            int[] expectedEventCounts = new int[args.Graph.Vertices.Count];

            s.VertexFiring += (_, e) =>
            {
                int eventIndex = args.Graph.Vertices.IndexOf(e.Vertex);
                Assert.AreEqual(expectedEventCounts[eventIndex], s.EventCount[eventIndex]);
            };

            s.VertexFired += (_, e) =>
            {
                int eventIndex = args.Graph.Vertices.IndexOf(e.Vertex);
                Assert.AreEqual(++expectedEventCounts[eventIndex], s.EventCount[eventIndex]);
            };

            s.Run();
            s.Wait();

            Assert.AreEqual(SimulationState.Complete, s.State);
        }
    }
}
