using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.AI.BehaviourTree
{
    public class SuccessDecorator : Decorator
    {
        public SuccessDecorator(Behaviour child)
            : base(child)
        {
        }

        protected override void OnTerminate(Behaviour.BStatus status)
        {
        }

        protected override BStatus Filter(BStatus initialState)
        {
            return BStatus.BH_SUCCESS;
        }
    }
}
