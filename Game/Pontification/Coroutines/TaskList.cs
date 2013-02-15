using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    internal sealed class TaskList
    {
        public readonly Scheduler Scheduler;

        public TaskItem First { get; private set; }
        public TaskItem Last { get; private set; }

        public TaskList(Scheduler scheduler)
        {
            this.Scheduler = scheduler;
        }

        public void Append(TaskItem task)
        {
            Debug.Assert(task.Next == null);

            if (First == null)
            {
                Debug.Assert(Last == null);
                First = Last = task;
            }
            else
            {
                Debug.Assert(Last.Next == null);
                {
                    Last.Next = task;
                    Last = task;
                }
            }
        }

        public void Remove(TaskItem task, TaskItem previous)
        {
            if (previous == null)
            {
                Debug.Assert(task == First);
                First = task.Next;
            }
            else
            {
                Debug.Assert(previous.Next == task);
                previous.Next = task.Next;
            }

            if (task.Next == null)
            {
                Debug.Assert(Last == task);
                Last = previous;
            }

            task.Next = null;
        }

        public TaskEnumerator GetEnumerator()
        {
            return new TaskEnumerator(this);
        }

        public sealed class TaskEnumerator
        {
            TaskList list;
            TaskItem current, previous;

            public TaskItem Current { get { return current; } }

            public TaskEnumerator(TaskList list)
            {
                this.list = list;
                previous = current = null;
            }

            public bool MoveNext()
            {
                TaskItem next;

                if (current == null)
                {
                    if (previous == null)
                    {
                        next = list.First;
                    }
                    else
                    {
                        next = previous.Next;
                    }
                }
                else
                {
                    next = current.Next;
                }

                if (next != null)
                {
                    if (current != null)
                    {
                        previous = current;
                    }
                    current = next;
                    return true;
                }
                return false;
            }

            public void MoveCurrentToList(TaskList otherList)
            {
                otherList.Append(RemoveCurrent());
            }

            public TaskItem RemoveCurrent()
            {
                Debug.Assert(current != null);
                TaskItem ret = current;
                list.Remove(current, previous);
                current = null;
                return ret;
            }
        }
    }
}
