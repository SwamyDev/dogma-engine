using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pontification.Monitoring;
using Pontification.SceneManagement;

namespace Pontification.Physics
{
    public enum CharacterPhysicsState
    {
        CPS_IDLE,
        CPS_WALKING,
        CPS_RUNNING,
        CPS_PUSHING,
        CPS_JUMPOFF,
        CPS_MIDAIR,
        CPS_LANDING
    }

    public class CharacterObject : DynamicObject
    {
        #region Private attributes
        private enum JumpState
        {
            JS_LANDED,
            JS_JUMPED,
            JS_MIDAIR
        }

        private List<Vector2> _checkLandingPoints = new List<Vector2>();

        private Vector2 _standingJumpVelocity;
        private Vector2 _runningJumpVelocity;

        private float _maxSlope = 0.65f;

        private float _cellThreshold = 0.002f;

        private Vector2 _walkTowardTarget;
        private float _walkTowardThreshold;
        private int _oldFacing;
        #endregion

        #region Public properties
        public CharacterPhysicsState CharacterState { get; private set; }
        public int Facing { get; private set; }
        public bool IsAutonomWalking { get; set; }
        public override bool IsStatic { get { return false; } }
        public float LandingTime = 0.28f;   // Time needed to land - landing animation

        public Vector2 StandingJumpVelocity { get { return _standingJumpVelocity; } }
        public Vector2 RunningJumpVelocity { get { return _runningJumpVelocity; } }
        #endregion

        public CharacterObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, float runSpeed, float airSpeed, float maxSpeed, Vector2 standingJumpCells, Vector2 runningJumpCells, Vector2 rectangle)
            : base(worldInfo, position, mass, friction, restitution, rectangle)
        {
            // Calculate velocity to jump accuratly requested cells.
            float standWidth = ConvertUnits.ToSimUnits(standingJumpCells.X * Game.CellSize);
            float standHeight = ConvertUnits.ToSimUnits(standingJumpCells.Y * Game.CellSize);
            float runWidth = ConvertUnits.ToSimUnits(runningJumpCells.X * Game.CellSize);
            float runHeight = ConvertUnits.ToSimUnits(runningJumpCells.Y * Game.CellSize);
            float g = WorldInfo.Gravity.Y;

            // Start with height so we can get flight time.
            float standVelY = (float)Math.Sqrt(2 * standHeight * g);
            float runVelY = (float)Math.Sqrt(2 * runHeight * g);

            // Calculate time how long the character will remain in air.
            float standDeltaTime = 2 * (standVelY / g);
            float runDeltaTime = 2 * (runVelY / g);

            // Now calculate x velocity needed to character reaches requested cell with.
            float standVelX = standWidth / standDeltaTime;
            float runVelX = runWidth / runDeltaTime;

            _standingJumpVelocity = new Vector2(standVelX, standVelY);
            _runningJumpVelocity = new Vector2(runVelX, runVelY);

            Facing = 1;
            _oldFacing = Facing;
        }

        public override void AddForce(Vector2 force)
        {
        }

        public override void ApplyImpulse(Vector2 impulse)
        {
        }

        // Performs a step to the next full cell of the grid.
        // Pass in the direction as with walk.
        //
        // All vectors are projected on the x-Axis:
        //
        // Origin...                Or
        // Floating Position...     Pf
        // Projected Position...    Pp
        // Cell Position...         Pc
        // Character velocity...    vc
        // Cell Size...             dc
        //
        public void Step(int direction)
        {
            // Can't step in the air.
            if (MotionState == MotionStates.MS_MIDAIR)
                return;

            var Or = new Vector2(ConvertUnits.ToSimUnits(SceneManager.Instance.FocusScene.Origin.X - Game.CellSize / 2), -Game.CellSize / 2);
            var Pf = new Vector2(Position.X, 0); // new Vector2(Owner.Position.X, 0);
            float dc = ConvertUnits.ToSimUnits(Game.CellSize / 2);
            var cx = new Vector2(dc, 0);

            direction = (sbyte)Math.Sign(direction);

            // Get the projected position which we want to reach.
            var Pp = cx * (float)(Math.Round(((Pf.X - Or.X) / dc - (1f * Facing))) + Facing) + Or;

            // Is walk towards projected position if needed.
            WalkToward(Pp, 0.025f);
        }

        float minDiff = float.PositiveInfinity;

