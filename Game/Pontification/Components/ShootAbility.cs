using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Interfaces;

namespace Pontification.Components
{
    public class ShootAbility : Component, IAbility
    {
        #region Private attributes
        private CharacterPhysicsComponent _physics;
        private AbilityManager _manager;
        private float _timeSinceLastShot;
        private bool _isMovementLocked;
        #endregion

        #region Public properties
        public Texture2D ProjectileSprite { get; set; }
        public Vector2 Offset { get; set; }
        public float Damage { get; set; }
        public float FireRate { get; set; }
        public float Velocity { get; set; }
        public float Range { get; set; }
        #endregion

        public ShootAbility()
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
            _timeSinceLastShot = FireRate;
        }

        public override void Update(GameTime gameTime)
        {
            if (_timeSinceLastShot < FireRate)
                _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
        }
        
        public override string ToString()
        {
            return "ShootAbility";
        }
        #endregion

        #region IAbility methods
        public void Use()
        {
            if (_physics == null)
                throw new ArgumentNullException("ExorciseAbility needs a physics component");

            SendMessage("LockMovement");

            if (_timeSinceLastShot >= FireRate)
            {
                spawnProjectile(_physics.ViewRotation);
                _timeSinceLastShot = 0.0f;
            }

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
        private void spawnProjectile(float rotation)
        {
            Console.WriteLine("Spawn projectile");
            Vector2 spawnPos = GameObject.Position + new Vector2(Math.Abs(Offset.X) * _physics.Facing, Offset.Y);
            var projectile = new GameObject("Projectile", spawnPos, GameObject.Scene, GameObject.Layer);
            var projectileComp = projectile.AddComponent<ProjectileComponent>();
            projectileComp.Sprite = ProjectileSprite;
            if (_physics.Facing >= 1)
                projectileComp.Angle = 0;
            else
                projectileComp.Angle = MathHelper.Pi;
            projectileComp.Damage = Damage;
            projectileComp.Velocity = Velocity;
            projectileComp.Start();
        }
        #endregion
    }
}
