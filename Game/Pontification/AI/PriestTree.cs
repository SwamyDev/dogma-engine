using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Components;
using Pontification.Interfaces;
using Pontification.AI.BehaviourTree;

namespace Pontification.AI
{
    public class PriestTree : Selector
    {
        public PriestTree(IController controller)
        {
            _behaviours.Add(new FightEnemies(controller));
            _behaviours.Add(new WalkTowardsTarget(controller));
        }

        protected override void OnTerminate(Behaviour.BStatus status)
        {
        }
    }
}
