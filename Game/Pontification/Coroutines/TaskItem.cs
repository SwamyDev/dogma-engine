using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    internal class TaskItem
    {
        public readonly IEnumerator Task;
        public TaskItem Next;
        public Scheduler Scheduler;
        public long Data;

        public TaskItem(IEnumerator task, Scheduler scheduler)
        {
            this.Task = task;
            this.Scheduler = scheduler;
        }
    }
}
