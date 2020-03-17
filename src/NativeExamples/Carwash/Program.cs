// Generated with tegs-cli v0.9.0.0
//
// Name: Carwash
// Description: An automatic carwash

using System;
using System.Collections.Generic;
using System.Text;

namespace Carwash
{
    enum EventType
    {
        Event_RUN = 0,
        Event_ENTER = 1,
        Event_START = 2,
        Event_LEAVE = 3,
    }

    class Simulation : SimulationBase
    {
        protected override EventType StartingEventType => EventType.Event_RUN;

        // State Variables
        int StateVariable_QUEUE = default;
        int StateVariable_SERVERS = default;

        public Simulation() { }

        protected override object ParseStartParameters(string[] startParameters) => Tuple.Create(int.Parse(startParameters[0]), int.Parse(startParameters[1]));

        protected override void ProcessEvent(EventType eventType, object parameterValues)
        {
            switch (eventType)
            {
                case EventType.Event_RUN:
                    Event_RUN((Tuple<int, int>)parameterValues);
                    break;
                case EventType.Event_ENTER:
                    Event_ENTER();
                    break;
                case EventType.Event_START:
                    Event_START();
                    break;
                case EventType.Event_LEAVE:
                    Event_LEAVE();
                    break;
            }
        }

        // Event #0: RUN
        // Description: The simulation run is started
        private void Event_RUN(Tuple<int, int> parameterValues)
        {
            // Parameters
            StateVariable_QUEUE = parameterValues.Item1;
            StateVariable_SERVERS = parameterValues.Item2;

            // Edge #0: Schedule RUN to ENTER
            // Description: The car will enter the line
            ScheduleEvent(EventType.Event_ENTER, 0, 5, null);
        }

        // Event #1: ENTER
        // Description: Cars enter the line
        private void Event_ENTER()
        {
            // Event Code
            StateVariable_QUEUE = StateVariable_QUEUE + 1;

            // Edge #1: Schedule ENTER to ENTER
            // Description: The next customer enters in 3 to 8 minutes
            ScheduleEvent(EventType.Event_ENTER, Random.UniformVariate(3, 8), 6, null);

            // Edge #2: Schedule ENTER to START
            // Description: There are available servers to start washing the car
            if (StateVariable_SERVERS > 0)
            {
                ScheduleEvent(EventType.Event_START, 0, 5, null);
            }
        }

        // Event #2: START
        // Description: Service starts
        private void Event_START()
        {
            // Event Code
            StateVariable_SERVERS = StateVariable_SERVERS - 1;
            StateVariable_QUEUE = StateVariable_QUEUE - 1;

            // Edge #3: Schedule START to LEAVE
            // Description: The car will be in service for at least 5 minutes
            ScheduleEvent(EventType.Event_LEAVE, Random.UniformVariate(5, 20), 6, null);
        }

        // Event #3: LEAVE
        // Description: Cars leave
        private void Event_LEAVE()
        {
            // Event Code
            StateVariable_SERVERS = StateVariable_SERVERS + 1;

            // Edge #4: Schedule LEAVE to START
            // Description: There are cars in queue, start service for the next car in line
            if (StateVariable_QUEUE > 0)
            {
                ScheduleEvent(EventType.Event_START, 0, 5, null);
            }
        }
    }

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
            var startValues = new List<string>();
    
            for (int i = 0; i < args.Length - 1; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--seed":
                        simArgs.Seed = int.Parse(args[++i]);
                        break;
                    case "--start-parameter":
                        startValues.Add(args[++i]);
                        break;
                    case "--stop-time":
                        simArgs.StopCondition.MaxTime = double.Parse(args[++i]);
                        break;
                    default:
                        throw new Exception($"Did not recognize option \"{args[i]}\"");
                }
            }
    
            if (startValues.Count > 0)
            {
                simArgs.StartParameterValues = startValues.ToArray();
            }
    
            return simArgs;
        }
    }
    
    struct ScheduleEntry : IComparable<ScheduleEntry>
    {
        public double Time;
        public double Priority;
        public EventType EventType;
        public object ParameterValues;
    
        public int CompareTo(ScheduleEntry other)
        {
            int timeCompare = Time.CompareTo(other.Time);
    
            if (timeCompare != 0)
            {
                return timeCompare;
            }
    
            return Priority.CompareTo(other.Priority);
        }
    }
    
    struct StopCondition
    {
        public double MaxTime;
    }
    
    struct SimulationArgs
    {
        public int Seed;
        public string[] StartParameterValues;
        public StopCondition StopCondition;
    }
    
    abstract class SimulationBase
    {
        private double _clock = 0.0;
    
        private readonly List<ScheduleEntry> _schedule = new List<ScheduleEntry>();
    
        protected Random Random;
    
        protected abstract EventType StartingEventType { get; }
    
        public void Run(SimulationArgs args)
        {
            Random = new Random(args.Seed);
    
            ScheduleEvent(StartingEventType, 0, 0, ParseStartParameters(args.StartParameterValues));
    
            while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
            {
                var entry = _schedule[0];
                _schedule.RemoveAt(0);
    
                _clock = entry.Time;
    
                ProcessEvent(entry.EventType, entry.ParameterValues);
            }
        }
    
        protected virtual object ParseStartParameters(string[] startParameters) => null;
    
        protected abstract void ProcessEvent(EventType eventType, object parameterValues);
    
        protected void ScheduleEvent(EventType eventType, double delay, double priority, object parameterValues)
        {
            var entry = new ScheduleEntry()
            {
                Time = _clock + delay,
                Priority = priority,
                EventType = eventType,
                ParameterValues = parameterValues
            };
    
            int index = _schedule.BinarySearch(entry);
    
            if (index < 0)
            {
                index = ~index;
            }
    
            if (index == _schedule.Count)
            {
                _schedule.Add(entry);
            }
            else
            {
                _schedule.Insert(index, entry);
            }
        }
    
        protected void CancelNextEvent(EventType eventType, object parameterValues)
        {
            for (int i = 0; i < _schedule.Count; i++)
            {
                if (CancelPredicate(_schedule[i], eventType, parameterValues))
                {
                    _schedule.RemoveAt(i);
                    break;
                }
            }
        }
    
        protected void CancelAllEvents(EventType eventType, object parameterValues)
        {
            _schedule.RemoveAll(entry => CancelPredicate(entry, eventType, parameterValues));
        }
    
        private static bool CancelPredicate(ScheduleEntry match, EventType eventType, object parameterValues)
        {
            return match.EventType == eventType &&
                (null == parameterValues || (null != match.ParameterValues && match.ParameterValues.Equals(parameterValues)));
        }
    
        protected double Clock() => _clock;
    
        protected int String_Length(string str) => str.Length;
    }
    
    public static class RandomExtensions
    {
        public static double UniformVariate(this Random random, double alpha, double beta)
        {
            return alpha + (beta - alpha) * random.NextDouble();
        }
    
        public static double ExponentialVariate(this Random random, double lambda)
        {
            return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }
    
        public static double NormalVariate(this Random random, double mu, double sigma)
        {
            double z = 0.0;
    
            while (true)
            {
                double u1 = random.NextDouble();
                double u2 = 1.0 - random.NextDouble();
    
                z = (4 * Math.Exp(-0.5) / Math.Sqrt(2.0)) * (u1 - 0.5) / u2;
    
                if ((z * z / 4.0) <= -Math.Log(u2))
                {
                    break;
                }
            }
    
            return mu + z * sigma;
        }
    
        public static double LogNormalVariate(this Random random, double mu, double sigma)
        {
            return Math.Exp(random.NormalVariate(mu, sigma));
        }
    }
}
