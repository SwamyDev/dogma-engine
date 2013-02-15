using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Pontification.AI;
using Pontification.Interfaces;

namespace Pontification.AI.BehaviourTree
{
    public class CanSeeEnemy : Behaviour
    {
        private IController _controller;
        private bool _forgetTimerStarted;
        private bool _resetTimer;

        public CanSeeEnemy(IController controller)
        {
            _controller = controller;
        }

        protected override BStatus Update()
        {
            AIMemorySheet memory = _controller.Memory;

            if (memory.Enemies != null)
            {
                for (int i = 0; i < memory.Enemies.Count; i++)
                {
                    GameObject enemy = memory.Enemies[i];
                    Vector2 diff = enemy.Position - memory.Position;
                    // Check if in range. (Maybe remove that check as we do that already in the Controller class)
                    if (diff.Length() <= memory.VisionRange)
                    {
                        // See if enemy is in vision cone.
                        diff.Normalize();
                        float angle = (float)Math.Atan2(diff.Y, diff.X);
                        float rotation = memory.Facing < 0 ? MathHelper.Pi : 0;
                        float angleDiff = MathHelper.WrapAngle(angle - rotation) * (180 / MathHelper.Pi);

                        if ((angleDiff >= memory.VisionAngle / -2f && angleDiff <= memory.VisionAngle / 2f) || memory.VisionAngle == 360.0f)
                        {
                            // If we are in vision angle then make a ray cast to test if enemy is in line of sight.
                            var world = SceneManagement.SceneManager.Instance.FocusScene.WorldInfo;
                            bool foundTarget = false;
                            world.RayCast((po, p, n) => 
                            {
                                if (po.GameObject != null)
                                {
                                    if (po.GameObject == enemy)
                                    {
                                        foundTarget = true;
                                        _controller.SetTarget(enemy);
                                    }
                                }
                                return false; 
                            }, Pontification.Physics.ConvertUnits.ToSimUnits(memory.Position), Pontification.Physics.ConvertUnits.ToSimUnits(enemy.Position));

                            if (foundTarget)
                            {
                                if (_forgetTimerStarted)
                                {
                                    _forgetTimerStarted = false;
                                    _resetTimer = true;
                                }
                                return BStatus.BH_SUCCESS;
                            }
                        }
                    }
                }
            }

            if (!_forgetTimerStarted && memory.Target != null)
                Scheduler.Instance.AddTask(forgetTarget());
            
            return BStatus.BH_FAILURE;
        }

        protected override void OnTerminate(BStatus status)
        {
        }

        private IEnumerator forgetTarget()
        {
            _forgetTimerStarted = true;
            yield return TimeSpan.FromSeconds(_controller.Memory.MemoryBuffer);

            if (_resetTimer)
            {
                _resetTimer = false;
            }
            else
            {
                _controller.SetTarget(null);
                _forgetTimerStarted = false;
            }
        }
    }
}
