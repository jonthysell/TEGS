// 
// Simulation.cs
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
using System.Threading.Tasks;

namespace TEGS
{
    public delegate void SimulationStateChangedEventHandler(object sender, SimulationStateEventArgs e);

    public delegate void ClockChangedEventHandler(object sender, ClockChangedEventArgs e);

    public delegate void VertexFiringEventHandler(object sender, VertexEventArgs e);

    public delegate void VertexFiredEventHandler(object sender, VertexEventArgs e);

    public delegate void EdgeFiringEventHandler(object sender, EdgeEventArgs e);

    public delegate void EdgeFiredEventHandler(object sender, EdgeEventArgs e);

    public class Simulation
    {
        public Graph Graph => Args.Graph;

        public IList<TraceExpression> TraceExpressions => Args.TraceExpressions;

        public ScriptingHost ScriptingHost => Args.ScriptingHost;

        public SimulationArgs Args { get; private set; }

        public SimulationState State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value != _state)
                {
                    _state = value;
                    OnSimulationStateChanged();
                }
            }
        }
        private SimulationState _state = SimulationState.None;

        public Schedule Schedule { get; private set; }

        public double Clock { get; private set; }

        public event SimulationStateChangedEventHandler SimulationStateChanged;

        public event ClockChangedEventHandler ClockChanged;

        public event VertexFiringEventHandler VertexFiring;

        public event VertexFiredEventHandler VertexFired;

        public event EdgeFiringEventHandler EdgeFiring;

        public event EdgeFiredEventHandler EdgeFired;

        public Simulation(SimulationArgs args)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));

            Schedule = new Schedule(Graph, ScriptingHost.CompareParameters);

            InitDelegates();
        }

        private void InitDelegates()
        {
            ScriptingHost.AssignDelegate("t_clock", new Func<double>(() => Clock));
        }

        public void Run()
        {
            Task task = RunAsync();
            task.Wait();
        }

        public async Task RunAsync()
        {
            if (State == SimulationState.Complete)
            {
                throw new InvalidOperationException();
            }

            if (State == SimulationState.None)
            {
                Start();
            }

            State = SimulationState.Running;

            while (State == SimulationState.Running)
            {
                await Task.Run(() => InternalStep());
            }
        }

        public void Pause()
        {
            Task task = PauseAsync();
            task.Wait();
        }

        public async Task PauseAsync()
        {
            if (State == SimulationState.None || State == SimulationState.Complete)
            {
                throw new InvalidOperationException();
            }

            await Task.Run(() =>
            {
                State = SimulationState.Paused;
            });
        }

        public void Step()
        {
            Task task = StepAsync();
            task.Wait();
        }

        public async Task StepAsync()
        {
            if (State == SimulationState.Running || State == SimulationState.Complete)
            {
                throw new InvalidOperationException();
            }

            if (State == SimulationState.None)
            {
                Start();
                State = SimulationState.Paused;
            }
            else
            {
                await Task.Run(() => InternalStep());
            }
        }

        private void Start()
        {
            // Initialize state variables
            for (int i = 0; i < Graph.StateVariables.Count; i++)
            {
                ScriptingHost.CreateStateVariable(Graph.StateVariables[i]);
            }

            // Set seed
            if (Args.StartingSeed.HasValue)
            {
                ScriptingHost.SetSeed(Args.StartingSeed.Value);
            }
            else
            {
                ScriptingHost.SetSeed();
            }

            // Initialize clock
            Clock = Schedule.MinTime;

            // Insert starting event
            Schedule.Insert(Graph.StartingVertex, Schedule.DefaultDelay, Schedule.DefaultPriority, ScriptingHost.EvaluateParameters(Args.StartParameters));
        }

        private void InternalStep()
        {
            // Get next event
            ScheduledEvent nextEvent = Schedule.GetNext();

            OnVertexFiring(nextEvent.Target);

            // Update internal variables
            Clock = nextEvent.Time;

            // Assign parameters
            ScriptingHost.AssignParameters(nextEvent.Target.Parameters, nextEvent.ParameterValues);

            // Execute event
            ScriptingHost.Execute(nextEvent.Target.Code);

            // Evaluate trace variables
            EvaluateTraces();

            OnVertexFired(nextEvent.Target);

            // Evaluate edges
            foreach (Edge edge in nextEvent.Target.Edges)
            {
                // Check condition
                if (ScriptingHost.EvaluateBoolean(edge.Condition, true))
                {
                    OnEdgeFiring(edge);

                    // Evaluate parameters
                    string parameterValues = ScriptingHost.EvaluateParameters(edge.Parameters);

                    switch (edge.Action)
                    {
                        case EdgeAction.Schedule:
                            double time = Clock + ScriptingHost.EvaluateDouble(edge.Delay, Schedule.DefaultDelay);
                            double priority = ScriptingHost.EvaluateDouble(edge.Priority, Schedule.DefaultPriority);
                            Schedule.Insert(edge.Target, time, priority, parameterValues);
                            break;
                        case EdgeAction.CancelNext:
                            Schedule.CancelNext(edge.Target, parameterValues);
                            break;
                        case EdgeAction.CancelAll:
                            Schedule.CancelAll(edge.Target, parameterValues);
                            break;
                    }

                    OnEdgeFired(edge);
                }
            }

            if (!KeepGoing())
            {
                State = SimulationState.Complete;
            }
        }

        private bool KeepGoing()
        {
            return Schedule.EventCount > 0 && (null == Args.StopCondition || !Args.StopCondition.ShouldStop(this));
        }

        private void EvaluateTraces()
        {
            for (int i = 0; i < TraceExpressions.Count; i++)
            {
                TraceExpressions[i].Evaluate(ScriptingHost);
            }
        }

        private void OnSimulationStateChanged()
        {
            SimulationStateChanged?.Invoke(this, new SimulationStateEventArgs(State));
        }

        private void OnClockChanged()
        {
            ClockChanged?.Invoke(this, new ClockChangedEventArgs(Clock));
        }

        private void OnVertexFiring(Vertex vertex)
        {
            VertexFiring?.Invoke(this, new VertexEventArgs(Clock, vertex, TraceExpressions));
        }

        private void OnVertexFired(Vertex vertex)
        {
            VertexFired?.Invoke(this, new VertexEventArgs(Clock, vertex, TraceExpressions));
        }

        private void OnEdgeFiring(Edge edge)
        {
            EdgeFiring?.Invoke(this, new EdgeEventArgs(Clock, edge));
        }

        private void OnEdgeFired(Edge edge)
        {
            EdgeFired?.Invoke(this, new EdgeEventArgs(Clock ,edge));
        }
    }

    public enum SimulationState
    {
        None,
        Running,
        Paused,
        Complete
    }

    public class SimulationStateEventArgs : EventArgs
    {
        public SimulationState State { get; private set; }

        public SimulationStateEventArgs(SimulationState state) : base()
        {
            State = state;
        }
    }

    public abstract class SimulationEventArgsBase : EventArgs
    {
        public double Clock { get; private set; }

        public SimulationEventArgsBase(double clock) : base()
        {
            Clock = clock;
        }
    }

    public class ClockChangedEventArgs : SimulationEventArgsBase
    {
        public ClockChangedEventArgs(double clock) : base(clock) { }
    }

    public class VertexEventArgs : SimulationEventArgsBase
    {
        public Vertex Vertex { get; private set; }

        public IList<TraceExpression> TraceExpressions { get; private set; }

        public VertexEventArgs(double clock, Vertex vertex, IList<TraceExpression> traceExpressions) : base(clock)
        {
            Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
            TraceExpressions = traceExpressions ?? throw new ArgumentNullException(nameof(traceExpressions));
        }
    }

    public class EdgeEventArgs : SimulationEventArgsBase
    {
        public Edge Edge { get; private set; }

        public EdgeEventArgs(double clock, Edge edge) : base(clock)
        {
            Edge = edge ?? throw new ArgumentNullException(nameof(edge));
        }
    }
}