        // Autonomously walks towards specified position.
        public void WalkToward(Vector2 position, float threshold = 0.2f)
        {
            _walkTowardTarget = position;
            _walkTowardThreshold = threshold;

            //Vector2 curPos = new Vector2(Owner.Position.X, 0);
            Vector2 curPos = new Vector2(Position.X, 0);// new Vector2(0, 0); /// USE PARENT GAME OBJECT HERE
            position.Y = 0;
            Vector2 diff = position - curPos;

            if (diff.Length() < minDiff)
            {
                minDiff = diff.Length();
            }

            // Debug.Log(ConvertUnits.ToDisplayUnits(diff.Length()));
            // Debug.Log(ConvertUnits.ToDisplayUnits(minDiff));

            if (diff.Length() > threshold && Math.Sign(diff.X) == Facing)
            {
                Walk((sbyte)Math.Sign(diff.X));
                IsAutonomWalking = true;
            }
            else
            {
                /// USE PARENT GAME OBJECT HERE
                // Owner.Position = new Vector2(position.X, Owner.Position.Y);  
                Position = new Vector2(position.X, Position.Y);
                Velocity = Vector2.Zero;
                IsAutonomWalking = false;
            }
        }

        public void ChangeFacing(int direction)
        {
            if (Velocity.X == 0)
                Facing = direction;
        }

        //Pass in -1 for left and +1 for right walking and 0 to stop
        public void Walk(int direction)
        {

            if (MotionState == MotionStates.MS_LANDED)
                Velocity.X = direction * 4.5f;
        }

        IEnumerator Move(sbyte direction)
        {
            /// USE PARENT GAME OBJECT HERE
            Vector2 endPosition = Position + new Vector2(ConvertUnits.ToSimUnits(105) * direction, 0); // Owner.Position + new Vector2(ConvertUnits.ToSimUnits(105) * direction, 0);
            float diff = endPosition.X - Position.X; // Owner.Position.X;

            while (Math.Abs(diff) > 0.05f)
            {
                diff = endPosition.X - Position.X; // Owner.Position.X;
                float newVelocity = 2f * direction;
                float projectedPosition = newVelocity * Time.DeltaTime;

                if (Math.Abs(diff) <= Math.Abs(projectedPosition))
                {
                    newVelocity = diff / Time.DeltaTime;
                }

                Velocity.X = newVelocity;
                yield return null;
            }

            Velocity.X = 0f;
            yield break;
        }

        //Let's the character object jump with the specified JumpVelocity
        public void Jump()
        {
            // We can only jump when we stand on something.
            if (MotionState == MotionStates.MS_LANDED)
            {
                /// USE PARENT GAMEOBJECT HERE!!
                // var pawn = _owningPawn;

                if (Velocity.X > -0.05f && Velocity.X < 0.05f)
                {
                    // We got a standing jump.
                    _standingJumpVelocity.X = Math.Abs(_standingJumpVelocity.X) * -Facing; // -pawn.Facing;
                    Velocity = Vector2.Zero;
                    Velocity -= _standingJumpVelocity;
                }
                else
                {
                    // We got a running jump.
                    _runningJumpVelocity.X = Math.Abs(_runningJumpVelocity.X) * -Facing; // -pawn.Facing;
                    Velocity = Vector2.Zero;
                    Velocity -= _runningJumpVelocity;
                }
                CharacterState = CharacterPhysicsState.CPS_JUMPOFF;
                // _owningPawn.ActiveAnimation = Pawn.AnimationStates.AS_JUMPOFF;

                MotionState = MotionStates.MS_MIDAIR;

                Scheduler.Instance.AddTask(SetToMidAir());
            }
        }

        IEnumerator SetToMidAir()
        {
            yield return TimeSpan.FromSeconds(0.1f);
            if (MotionState == MotionStates.MS_MIDAIR)
            {
                CharacterState = CharacterPhysicsState.CPS_MIDAIR;
            }
        }

