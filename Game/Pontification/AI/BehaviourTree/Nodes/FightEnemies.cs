using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Interfaces;

namespace Pontification.AI.BehaviourTree
{
    public class FightEnemies : Sequence
    {
        public FightEnemies(IController controller)
        {
            _behaviours.Add(new CanSeeEnemy(controller));
            _behaviours.Add(new CanAttackTarget(controller));
            _behaviours.Add(new AttackEnemy(controller));
        }

        protected override void OnTerminate(Behaviour.BStatus status)
        {
        }
    }
}
