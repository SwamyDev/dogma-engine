using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    public sealed class Scheduler
    {
        public static readonly Scheduler Instance = new Scheduler();

        TaskList _active, _sleeping;

        public Scheduler()
        {
            this._active = new TaskList(this);
            this._sleeping = new TaskList(this);
        }

        public void AddTask(IEnumerator task)
        {
            _active.Append(new TaskItem(task, this));
        }

        public void Run()
        {
            long nowTicks = DateTime.Now.Ticks;

            //Move woken tasks back to active.
            var en = _sleeping.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.Data < nowTicks)
                {
                    en.MoveCurrentToList(_active);
                }
            }

            //Run all active tasks.
            en = _active.GetEnumerator();

            while (en.MoveNext())
            {
                //Run each task's enumerator for one yield iteration.
                IEnumerator task = en.Current.Task;
                if (!task.MoveNext())
                {
                    //It is finished so remove it
                    en.RemoveCurrent();
                    continue;
                }

                //Check current state
                object state = task.Current;
                if (state == null)
                {
                    //Cooperative yielding -> state remains unchanged.
                    continue;
                }
                else if (state is TimeSpan)
                {
                    //Wants to sleep -> Move to sleeping list. Data value contains sleep duration.
                    en.Current.Data = nowTicks + ((TimeSpan)state).Ticks;
                    en.MoveCurrentToList(_sleeping);
                }
                else if (state is Signal)
                {
                    TaskItem signalTask = en.RemoveCurrent();
                    signalTask.Data = 0;
                    ((Signal)state).Add(signalTask);
                }
                else if (state is ICollection<Signal>)
                {
                    TaskItem signalTask = en.RemoveCurrent();
                    signalTask.Data = 0;

                    foreach (Signal signal in ((ICollection<Signal>)state))
                    {
                        signal.Add(signalTask);
                    }
                }
                else if (state is IEnumerable)
                {
                    throw new NotImplementedException("Nested tasks are not supported yet!");
                }
                else
                {
                    throw new InvalidOperationException("Unknown task state yielded: " + state.GetType().FullName);
                }
            }
        }

        internal void AddToActive(TaskItem task)
        {
            _active.Append(task);
        }
    }
}
