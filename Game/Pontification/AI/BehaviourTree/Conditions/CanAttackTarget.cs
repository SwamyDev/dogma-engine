using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Interfaces;
using Pontification.Physics;

namespace Pontification.AI.BehaviourTree
{
    public class CanAttackTarget : Behaviour
    {
        private IController _controller;

        public CanAttackTarget(IController controller)
        {
            _controller = controller;
        }

        protected override BStatus Update()
        {
            var memory = _controller.Memory;

            Vector2 start = memory.Position + new Vector2(Math.Abs(memory.CurrentPrimary.Offset.X) * memory.Facing, memory.CurrentPrimary.Offset.Y);
            Vector2 end = start + Vector2.Normalize(memory.Target.Position - start) * memory.CurrentPrimary.Range;
            var world = SceneManagement.SceneManager.Instance.FocusScene.WorldInfo;

            bool reachedTarget = false;
            world.RayCast((po, p, n) => 
            {
                if (po.GameObject != null && po.GameObject == memory.Target)
                    reachedTarget = true;

                return false; 
            }, ConvertUnits.ToSimUnits(start), ConvertUnits.ToSimUnits(end));

            if (reachedTarget)
                return BStatus.BH_SUCCESS;

            return BStatus.BH_FAILURE;
        }

        protected override void OnTerminate(BStatus status)
        {
        }
    }
}
