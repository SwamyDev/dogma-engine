using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    public abstract class YieldState
    {
        internal abstract void Add(TaskItem task);
    }
}
