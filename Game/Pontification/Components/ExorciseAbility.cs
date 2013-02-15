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
    public class ExorciseAbility : Component, IAbility
    {
        #region Private attributes
        private CharacterPhysicsComponent _physics;
        private AbilityManager _manager;
        private DamageTypes _damageType = DamageTypes.DT_HOLY;
        private Vector2 _collisionPoint;
        private Vector2 _start;
        private bool _isMovementLocked;
        private bool _isCasting;
        #endregion

        #region Public properties
        public Texture2D EffectSprite { get; set; }
        public Vector2 Offset { get; set; }
        public float Damage { get; set; }
        public float Range { get; set; }
        #endregion

        public ExorciseAbility()
        {
            Damage = 10f;
        }

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<CharacterPhysicsComponent>();
            _manager = GetComponent<AbilityManager>();

            // If not present in the current game object add it.
            if (_manager == null)
                _manager = AddComponent<AbilityManager>();

            // Add ability to manager
            _manager.AddAbility(this, false);

            // Hook into pre update
            GameObject.Scene.PreUpdate += preUpdate;
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isCasting && _isMovementLocked)
            {
                _isMovementLocked = false;
            }
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (_isCasting)
            {
                // Draw and scale the effect sprite for exorcise.
                Vector2 diff = Vector2.Normalize(_collisionPoint - _start) * ((_collisionPoint - _start).Length() + 40);
                Vector2 drawPos = _start + diff / 2;
                float rotation = (float)Math.Atan2(diff.Y, diff.X);
                float distance = diff.Length();
                float lengthScale = distance / EffectSprite.Width;

                sb.Draw(EffectSprite, drawPos, null, Color.White, rotation, new Vector2(EffectSprite.Width / 2, EffectSprite.Height / 2), new Vector2(lengthScale, 1), SpriteEffects.None, 0);
            }
        }
        
        public override string ToString()
        {
            return "ExorciseAbility";
        }
        #endregion

        protected override void disposing()
        {
            // Unhook when disposing
            GameObject.Scene.PreUpdate -= preUpdate;
        }

        #region IAbility methods
        public void Use()
        {
            if (_physics == null)
                throw new ArgumentNullException("ExorciseAbility needs a physics component");

            // Player should always face cursor when using an ability.
            if (GameObject == SceneManagement.SceneInfo.Player)
                _physics.FaceCursor();

            if (!_isMovementLocked)
            {
                SendMessage("LockMovement");
                _isMovementLocked = true;
            }

            float rotation = _physics.ViewRotation;

            var distance = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * Range;
            _start = GameObject.Position + new Vector2(Math.Abs(Offset.X) * _physics.Facing, Offset.Y);
            Vector2 end = GameObject.Position + _physics.ViewPoint + distance;

            _isCasting = true;
            _collisionPoint = end;

            // Perform raycast and add damage.
            var world = GameObject.Scene.WorldInfo;
            world.RayCast((po, p, n) =>
            {
                _collisionPoint = ConvertUnits.ToDisplayUnits(p);
                if (po.GameObject != null)
                {
                    po.GameObject.SendMessage("TakeDamage", new object[] { Damage, _damageType });
                }
                return false;
            }, ConvertUnits.ToSimUnits(_start), ConvertUnits.ToSimUnits(end));
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
