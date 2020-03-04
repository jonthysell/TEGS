// Generated with tegs v0.9.0.0
//
// Name: Breakdown
// Description: A deterministic queue (M/D/1) with breakdowns

using System;
using System.Collections.Generic;
using System.Text;

namespace Breakdown
{
    class Simulation : SimulationBase
    {
        protected override int StartingEventId => 0;

        // State Variables
        int SV_QUEUE = default;
        int SV_SERVER = default;

        public Simulation() { }

        protected override object ParseStartParameters(string[] startParameters) => Tuple.Create(int.Parse(startParameters[0]));

        protected override void ProcessEvent(int eventId, object parameterValues)
        {
            switch (eventId)
            {
                case 0:
                    Event0((Tuple<int>)parameterValues);
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
                case 4:
                    Event4();
                    break;
                case 5:
                    Event5();
                    break;
            }
        }

        // Event #0
        // Name: RUN
        // Description: The simulation has started
        private void Event0(Tuple<int> parameterValues)
        {
            // Parameters
            SV_QUEUE = parameterValues.Item1;

            // Event Code
            SV_SERVER = 1;

            // Edge #0
            // Action: Schedule
            // Direction: RUN to ENTER
            // Description: Initiate the first job arrival
            ScheduleEvent(1, 0, 5, null);

            // Edge #1
            // Action: Schedule
            // Direction: RUN to FAIL
            // Description: Schedule the first machine breakdown
            ScheduleEvent(5, Random.ExponentialVariate(1d / 15d), 4, null);
        }

        // Event #1
        // Name: ENTER
        // Description: Arrival of a job
        private void Event1()
        {
            // Event Code
            SV_QUEUE = SV_QUEUE + 1;

            // Edge #2
            // Action: Schedule
            // Direction: ENTER to ENTER
            // Description: Schedule the next arrival
            ScheduleEvent(1, Random.ExponentialVariate(1d / 6d), 6, null);

            // Edge #3
            // Action: Schedule
            // Direction: ENTER to START
            // Description: Start service
            if (SV_SERVER > 0)
            {
                ScheduleEvent(2, 0, 5, null);
            }
        }

        // Event #2
        // Name: START
        // Description: Start of Service
        private void Event2()
        {
            // Event Code
            SV_SERVER = 0;
            SV_QUEUE = SV_QUEUE - 1;

            // Edge #4
            // Action: Schedule
            // Direction: START to LEAVE
            // Description: The job is placed in service for 2 minutes
            ScheduleEvent(3, 2, 6, null);
        }

        // Event #3
        // Name: LEAVE
        // Description: End of Service
        private void Event3()
        {
            // Event Code
            SV_SERVER = 1;

            // Edge #5
            // Action: Schedule
            // Direction: LEAVE to START
            // Description: Start servicing the waiting job
            if (SV_QUEUE > 0)
            {
                ScheduleEvent(2, 0, 5, null);
            }
        }

        // Event #4
        // Name: FIX
        // Description: Completion of repair on the machine
        private void Event4()
        {
            // Event Code
            SV_SERVER = 1;

            // Edge #8
            // Action: Schedule
            // Direction: FIX to FAIL
            // Description: Schedule the next machine failure
            ScheduleEvent(5, Random.ExponentialVariate(1d / 15d), 4, null);

            // Edge #9
            // Action: Schedule
            // Direction: FIX to START
            if (SV_QUEUE > 0)
            {
                ScheduleEvent(2, 0, 5, null);
            }
        }

        // Event #5
        // Name: FAIL
        // Description: The occurrence of a service failure
        private void Event5()
        {
            // Event Code
            SV_SERVER =  - 1;

            // Edge #6
            // Action: Schedule
            // Direction: FAIL to FIX
            // Description: After 30 minutes the machine will be fixed
            ScheduleEvent(4, 30, 6, null);

            // Edge #7
            // Action: CancelNext
            // Direction: FAIL to LEAVE
            CancelNextEvent(3, null);
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