        public override void Update(float deltaTime)
        {
            if (IsAutonomWalking)
            {
                WalkToward(_walkTowardTarget, _walkTowardThreshold);
            }

            base.Update(deltaTime);

            if (Velocity.X != 0)
                Facing = Math.Sign(Velocity.X);

            if (CharacterState == CharacterPhysicsState.CPS_PUSHING)
            {
                if (Facing != _oldFacing)
                    CharacterState = CharacterPhysicsState.CPS_IDLE;
            }

            _oldFacing = Facing;

            if (MotionState == MotionStates.MS_LANDED)
            {
                if (Math.Abs(Velocity.X) > 0.001f)
                {
                    if (CharacterState != CharacterPhysicsState.CPS_PUSHING)
                        CharacterState = CharacterPhysicsState.CPS_WALKING;
                }
                else
                {
                    CharacterState = CharacterPhysicsState.CPS_IDLE;
                }
            }
            else if (MotionState == MotionStates.MS_MIDAIR && CharacterState != CharacterPhysicsState.CPS_LANDING && Velocity.Y > 0)
            {
                // Get the lower vertices in direction of the jump
                Vector2 leftVertex = Position + new Vector2(-BoundingBox.XHalfWidth.X, BoundingBox.YHalfWidth.Y);
                Vector2 rightVertex = Position + new Vector2(BoundingBox.XHalfWidth.X, BoundingBox.YHalfWidth.Y);

                // Project it to expected position.
                leftVertex += Velocity * LandingTime + (WorldInfo.Gravity / 2) * LandingTime * LandingTime;
                rightVertex += Velocity * LandingTime + (WorldInfo.Gravity / 2) * LandingTime * LandingTime;

                _checkLandingPoints.Add(ConvertUnits.ToDisplayUnits(leftVertex));
                _checkLandingPoints.Add(ConvertUnits.ToDisplayUnits(rightVertex));

                // Test projected point for collision.
                Bag<PhysicsObject> leftObjects = WorldInfo.TestPointAll(leftVertex);
                Bag<PhysicsObject> rightObjects = WorldInfo.TestPointAll(rightVertex);
                leftObjects.AddAll(rightObjects);

                for (int i = 0; i < leftObjects.Count; i++)
                {
                    var curObject = leftObjects[i];

                    if (curObject == this)
                        continue;

                    if (!curObject.IsSensor && curObject.IsActive)
                    {
                        // We got a collision start with landing animation and stop loop
                        CharacterState = CharacterPhysicsState.CPS_LANDING;
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            _checkLandingPoints.ForEach((p) => { Primitives.Instance.DrawPoint(sb, p, Color.Red, 4); });
        }

        public bool CollidingWithFloor(PhysicsObject physicsObject, Vector2 point, Vector2 normal)
        {
            /// USE PARENT GAMEOBJECT HERE!!
            // _owningPawn.ActiveAnimation = Pawn.AnimationStates.AS_LANDING;
            CharacterState = CharacterPhysicsState.CPS_LANDING;

            return true;
        }

        public override void Collided(PhysicsObject collider, Edge collidingEdge, Vector2 projectionVector, float deltaTime)
        {
            if (!IsActive)
                return;

            if (IsEthereal || collider.IsEthereal)
                return;

            _projectionVector = projectionVector;
            // After collision project out of colliding surface.
            Position -= projectionVector;
            
            if (projectionVector.Length() == 0.0f)  // Return when no projection.
                return;
            // Split reflection velocity into friction and bounce vector.
            var surfaceNorm = Vector2.Normalize(projectionVector);
            var surfaceDir = new Vector2(surfaceNorm.Y, -surfaceNorm.X);

            _surfaceNormal = surfaceNorm;

            // Get collision point
            //

            // Distance to line
            Vector2 p1 = collidingEdge.P1;
            Vector2 p2 = collidingEdge.P2;
            float normalLength = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            float dist = Math.Abs((Position.X - p1.X) * (p2.Y - p1.Y) - (Position.Y - p1.Y) * (p2.X - p1.X)) / normalLength;
            Vector2 collisionPoint = Position + surfaceNorm * dist; 

            if ((p1 - collisionPoint).Length() < 0.3f || (p2 - collisionPoint).Length() < 0.3f)
            {
                //Position += surfaceDir * 0.09f * Math.Sign(Velocity.X);
            }

            Vector2 bounce = surfaceNorm * Vector2.Dot(Velocity, surfaceNorm) * Restitution * -1 * 0;
            Vector2 friction = surfaceDir * Vector2.Dot(Velocity, surfaceDir) * (1 - Friction);

            if (Vector2.Dot(surfaceNorm, Vector2.UnitY) > _maxSlope)
            {
                MotionState = MotionStates.MS_LANDED;

                // Enable character to walk up and down a specific slope at a certain speed
                Vector2 velDirection = surfaceDir;
                if (Vector2.Dot(velDirection, Velocity) < 0)
                    velDirection = -velDirection;

                Velocity = velDirection * Velocity.Length() * Friction;

                // Move with dynamic object.
                if (collider.IsStatic == false)
                {
                    var dynamicObject = collider as DynamicObject;
                    Position += dynamicObject.Velocity * deltaTime;
                }
            }
            else
            {
                _bounceVector = bounce;
                _frictionVector = friction;

                // Add up to reflection vector.
                _reflectionVector = bounce + friction;
                Velocity = _reflectionVector;
            }

            // Check if we push colliding object.
            float xProj = Math.Abs(Vector2.Dot(surfaceNorm, Vector2.UnitX));
            if (xProj > 0.9f && xProj < 1.1f)
                CharacterState = CharacterPhysicsState.CPS_PUSHING;
        }
    }
}
