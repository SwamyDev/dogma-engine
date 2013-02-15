using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Interfaces;
using Microsoft.Xna.Framework;
using Pontification.Physics;

namespace Pontification.AI.BehaviourTree
{
    public class WalkTowardsTarget : Behaviour
    {
        private IController _controller;
        private float _distanceTreshold = 120f;
        private float _jumpTestIntervall = 0.1f;
        private int _jumpTests = 10;

        public WalkTowardsTarget(IController controller)
        {
            _controller = controller;
        }

        protected override BStatus Update()
        {
            var memory = _controller.Memory;

            if (memory.Target == null)
            {
                _controller.Walk(0);
                return BStatus.BH_FAILURE;
            }

            int direction = Math.Sign(memory.Target.Position.X - memory.Position.X);

            if (memory.Facing != direction)
            {
                _controller.ChangeFacing(direction);
                if (memory.Velocity.X == 0)
                    memory.Facing = direction;
            }

            // Probe for abyss.
            var world = SceneManagement.SceneManager.Instance.FocusScene.WorldInfo;
            var start = memory.Position+ new Vector2(33 * memory.Facing, 83);
            var end = start + new Vector2(0, 1000);

            bool foundGround = false;
            world.RayCast((po, p, n) => 
            {
                foundGround = true;
                return false; 
            }, ConvertUnits.ToSimUnits(start), ConvertUnits.ToSimUnits(end));

            if (foundGround)
            {
                if ((memory.Target.Position - memory.Position).Length() >= _distanceTreshold)
                    _controller.Walk(direction);
            }
            else
            {
                // Check if we can jump over abyss.
                bool shouldJump = false;

                // Get grid coordinates.
                Vector2 gridCoordinates = memory.Position - SceneManagement.SceneManager.Instance.FocusScene.Origin;
                gridCoordinates = new Vector2((int)(gridCoordinates.X / Game.CellSize), (int)(gridCoordinates.Y / Game.CellSize));

                float footHeight = (gridCoordinates.Y + 2) * Game.CellSize;

                // Check if we can land somewhere when performing a running jump.
                float runJumpTestEndX = (gridCoordinates.X + (7 * memory.Facing)) * Game.CellSize + (Game.CellSize / 2f);
                float runJumpTestEndY = footHeight + (Game.CellSize * 2.5f);

                var runJumpTestEnd = new Vector2(runJumpTestEndX, runJumpTestEndY);
                var runJumpTestStart = runJumpTestEnd - Vector2.UnitY * Game.CellSize * 4;

                // Test for a ledge.
                world.RayCast((po, p, n) => 
                { 
                    // Calculate the point on the edge. (Hint: We didn't start the raycast at the edge because of glancing intersection.
                    Vector2 edge = p + new Vector2(-n.Y, n.X) * Game.CellSize / 2;
                    float ydiff = ConvertUnits.ToSimUnits(footHeight) - edge.Y;

                    if (ydiff <= 0.05f)
                        shouldJump = true;

                    return false;
                }, ConvertUnits.ToSimUnits(runJumpTestStart), ConvertUnits.ToSimUnits(runJumpTestEnd));

                if (shouldJump)
                {
                    _controller.Jump();
                }
                else
                {
                    _controller.Walk(0);
                }
            }

            return BStatus.BH_SUCCESS;
        }

        protected override void OnTerminate(BStatus status)
        {
        }

        /// <summary>
        /// Projects the point during a jump where it will be after the specified time.
        /// </summary>
        /// <param name="point">Point to project</param>
        /// <param name="time">Time that has passed</param>
        /// <returns>Projected point</returns>
        private Vector2 projectedPoint(Vector2 point, float time)
        {
            Vector2 jumpVelocity = Vector2.Zero;
            var memory = _controller.Memory;
            var world = SceneManagement.SceneManager.Instance.FocusScene.WorldInfo;

            if (memory.Velocity.X != 0)
            {   // Running jump.
                jumpVelocity = new Vector2(Math.Abs(memory.RunningJumpVelocity.X) * memory.Facing, -memory.RunningJumpVelocity.Y);
            }
            else
            {   // Standing jump.
                jumpVelocity = new Vector2(Math.Abs(memory.StandingJumpVelocity.X) * memory.Facing, -memory.StandingJumpVelocity.Y);
            }

            point += jumpVelocity * time + (world.Gravity / 2) * time * time;

            return point;
        }
    }
}
