// Generated with tegscli v0.9
//
// Name: Breakdown
// Description: A deterministic queue (M/D/1) with breakdowns
//
// Outputs:
// Clock
// Event
// QUEUE
// SERVER

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Breakdown
{
    enum EventType
    {
        Event_RUN = 0,
        Event_ENTER = 1,
        Event_START = 2,
        Event_LEAVE = 3,
        Event_FIX = 4,
        Event_FAIL = 5,
    }

    class Simulation : SimulationBase
    {
        protected override EventType StartingEventType => EventType.Event_RUN;

        // State Variables
        int StateVariable_QUEUE = default;
        int StateVariable_SERVER = default;

        public Simulation() { }

        protected override object ParseStartParameters(string[] startParameters) => Tuple.Create(int.Parse(startParameters[0]));

        protected override void ProcessEvent(EventType eventType, object parameterValues)
        {
            switch (eventType)
            {
                case EventType.Event_RUN:
                    Event_RUN((Tuple<int>)parameterValues);
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
                case EventType.Event_FIX:
                    Event_FIX();
                    break;
                case EventType.Event_FAIL:
                    Event_FAIL();
                    break;
            }
        }

        // Event #0: RUN
        // Description: The simulation has started
        private void Event_RUN(Tuple<int> parameterValues)
        {
            // Parameters
            StateVariable_QUEUE = parameterValues.Item1;

            // Event Code
            StateVariable_SERVER = 1;

            // Edge #0: Schedule RUN to ENTER
            // Description: Initiate the first job arrival
            ScheduleEvent(EventType.Event_ENTER, 0, 5, null);

            // Edge #1: Schedule RUN to FAIL
            // Description: Schedule the first machine breakdown
            ScheduleEvent(EventType.Event_FAIL, Random.ExponentialVariate(1.0 / 15.0), 4, null);
        }

        // Event #1: ENTER
        // Description: Arrival of a job
        private void Event_ENTER()
        {
            // Event Code
            StateVariable_QUEUE = StateVariable_QUEUE + 1;

            // Edge #2: Schedule ENTER to ENTER
            // Description: Schedule the next arrival
            ScheduleEvent(EventType.Event_ENTER, Random.ExponentialVariate(1.0 / 6.0), 6, null);

            // Edge #3: Schedule ENTER to START
            // Description: Start service
            if (StateVariable_SERVER > 0)
            {
                ScheduleEvent(EventType.Event_START, 0, 5, null);
            }
        }

        // Event #2: START
        // Description: Start of Service
        private void Event_START()
        {
            // Event Code
            StateVariable_SERVER = 0;
            StateVariable_QUEUE = StateVariable_QUEUE - 1;

            // Edge #4: Schedule START to LEAVE
            // Description: The job is placed in service for 2 minutes
            ScheduleEvent(EventType.Event_LEAVE, 2, 6, null);
        }

        // Event #3: LEAVE
        // Description: End of Service
        private void Event_LEAVE()
        {
            // Event Code
            StateVariable_SERVER = 1;

            // Edge #5: Schedule LEAVE to START
            // Description: Start servicing the waiting job
            if (StateVariable_QUEUE > 0)
            {
                ScheduleEvent(EventType.Event_START, 0, 5, null);
            }
        }

        // Event #4: FIX
        // Description: Completion of repair on the machine
        private void Event_FIX()
        {
            // Event Code
            StateVariable_SERVER = 1;

            // Edge #8: Schedule FIX to FAIL
            // Description: Schedule the next machine failure
            ScheduleEvent(EventType.Event_FAIL, Random.ExponentialVariate(1.0 / 15.0), 4, null);

            // Edge #9: Schedule FIX to START
            if (StateVariable_QUEUE > 0)
            {
                ScheduleEvent(EventType.Event_START, 0, 5, null);
            }
        }

        // Event #5: FAIL
        // Description: The occurrence of a service failure
        private void Event_FAIL()
        {
            // Event Code
            StateVariable_SERVER =  - 1;

            // Edge #6: Schedule FAIL to FIX
            // Description: After 30 minutes the machine will be fixed
            ScheduleEvent(EventType.Event_FIX, 30, 6, null);

            // Edge #7: CancelNext FAIL to LEAVE
            CancelNextEvent(EventType.Event_LEAVE, null);
        }

        protected override string GetEventName(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Event_RUN:
                    return "RUN";
                case EventType.Event_ENTER:
                    return "ENTER";
                case EventType.Event_START:
                    return "START";
                case EventType.Event_LEAVE:
                    return "LEAVE";
                case EventType.Event_FIX:
                    return "FIX";
                case EventType.Event_FAIL:
                    return "FAIL";
            }
            return "";
        }

        protected override void TraceExpressionHeaders(bool traceToConsole, StreamWriter outputWriter)
        {
            Trace(traceToConsole, outputWriter, "\tQUEUE");
            Trace(traceToConsole, outputWriter, "\tSERVER");
        }

        protected override void TraceExpressionValues(bool traceToConsole, StreamWriter outputWriter)
        {
            Trace(traceToConsole, outputWriter, $"\t{ StateVariable_QUEUE }");
            Trace(traceToConsole, outputWriter, $"\t{ StateVariable_SERVER }");
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
                    case "-o":
                    case "--output-file":
                        simArgs.OutputFile = args[++i];
                        break;
                    case "--seed":
                        simArgs.Seed = int.Parse(args[++i]);
                        break;
                    case "--silent":
                        simArgs.Silent = true;
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
        public string OutputFile;
        public int Seed;
        public bool Silent;
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
    
            bool traceToConsole = !args.Silent;
            using StreamWriter outputWriter = args.OutputFile is not null ? new StreamWriter(new FileStream(args.OutputFile, FileMode.Create), Encoding.UTF8) : null;
    
            StartTraceHeader(traceToConsole, outputWriter);
            TraceExpressionHeaders(traceToConsole, outputWriter);
            EndTraceLine(traceToConsole, outputWriter);
    
            while (_schedule.Count > 0 && _clock < args.StopCondition.MaxTime)
            {
                var entry = _schedule[0];
                _schedule.RemoveAt(0);
    
                _clock = entry.Time;
    
                ProcessEvent(entry.EventType, entry.ParameterValues);
    
                StartTrace(traceToConsole, outputWriter, entry.EventType);
                TraceExpressionValues(traceToConsole, outputWriter);
                EndTraceLine(traceToConsole, outputWriter);
            }
        }
    
        protected virtual object ParseStartParameters(string[] startParameters) => null;
    
        protected abstract void ProcessEvent(EventType eventType, object parameterValues);
    
        protected abstract string GetEventName(EventType eventType);
    
        protected void Trace(bool traceToConsole, StreamWriter outputWriter, string str)
        {
            if (traceToConsole)
            {
                Console.Write(str);
            }
            outputWriter?.Write(str);
        }
    
        private void StartTraceHeader(bool traceToConsole, StreamWriter outputWriter)
        {
            Trace(traceToConsole, outputWriter, "Clock\tEvent");
        }
    
        private void StartTrace(bool traceToConsole, StreamWriter outputWriter, EventType eventType)
        {
            Trace(traceToConsole, outputWriter, $"{ _clock }\t{ GetEventName(eventType) }");
        }
    
        protected abstract void TraceExpressionHeaders(bool traceToConsole, StreamWriter outputWriter);
    
        protected abstract void TraceExpressionValues(bool traceToConsole, StreamWriter outputWriter);
    
        private void EndTraceLine(bool traceToConsole, StreamWriter outputWriter)
        {
            if (traceToConsole)
            {
                Console.WriteLine();
            }
    
            outputWriter?.WriteLine();
        }
    
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
                (parameterValues is null || (match.ParameterValues is not null && match.ParameterValues.Equals(parameterValues)));
        }
    
        protected double Clock() => _clock;
    
        protected static int String_Length(string str) => str.Length;
    }
    
    public static class RandomExtensions
    {
        public static double UniformVariate(this Random random, double alpha, double beta)
        {
            return alpha + (beta - alpha) * random.NextDouble();
        }
    
        public static double ExponentialVariate(this Random random, double lambda)
        {
            if (lambda == 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(lambda));
            }
    
            return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }
    
        public static double NormalVariate(this Random random, double mu, double sigma)
        {
            double z;
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
    
        public static double TriangularVariate(this Random random, double low, double high, double mode)
        {
            double u = random.NextDouble();
            if (high == low)
            {
                return low;
            }
    
            double c = (mode - low) / (high - low);
            if (u > c)
            {
                u = 1.0 - u;
                c = 1.0 - c;
                double h = high;
                high = low;
                low = h;
            }
    
            return low + (high - low) * Math.Sqrt(u * c);
        }
    
        public static double GammaVariate(this Random random, double alpha, double beta)
        {
            if (alpha <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha));
            }
    
            if (beta <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(beta));
            }
    
            if (alpha > 1.0)
            {
                double ainv = Math.Sqrt(2.0 * alpha - 1.0);
                double bbb = alpha - Math.Log(4.0);
                double ccc = alpha + ainv;
    
                while (true)
                {
                    double u1 = random.NextDouble();
                    if (!(1E-7 < u1 && u1 < 0.9999999))
                    {
                        continue;
                    }
    
                    double u2 = 1.0 - random.NextDouble();
                    double v = Math.Log(u1 / (1.0 - u1)) / ainv;
                    double x = alpha * Math.Exp(v);
                    double z = u1 * u1 * u2;
                    double r = bbb + ccc * v - x;
    
                    if ((r + (1.0 + Math.Log(4.5)) - 4.5 * z >= 0.0) || r >= Math.Log(z))
                    {
                        return x * beta;
                    }
                }
            }
            else if (alpha == 1.0)
            {
                return -Math.Log(1.0 - random.NextDouble()) * beta;
            }
            else
            {
                double x;
                while (true)
                {
                    double u = random.NextDouble();
                    double b = (Math.E + alpha) / Math.E;
                    double p = b * u;
    
                    if (p <= 1.0)
                    {
                        x = Math.Pow(p, 1.0 / alpha);
                    }
                    else
                    {
                        x = -Math.Log((b - p) / alpha);
                    }
    
                    double u1 = random.NextDouble();
                    if (p > 1.0)
                    {
                        if (u1 <= Math.Pow(x, alpha - 1.0))
                        {
                            break;
                        }
                        else if (u1 <= Math.Exp(-x))
                        {
                            break;
                        }
                    }
                }
    
                return x * beta;
            }
        }
    
        public static double BetaVariate(this Random random, double alpha, double beta)
        {
            if (alpha <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha));
            }
    
            if (beta <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(beta));
            }
    
            double y = random.GammaVariate(alpha, 1.0);
            if (y != 0.0)
            {
                return y / (y + random.GammaVariate(beta, 1.0));
            }
    
            return 0.0;
        }
    }
}
