using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.AI.BehaviourTree;
using Pontification.Interfaces;

namespace Pontification.AI
{
    public class SpiritTree : Sequence
    {
        public SpiritTree(IController controller)
        {
            _behaviours.Add(new CanSeeEnemy(controller));
            _behaviours.Add(new AttackEnemy(controller));
        }

        protected override void OnTerminate(Behaviour.BStatus status)
        {
        }
    }
}
