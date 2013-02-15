using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;
using Pontification.Monitoring;

namespace Pontification.Components
{
    public class CharacterPhysicsComponent : Component
    {
        #region Private attributes
        private CharacterObject _character;
        private CharacterPhysicsState _oldState;
        #endregion

        #region Public properties
        public Vector2 StandingJumpCells { get; set; }
        public Vector2 RunningJumpCells { get; set; }
        public Vector2 CollisionShapeCells { get; set; }
        public Vector2 ViewPoint { get; set; }
        public float ViewRotation { get; set; }
        public float Rotation { get; set; }
        public float Mass { get; set; }
        public int Facing { get; set; }

        public CharacterPhysicsState State { get { return _character.CharacterState; } }
        public Vector2 StandingJumpVelocity { get { return _character.StandingJumpVelocity; } }
        public Vector2 RunningJumpVelocity { get { return _character.RunningJumpVelocity; } }
        public Vector2 Velocity { get { return _character.Velocity; } }
        public Vector2 PreviouseVelocity;
        #endregion

        #region Delegates
        public delegate void StateChangeCallback(CharacterPhysicsState newState);
        public StateChangeCallback StateChanged;
        #endregion

        public CharacterPhysicsComponent()
        {
        }

        #region Public methods;
        public override void Start()
        {
            _character = new CharacterObject(
                GameObject.Scene.WorldInfo,
                ConvertUnits.ToSimUnits(GameObject.Position),
                Mass,
                0.1f,
                0f,
                10f,
                12f,
                20f,
                StandingJumpCells,
                RunningJumpCells,
                ConvertUnits.ToSimUnits(CollisionShapeCells * Game.CellSize));

            _character.GameObject = GameObject;
            _character.Restitution = 0f;
            GameObject.Scene.PreUpdate += preUpdate;
        }

        public override void Update(GameTime gameTime)
        {
            if (ViewRotation >= -MathHelper.Pi / 2 && ViewRotation <= MathHelper.Pi / 2)
                _character.ChangeFacing(1);
            else
                _character.ChangeFacing(-1);

            GameObject.Position = ConvertUnits.ToDisplayUnits(_character.Position);
            Facing = _character.Facing;
            ViewPoint = new Vector2(Math.Abs(ViewPoint.X) * Facing, ViewPoint.Y);

            if (_oldState != State)
            {
                if (StateChanged != null)
                    StateChanged(State);
                _oldState = State;
            }
        }

        public void Jump()
        {
            _character.Jump();

            if (GameObject == SceneManagement.SceneInfo.Player)
                Logger.Instance.Log("Jumped", MessageType.MT_STATISTICS);
        }

        public void Walk(int direction)
        {
            if (direction != 0)
            {
                direction = Math.Sign(direction);
                ChangeFacing(direction);
            }
            _character.Walk(direction);
        }

        public void Step(int direction)
        {
            if (direction != 0)
                direction = Math.Sign(direction);

            _character.Step(direction);
        }

        public void ChangeFacing(int direction)
        {
            if (direction >= 1)
                ViewRotation = MathHelper.Pi / 2f;
            else
                ViewRotation = MathHelper.Pi;

            _character.ChangeFacing(direction);
        }

        public void FaceCursor()
        {
            Vector2 end = Pontification.SceneManagement.SceneInfo.Cursor.Position;
            Vector2 start = GameObject.Position + ViewPoint;

            var dirction = Vector2.Normalize((end - start));
            ViewRotation = (float)Math.Atan2(dirction.Y, dirction.X);
        }

        public void Teleport(Vector2 position)
        {
            _character.Position = ConvertUnits.ToSimUnits(position);
            GameObject.Position = position;
        }

        public void AddVelocity(Vector2 velocity)
        {
            _character.Velocity += velocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _character.Velocity = velocity;
        }

        public void SetCharacterPhysicsActive(bool active)
        {
            _character.IsActive = active;
        }
        #endregion

        protected override void disposing()
        {
            GameObject.Scene.PreUpdate -= preUpdate;
            _character.Dispense();
        }

        #region Private methods
        private void preUpdate(object sender, EventArgs e)
        {
            PreviouseVelocity = Velocity;
        }
        #endregion
    }
}
