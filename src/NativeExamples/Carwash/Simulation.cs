// 
// Simulation.cs
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

using System;

namespace Carwash
{
    enum EventType
    {
        RUN,
        ENTER,
        START,
        LEAVE,
    }

    class Simulation : SimulationBase
    {
        protected override EventType StartingEvent => EventType.RUN;

        // User Variables
        int QUEUE = default;
        int SERVERS = default;

        public Simulation() { }

        protected override void ProcessEvent(EventType eventType, int[] parameterValues)
        {
            switch (eventType)
            {
                case EventType.RUN:
                    Event_RUN(parameterValues);
                    break;
                case EventType.ENTER:
                    Event_ENTER();
                    break;
                case EventType.START:
                    Event_START();
                    break;
                case EventType.LEAVE:
                    Event_LEAVE();
                    break;
            }
        }

        public void Event_RUN(int[] parameterValues)
        {
            // Process Parameters
            QUEUE = parameterValues[0];
            SERVERS = parameterValues[1];

            // Edges
            ScheduleEvent(EventType.ENTER, delay: 0, priority: 5, parameterValues: null);
        }

        public void Event_ENTER()
        {
            // Event Body
            QUEUE = QUEUE + 1;

            // Edges
            ScheduleEvent(EventType.ENTER, delay: Random.UniformVariate(3, 8), priority: 6, parameterValues: null);
            if (SERVERS > 0)
            {
                ScheduleEvent(EventType.START, delay: 0, priority: 5, parameterValues: null);
            }
        }

        public void Event_START()
        {
            // Event Body
            SERVERS = SERVERS - 1;
            QUEUE = QUEUE - 1;

            // Edges
            ScheduleEvent(EventType.LEAVE, delay: Random.UniformVariate(5, 20), priority: 6, parameterValues: null);
        }

        public void Event_LEAVE()
        {
            // Event Body
            SERVERS = SERVERS + 1;

            // Edges
            if (QUEUE > 0)
            {
                ScheduleEvent(EventType.START, delay: 0, priority: 5, parameterValues: null);
            }
        }
    }
}
