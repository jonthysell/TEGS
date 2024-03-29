﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
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

        public ScriptingHost ScriptingHost { get; private set; }

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
        private volatile SimulationState _state = SimulationState.None;

        public Schedule Schedule { get; private set; }

        public double Clock { get; private set; }

        public int[] EventCount { get; private set; }

        public Vertex CurrentVertex { get; private set; }

        public Edge CurrentEdge { get; private set; }

        public event SimulationStateChangedEventHandler SimulationStateChanged;

        public event VertexFiringEventHandler VertexFiring;

        public event VertexFiredEventHandler VertexFired;

        public event EdgeFiringEventHandler EdgeFiring;

        public event EdgeFiredEventHandler EdgeFired;

        private Task _currentTask = null;
        private CancellationTokenSource _currentCTS = null;

        public Simulation(SimulationArgs args)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));

            Schedule = new Schedule(Graph);

            // Initialize clock
            Clock = Schedule.MinTime;

            // Initialize event count
            EventCount = new int[Graph.Vertices.Count];

            CurrentVertex = null;
            CurrentEdge = null;
        }

        #region Simulation Operations

        public void Run()
        {
            if (State == SimulationState.None)
            {
                InternalStart();
                State = SimulationState.Paused;
            }

            if (State == SimulationState.Paused)
            {
                _currentCTS = new CancellationTokenSource();

                var cancelationToken = _currentCTS.Token;

                State = SimulationState.Running;

                _currentTask = Task.Run(() =>
                {
                    while (State == SimulationState.Running)
                    {
                        if (cancelationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        InternalStep();
                    }
                });
            }
        }

        public void Wait()
        {
            if (State == SimulationState.Running)
            {
                _currentTask?.Wait();

                _currentCTS = null;
                _currentTask = null;
            }
        }

        public void Pause()
        {
            if (State == SimulationState.Running)
            {
                _currentCTS?.Cancel();
                _currentTask?.Wait();

                State = SimulationState.Paused;

                _currentCTS = null;
                _currentTask = null;
            }
        }

        public void Step()
        {
            if (State == SimulationState.None)
            {
                InternalStart();
                State = SimulationState.Paused;
            }
            else if (State == SimulationState.Paused)
            {
                InternalStep();
            }
        }

        private void InternalStart()
        {
            // Get a base scripting host with built in libraries
            ScriptingHost = BaseLibraries.MakeBaseScriptingHost(Args.StartingSeed);

            // Expose Simulation functions
            ScriptingHost.DefineCustomFunction(nameof(Clock), GetClock);
            ScriptingHost.DefineCustomFunction(nameof(EventCount), GetCurrentEventCount);

            // Initialize state variables
            foreach (StateVariable stateVariable in Graph.StateVariables)
            {
                ScriptingHost.Create(stateVariable);
            }

            // Insert starting event
            Schedule.Insert(Graph.StartingVertex, Schedule.DefaultDelay, Schedule.DefaultPriority, EvaluateParameters(Args.StartParameterExpressions));
        }

        private void InternalStep()
        {
            // Get next event
            ScheduledEvent nextEvent = Schedule.GetNext();

            // Update clock
            Clock = nextEvent.Time;

            OnVertexFiring(nextEvent.Target);

            // Assign parameters
            AssignParameters(nextEvent.Target, nextEvent.ParameterValues);

            // Execute event code
            ScriptingHost.Execute(nextEvent.Target.Code);

            // Update event count
            EventCount[Graph.Vertices.IndexOf(nextEvent.Target)]++;

            // Evaluate trace variables
            EvaluateTraces();

            OnVertexFired(nextEvent.Target);

            // Evaluate edges
            for (int i = 0; i < Graph.Edges.Count; i++)
            {
                if (Graph.Edges[i].Source == nextEvent.Target)
                {
                    Edge edge = Graph.Edges[i];

                    // Check condition
                    if (ScriptingHost.Evaluate(edge.Condition, VariableValue.True).AsBoolean())
                    {
                        OnEdgeFiring(edge);

                        // Evaluate parameters
                        IReadOnlyList<VariableValue> parameterValues = EvaluateParameters(edge.ParameterExpressions);

                        switch (edge.Action)
                        {
                            case EdgeAction.Schedule:
                                double time = Clock + ScriptingHost.Evaluate(edge.Delay, new VariableValue(Schedule.DefaultDelay)).AsNumber();
                                double priority = ScriptingHost.Evaluate(edge.Priority, new VariableValue(Schedule.DefaultPriority)).AsNumber();
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
            }

            if (!KeepGoing())
            {
                State = SimulationState.Complete;
            }
        }

        private bool KeepGoing()
        {
            return Schedule.EventCount > 0 && (Args.StopCondition is null || !Args.StopCondition.ShouldStop(this));
        }

        private void EvaluateTraces()
        {
            for (int i = 0; i < TraceExpressions.Count; i++)
            {
                TraceExpressions[i].Evaluate(ScriptingHost);
            }
        }

        private void AssignParameters(Vertex target, IReadOnlyList<VariableValue> parameterValues)
        {
            if (parameterValues is not null && parameterValues.Count > 0)
            {
                var parameterNames = target.ParameterNames;

                for (int i = 0; i < parameterValues.Count; i++)
                {
                    ScriptingHost.SetValue(parameterNames[i], parameterValues[i]);
                }
            }
        }

        private IReadOnlyList<VariableValue> EvaluateParameters(IReadOnlyList<string> parameterExpressions)
        {
            if (parameterExpressions is not null && parameterExpressions.Count > 0)
            {
                List<VariableValue> values = new List<VariableValue>(parameterExpressions.Count);

                for (int i = 0; i < parameterExpressions.Count; i++)
                {
                    values.Add(ScriptingHost.Evaluate(parameterExpressions[i]));
                }

                return values;
            }

            return null;
        }

        #endregion

        #region Events

        private void OnSimulationStateChanged()
        {
            SimulationStateChanged?.Invoke(this, new SimulationStateEventArgs(State));
        }

        private void OnVertexFiring(Vertex vertex)
        {
            CurrentVertex = vertex;
            VertexFiring?.Invoke(this, new VertexEventArgs(Clock, vertex, TraceExpressions));
        }

        private void OnVertexFired(Vertex vertex)
        {
            VertexFired?.Invoke(this, new VertexEventArgs(Clock, vertex, TraceExpressions));
        }

        private void OnEdgeFiring(Edge edge)
        {
            CurrentEdge = edge;
            EdgeFiring?.Invoke(this, new EdgeEventArgs(Clock, edge));
        }

        private void OnEdgeFired(Edge edge)
        {
            EdgeFired?.Invoke(this, new EdgeEventArgs(Clock, edge));
        }

        #endregion

        #region Custom Functions

        private VariableValue GetClock(VariableValue[] args)
        {
            if (args is null || args.Length == 0)
            {
                return new VariableValue(Clock);
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        private VariableValue GetCurrentEventCount(VariableValue[] args)
        {
            if (args is null || args.Length == 0)
            {
                return new VariableValue(EventCount[Graph.Vertices.IndexOf(CurrentVertex)]);
            }

            throw new ArgumentOutOfRangeException(nameof(args));
        }

        #endregion
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
