using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.AI.BehaviourTree
{
    public class NotDecorator : Decorator
    {
        public NotDecorator(Behaviour child)
            : base(child)
        {
        }

        protected override void OnTerminate(Behaviour.BStatus status)
        {
        }

        protected override BStatus Filter(BStatus initialState)
        {
            if (initialState == BStatus.BH_SUCCESS)
            {
                return BStatus.BH_FAILURE;
            }
            if (initialState == BStatus.BH_FAILURE)
            {
                return BStatus.BH_SUCCESS;
            }

            return initialState;
        }
    }
}
