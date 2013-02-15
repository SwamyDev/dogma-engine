using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Interfaces;
using Pontification.Physics;

namespace Pontification.Components
{
    public class PossessionAbility : Component, IAbility
    {
        #region Private attributes
        private CharacterPhysicsComponent _physics;
        private AbilityManager _manager;
        private GameObject _victim;
        private Vector2 _collisionPoint;
        private Vector2 _start;
        private float _timeElapsed;
        private float _progress;
        private bool _isCasting;
        #endregion

        #region Public properties
        public Texture2D EffectSprite { get; set; }
        public Vector2 Offset { get; set; }
        public float Range { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<CharacterPhysicsComponent>();
            _manager = GetComponent<AbilityManager>();

            // If not present in the current game object add it.
            if (_manager == null)
                _manager = AddComponent<AbilityManager>();

            // Add ability to manager
            _manager.AddAbility(this);

            // Hook into pre update
            GameObject.Scene.PreUpdate += preUpdate;
        }

        public override void Removed()
        {
            _manager.RemoveAbility(this);
        }

        public override string ToString()
        {
            return "PossessionAbility";
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (_isCasting)
            {
                // Draw and scale the effect sprite for exorcise.
                Color drawColor = Color.White * (0.2f + 0.8f * _progress);
                Vector2 diff = Vector2.Normalize(_collisionPoint - _start) * ((_collisionPoint - _start).Length() + 40);
                Vector2 drawPos = _start + diff / 2;
                float rotation = (float)Math.Atan2(diff.Y, diff.X);
                float distance = diff.Length();
                float lengthScale = distance / EffectSprite.Width;

                sb.Draw(EffectSprite, drawPos, null, drawColor, rotation, new Vector2(EffectSprite.Width / 2, EffectSprite.Height / 2), new Vector2(lengthScale, 1), SpriteEffects.None, 0);
            }
        }
        #endregion

        #region IAbility methods
        public void Use()
        {
            if (!IsActive)
                return;

            if (_physics == null)
                throw new ArgumentNullException("PossessionAbility needs a physics component");

            SendMessage("LockMovement");

            // Player should always face cursor when using an ability.
            if (GameObject == SceneManagement.SceneInfo.Player)
                _physics.FaceCursor();

            float rotation = _physics.ViewRotation;

            var distance = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * Range;
            Vector2 start = GameObject.Position + new Vector2(Math.Abs(Offset.X) * _physics.Facing, Offset.Y);
            Vector2 end = GameObject.Position + _physics.ViewPoint + distance;

            _isCasting = true;
            _start = start;
            _collisionPoint = end;
            _progress = 0.0f;

            // Perform raycast and add damage.
            var world = GameObject.Scene.WorldInfo;
            world.RayCast((po, p, n) =>
            {
                _collisionPoint = ConvertUnits.ToDisplayUnits(p);
                if (po.GameObject != null)
                {
                    GameObject victim = po.GameObject;
                    Possessable possessInfo = victim.GetComponent<Possessable>();

                    if (possessInfo != null)
                    {
                        if (victim != _victim)
                        {
                            _timeElapsed = 0;
                            _victim = victim;
                        }

                        if (_timeElapsed >= possessInfo.PossessTime)
                        {
                            _timeElapsed = 0.0f;
                            _progress = 1f;
                            SendMessage("UnlockMovement");

                            // Deactivate player.
                            GameObject.IsActive = false;
                            GameObject.SendMessage("SetCharacterPhysicsActive", new object[] { false });

                            // Toggle animations.
                            _victim.GetComponents<AnimationComponent>().ForEach((c) => { c.IsActive = !c.IsActive; });

                            // Activate life drain.
                            _victim.GetComponent<LifeDrainAbility>().IsActive = true;

                            // Set player controller to victim to gain control.
                            _victim.SendMessage("DeactivateAI");
                            _victim.SendMessage("DeactivateSensitivity");
                            _victim.SendMessage("SetDrainer", new object[] { GameObject });
                            _victim.MoveComponent(GetComponent<PlayerInput>());
                            _victim.MoveComponent(GameObject.GetComponent<Camera>());
                            _victim.SendMessage("BlockPrimary");
                            possessInfo.IsActive = false;
                        }
                        else
                        {
                            _timeElapsed += Time.DeltaTime;
                            _progress = _timeElapsed / possessInfo.PossessTime;
                        }
                    }
                    else
                    {
                        _victim = null;
                        _timeElapsed = 0;
                    }
                }
                else
                {
                    _victim = null;
                    _timeElapsed = 0;
                }
                return false;
            }, ConvertUnits.ToSimUnits(start), ConvertUnits.ToSimUnits(end));
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void ResetTimer()
        {
            _timeElapsed = 0.0f;
        }
        #endregion

        #region Private methods
        private void preUpdate(object sender, EventArgs e)
        {
            _isCasting = false;
        }
        #endregion
    }
}
