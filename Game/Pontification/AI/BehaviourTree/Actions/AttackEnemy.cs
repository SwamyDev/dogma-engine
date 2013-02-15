using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Interfaces;

namespace Pontification.AI.BehaviourTree
{
    public class AttackEnemy : Behaviour
    {
        private IController _controller;

        public AttackEnemy(IController controller)
        {
            _controller = controller;
        }

        protected override BStatus Update()
        {
            _controller.Attack();

            return BStatus.BH_SUCCESS;
        }

        protected override void OnTerminate(BStatus status)
        {
        }
    }
}
