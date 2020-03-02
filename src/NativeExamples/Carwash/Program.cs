// Generated with tegs v0.9.0.0
//
// Name: Carwash
// Description: An automatic carwash

using System;
using System.Collections.Generic;
using System.Text;

namespace Carwash
{
    class Simulation : SimulationBase
    {
        protected override int StartingEventId => 0;

        // State Variables
        int QUEUE = default;
        int SERVERS = default;

        public Simulation() { }

        protected override object ParseStartParameters(string[] startParameters) => Tuple.Create(int.Parse(startParameters[0]), int.Parse(startParameters[1]));

        protected override void ProcessEvent(int eventId, object parameterValues)
        {
            switch (eventId)
            {
                case 0:
                    Event0((Tuple<int, int>)parameterValues);
                    break;
                case 1:
                    Event1();
                    break;
                case 2:
                    Event2();
                    break;
                case 3:
                    Event3();
                    break;
            }
        }

        // Event #0
        // Name: RUN
        // Description: The simulation run is started
        private void Event0(Tuple<int, int> parameterValues)
        {
            // Parameters
            QUEUE = parameterValues.Item1;
            SERVERS = parameterValues.Item2;

            // Edge #0
            // Action: Schedule
            // Direction: RUN to ENTER
            // Description: The car will enter the line
            ScheduleEvent(1, 0, 5, null);
        }

        // Event #1
        // Name: ENTER
        // Description: Cars enter the line
        private void Event1()
        {
            // Event Code
            QUEUE = QUEUE + 1;

            // Edge #1
            // Action: Schedule
            // Direction: ENTER to ENTER
            // Description: The next customer enters in 3 to 8 minutes
            ScheduleEvent(1, Random.UniformVariate(3, 8), 6, null);

            // Edge #2
            // Action: Schedule
            // Direction: ENTER to START
            // Description: There are available servers to start washing the car
            if (SERVERS > 0)
            {
                ScheduleEvent(2, 0, 5, null);
            }
        }

        // Event #2
        // Name: START
        // Description: Service starts
        private void Event2()
        {
            // Event Code
            SERVERS = SERVERS - 1;
            QUEUE = QUEUE - 1;

            // Edge #3
            // Action: Schedule
            // Direction: START to LEAVE
            // Description: The car will be in service for at least 5 minutes
            ScheduleEvent(3, Random.UniformVariate(5, 20), 6, null);
        }

        // Event #3
        // Name: LEAVE
        // Description: Cars leave
        private void Event3()
        {
            // Event Code
            SERVERS = SERVERS + 1;

            // Edge #4
            // Action: Schedule
            // Direction: LEAVE to START
            // Description: There are cars in queue, start service for the next car in line
            if (QUEUE > 0)
            {
                ScheduleEvent(2, 0, 5, null);
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
        public int EventId;
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
    
        private List<ScheduleEntry> _schedule = new List<ScheduleEntry>();
    
        protected Random Random;
    
        protected abstract int StartingEventId { get; }
    
        public void Run(SimulationArgs args)
        {
            Random = new Random(args.Seed);
    
            ScheduleEvent(StartingEventId, 0, 0, ParseStartParameters(args.StartParameterValues));
    
            while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
            {
                var entry = _schedule[0];
                _schedule.RemoveAt(0);
    
                _clock = entry.Time;
    
                ProcessEvent(entry.EventId, entry.ParameterValues);
            }
        }
    
        protected virtual object ParseStartParameters(string[] startParameters) => null;
    
        protected abstract void ProcessEvent(int eventId, object parameterValues);
    
        protected void ScheduleEvent(int eventId, double delay, double priority, object parameterValues)
        {
            var entry = new ScheduleEntry()
            {
                Time = _clock + delay,
                Priority = priority,
                EventId = eventId,
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
    
        protected void CancelNextEvent(int eventId, object parameterValues)
        {
            for (int i = 0; i < _schedule.Count; i++)
            {
                if (CancelPredicate(_schedule[i], eventId, parameterValues))
                {
                    _schedule.RemoveAt(i);
                    break;
                }
            }
        }
    
        protected void CancelAllEvents(int eventId, object parameterValues)
        {
            _schedule.RemoveAll(entry => CancelPredicate(entry, eventId, parameterValues));
        }
    
        private static bool CancelPredicate(ScheduleEntry match, int eventId, object parameterValues)
        {
            return match.EventId == eventId &&
                (null == parameterValues || (null != match.ParameterValues && match.ParameterValues.Equals(parameterValues)));
        }
    
        protected double Clock() => _clock;
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
    
    public static class StringExtensions
    {
        public static int Length(this string str) => str.Length;
    }
}
