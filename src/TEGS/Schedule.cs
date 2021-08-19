// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

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
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
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
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
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
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            int removed = _events.RemoveAll(evt => CancelPredicate(evt, target, parameterValues));
            if (removed > 0)
            {
                OnScheduleChanged();
            }
        }

        private static bool CancelPredicate(ScheduledEvent match, Vertex target, IReadOnlyList<VariableValue> parameterValues)
        {
            if (match.Target == target)
            {
                if (parameterValues is null)
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
}
