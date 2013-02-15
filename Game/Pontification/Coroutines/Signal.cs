using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    public class Signal : YieldState
    {
        static int nextId = int.MinValue;

        int id = nextId++;
        List<TaskItem> tasks = new List<TaskItem>();
        bool bIsSet = true;

        public void Set()
        {
            if (bIsSet)
                return;

            bIsSet = true;

            //Decrement the data property for all taks waiting for this signal.
            foreach (TaskItem task in tasks)
            {
                if (--task.Data == 0)
                {
                    //If Data is 0 then the task isn't waiting for any other signals -> Reschudule it.
                    task.Scheduler.AddToActive(task);
                }
            }
            tasks.Clear();
        }

        internal override void Add(TaskItem task)
        {
            //Signal only becomes unset when it has tasks.
            if (bIsSet)
            {
                bIsSet = false;
            }

            //Add to list of tasks waiting for signals.
            tasks.Add(task);
            //Data contains the number of signals we are still waiting for.
            task.Data++;
        }
    }
}
