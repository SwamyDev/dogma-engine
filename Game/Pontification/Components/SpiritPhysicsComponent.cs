using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;

namespace Pontification.Components
{
    public enum SpiritForm
    {
        SF_BALL,
        SF_SPIRIT,
        SF_CHASER
    }

    public enum SpiritState
    {
        SS_IDLE,
        SS_MOVING
    }
    /// <summary>
    /// Handles spirit movement and collision.
    /// </summary>
    public class SpiritPhysicsComponent :  Component
    {
        #region Private attributes
        private DynamicObject _physics;
        private SpiritState _oldState;
        #endregion

        #region Public properties
        public SpiritForm Form { get; private set; }
        public SpiritState State { get; private set; }
        public float HoverSpeed { get; set; }
        public float ChaseSpeed { get; set; }
        public int Facing { get; set; }
        #endregion

        public delegate void OnFormChangeCallback(SpiritForm form);
        public delegate void OnStateChangeCallback(SpiritState state);
        public OnFormChangeCallback OnFormChange;
        public OnStateChangeCallback OnStateChange;

        #region Public methods
        public override void Start()
        {
            Vector2 position = ConvertUnits.ToSimUnits(GameObject.Position);
            List<Vector2> polygon = new List<Vector2>(4);
            polygon.Add(ConvertUnits.ToSimUnits(GameObject.Position + new Vector2(-29, -29)));
            polygon.Add(ConvertUnits.ToSimUnits(GameObject.Position + new Vector2(29, -29)));
            polygon.Add(ConvertUnits.ToSimUnits(GameObject.Position + new Vector2(29, 29)));
            polygon.Add(ConvertUnits.ToSimUnits(GameObject.Position + new Vector2(-29, 29)));
            _physics = new DynamicObject(GameObject.Scene.WorldInfo, position, 2f, 0f, 0f, polygon);
            _physics.IgnoreGravity = true;
            _physics.OnCollision += (A, B, p) =>
            {
                if (A.GameObject != null)
                {
                    if (A.GameObject == SceneManagement.SceneInfo.Player)
                        ScreenManagement.ScreenManager.Instance.Game.Exit();
                        //A.GameObject.SendMessage("Kill");
                }
                else if (B.GameObject != null)
                {
                    if (B.GameObject == SceneManagement.SceneInfo.Player)
                        ScreenManagement.ScreenManager.Instance.Game.Exit();
                        //B.GameObject.SendMessage("Kill");
                }
            };

            if (OnFormChange != null)
                OnFormChange(SpiritForm.SF_BALL);
        }

        public override void Update(GameTime gameTime)
        {
            GameObject.Position = ConvertUnits.ToDisplayUnits(_physics.Position);

            if (_physics.Velocity == Vector2.Zero)
            {
                State = SpiritState.SS_IDLE;
            }
            else
            {
                State = SpiritState.SS_MOVING;
                if (_physics.Velocity.X < 0)
                    Facing = -1;
                else
                    Facing = 1;
            }

            if (_oldState != State)
            {
                if (OnStateChange != null)
                    OnStateChange(State);
            }
            _oldState = State;
        }

        public void Fly(Vector2 direction)
        {
            if (direction == Vector2.Zero)
                return;

            Vector2 velocity = Vector2.Zero;
            if (Form == SpiritForm.SF_SPIRIT)
                velocity = Vector2.Normalize(direction) * HoverSpeed;
            else if (Form == SpiritForm.SF_CHASER)
                velocity = Vector2.Normalize(direction) * ChaseSpeed;

            _physics.Velocity = velocity;
        }

        public void ChangeForm(SpiritForm form)
        {
            Form = form;
            if (OnFormChange != null)
                OnFormChange(form);
        }

        public Vector2 GetVelocity()
        {
            return _physics.Velocity;
        }
        #endregion

        protected override void disposing()
        {
            _physics.Dispense();
        }
    }
}
