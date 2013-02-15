using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification
{
    public class WaitForSeconds : YieldState
    {
        private float _ticks;

        public WaitForSeconds(float seconds)
        {
            _ticks = TimeSpan.FromSeconds(seconds).Ticks;
        }

        internal override void Add(TaskItem task)
        {
            throw new NotImplementedException();
        }
    }
}
