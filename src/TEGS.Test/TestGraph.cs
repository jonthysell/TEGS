// 
// TestGraph.cs
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

namespace TEGS.Test
{
    public class TestGraph
    {
        public static Graph Carwash
        {
            get
            {
                if (null == _carwash)
                {
                    Graph g = new Graph
                    {
                        Name = "Carwash",
                        Description = "An automatic carwash"
                    };

                    g.StateVariables.Add(new StateVariable("QUEUE", VariableValueType.Integer));
                    g.StateVariables.Add(new StateVariable("SERVERS", VariableValueType.Integer));

                    Vertex run = g.AddVertex("RUN");
                    run.Description = "The simulation run is started";
                    run.Parameters = "QUEUE, SERVERS";
                    g.StartingVertex = run;

                    Vertex enter = g.AddVertex("ENTER");
                    enter.Description = "Cars enter the line";
                    enter.Code = "QUEUE = QUEUE + 1";

                    Vertex start = g.AddVertex("START");
                    start.Description = "Service starts";
                    start.Code = "SERVERS = SERVERS - 1\r\nQUEUE = QUEUE - 1";

                    Vertex leave = g.AddVertex("LEAVE");
                    leave.Description = "Cars leave";
                    leave.Code = "SERVERS = SERVERS + 1";

                    Edge run_enter = g.AddEdge(run, enter);
                    run_enter.Description = "The car will enter the line";
                    run_enter.Priority = "5";

                    Edge enter_enter = g.AddEdge(enter, enter);
                    enter_enter.Description = "The next customer enters in 3 to 8 minutes";
                    enter_enter.Delay = "t_uniformvariate(3, 8)";
                    enter_enter.Priority = "6";

                    Edge enter_start = g.AddEdge(enter, start);
                    enter_start.Description = "There are available servers to start washing the car";
                    enter_start.Condition = "SERVERS > 0";
                    enter_start.Priority = "5";

                    Edge start_leave = g.AddEdge(start, leave);
                    start_leave.Description = "The car will be in service for at least 5 minutes";
                    start_leave.Delay = "t_uniformvariate(5, 20)";
                    start_leave.Priority = "6";

                    Edge leave_start = g.AddEdge(leave, start);
                    leave_start.Description = "There are cars in queue, start service for the next car in line";
                    leave_start.Condition = "QUEUE > 0";
                    leave_start.Priority = "5";

                    _carwash = g;
                }

                return _carwash;
            }
        }
        private static Graph _carwash;

        public static Graph Breakdown
        {
            get
            {
                if (null == _breakdown)
                {
                    Graph g = new Graph
                    {
                        Name = "Breakdown",
                        Description = "A deterministic queue (M/D/1) with breakdowns"
                    };

                    g.StateVariables.Add(new StateVariable("QUEUE", VariableValueType.Integer));
                    g.StateVariables.Add(new StateVariable("SERVER", VariableValueType.Integer));

                    Vertex run = g.AddVertex("RUN");
                    run.Description = "The simulation has started";
                    run.Parameters = "QUEUE";
                    run.Code = "SERVER = 1";
                    g.StartingVertex = run;

                    Vertex enter = g.AddVertex("ENTER");
                    enter.Description = "Arrival of a job";
                    enter.Code = "QUEUE = QUEUE + 1";

                    Vertex start = g.AddVertex("START");
                    start.Description = "Start of Service";
                    start.Code = "SERVER = 0\r\nQUEUE = QUEUE - 1";

                    Vertex leave = g.AddVertex("LEAVE");
                    leave.Description = "End of Service";
                    leave.Code = "SERVER = 1";

                    Vertex fix = g.AddVertex("FIX");
                    fix.Description = "Completion of repair on the machine";
                    fix.Code = "SERVER = 1";

                    Vertex fail = g.AddVertex("FAIL");
                    fail.Description = "The occurrence of a sercice failure";
                    fail.Code = "SERVER = -1";

                    Edge run_enter = g.AddEdge(run, enter);
                    run_enter.Description = "Initiate the first job arrival";
                    run_enter.Priority = "5";

                    Edge run_fail = g.AddEdge(run, fail);
                    run_fail.Description = "Schedule the first machine breakdown";
                    run_fail.Delay = "t_expovariate(1/15)";
                    run_fail.Priority = "4";

                    Edge enter_enter = g.AddEdge(enter, enter);
                    enter_enter.Description = "Schedule the next arrival";
                    enter_enter.Delay = "t_expovariate(1/6)";
                    enter_enter.Priority = "6";

                    Edge enter_start = g.AddEdge(enter, start);
                    enter_start.Description = "Start service";
                    enter_start.Condition = "SERVERS > 0";
                    enter_start.Priority = "5";

                    Edge start_leave = g.AddEdge(start, leave);
                    start_leave.Description = "The job is placed in service for 2 minutes";
                    start_leave.Delay = "2";
                    start_leave.Priority = "6";

                    Edge leave_start = g.AddEdge(leave, start);
                    leave_start.Description = "Start servicing the waiting job";
                    leave_start.Condition = "QUEUE > 0";
                    leave_start.Priority = "5";

                    Edge fail_fix = g.AddEdge(fail, fix);
                    fail_fix.Description = "After 30 minutes the machine will be fixed";
                    fail_fix.Delay = "30";
                    fail_fix.Priority = "6";

                    Edge fail_leave = g.AddEdge(fail, leave);
                    fail_leave.Action = EdgeAction.CancelNext;
                    fail_leave.Priority = "5";

                    Edge fix_fail = g.AddEdge(fix, fail);
                    fix_fail.Description = "Schedule the next machine failure";
                    fix_fail.Delay = "t_expovariate(1/15)";
                    fix_fail.Priority = "4";

                    Edge fix_start = g.AddEdge(fix, start);
                    fix_start.Condition = "QUEUE > 0";
                    fix_start.Priority = "5";

                    _breakdown = g;
                }

                return _breakdown;
            }
        }
        private static Graph _breakdown;
    }
}
