﻿// 
// Schedule.cs
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
using System.Collections.Generic;

namespace TEGS
{
    public delegate void ScheduleChangedEventHandler(object sender, ScheduleChangedEventArgs e);

    public class Schedule
    {
        public Graph Graph { get; private set; }

        public IEnumerable<ScheduledEvent> Events => _events;
        private readonly List<ScheduledEvent> _events = new List<ScheduledEvent>();

        public int EventCount => _events.Count;

        public event ScheduleChangedEventHandler ScheduleChanged;

        public Schedule(Graph graph)
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        public ScheduledEvent GetNext()
        {
            if (_events.Count == 0)
            {
                throw new InvalidOperationException();
            }

            ScheduledEvent next = _events[0];
            _events.RemoveAt(0);
            OnScheduleChanged();
            return next;
        }

        public void Insert(Vertex target, double time, double priority, IReadOnlyList<VariableValue> parameterValues)
        {
            if (null == target)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.Graph != Graph)
            {
                throw new GraphMismatchException();
            }

            ScheduledEvent item = new ScheduledEvent(target, time, priority, parameterValues);

            int index = _events.BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }

            if (index == _events.Count)
            {
                _events.Add(item);
            }
            else
            {
                _events.Insert(index, item);
            }
            
            OnScheduleChanged();
        }

        public void CancelNext(Vertex target, IReadOnlyList<VariableValue> parameterValues = null)
        {
            if (null == target)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.Graph != Graph)
            {
                throw new GraphMismatchException();
            }

            bool removed = false;

            for (int i = 0; i < _events.Count; i++)
            {
                if (CancelPredicate(_events[i], target, parameterValues))
                {
                    _events.RemoveAt(i);
                    removed = true;
                    break;
                }
            }

            if (removed)
            {
                OnScheduleChanged();
            }
        }

        public void CancelAll(Vertex target, IReadOnlyList<VariableValue> parameterValues = null)
        {
            if (null == target)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.Graph != Graph)
            {
                throw new GraphMismatchException();
            }

            int removed = _events.RemoveAll(evt => CancelPredicate(evt, target, parameterValues));
            if (removed > 0)
            {
                OnScheduleChanged();
            }
        }

        private bool CancelPredicate(ScheduledEvent match, Vertex target, IReadOnlyList<VariableValue> parameterValues)
        {
            if (match.Target == target)
            {
                if (null == parameterValues)
                {
                    return true;
                }
                else if (parameterValues.Count == match.ParameterValues.Count)
                {
                    for (int i = 0; i < parameterValues.Count; i++)
                    {
                        if (parameterValues[i] != match.ParameterValues[i])
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        private void OnScheduleChanged()
        {
            ScheduleChanged?.Invoke(this, new ScheduleChangedEventArgs(EventCount));
        }

        public const double MinTime = 0.0;
        public const double DefaultDelay = 0.0;
        public const double DefaultPriority = 0.0;
    }

    public class ScheduleChangedEventArgs : EventArgs
    {
        public int EventCount { get; private set; }

        public ScheduleChangedEventArgs(int eventCount)
        {
            EventCount = eventCount;
        }
    }

    #region Exceptions

    [Serializable]
    public class GraphMismatchException : Exception
    {
        public GraphMismatchException() : base() { }
    }

    #endregion
}
