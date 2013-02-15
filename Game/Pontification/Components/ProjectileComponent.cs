using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Components
{
    public class ProjectileComponent :  Component
    {
        #region Private attributes
        private PhysicsComponent _physics;
        private DamageTypes _damageType = DamageTypes.DT_PHYSICAL;
        #endregion

        #region Public properties
        public Texture2D Sprite { get; set; }
        public float Damage { get; set; }
        public float Velocity { get; set; }
        public float Angle { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            if (Sprite != null)
            {
                var texture = AddComponent<TextureComponent>();
                texture.Texture = Sprite;
                texture.Start();
            }

            _physics = AddComponent<PhysicsComponent>();

            var vertices = new List<Vector2>(4);
            vertices.Add(GameObject.Position + new Vector2(-10, -10));
            vertices.Add(GameObject.Position + new Vector2(10, -10));
            vertices.Add(GameObject.Position + new Vector2(10, 10));
            vertices.Add(GameObject.Position + new Vector2(-10, 10));

            _physics.Polygon = vertices;
            _physics.Friction = 0.0f;
            _physics.Restitution = 0.6f;
            _physics.Mass = 1f;
            _physics.IsStatic = false;
            _physics.IsProjectile = true;
            _physics.IgnoreGravity = true;
            _physics.Start();

            var velocity = new Vector2(Velocity * (float)Math.Cos(Angle), Velocity * (float)Math.Sin(Angle));
            _physics.SetVelocity(velocity);

            // Hook up events.
            _physics.OnCollision += onCollision;
        }
        #endregion

        protected override void disposing()
        {
            base.disposing();

            _physics.OnCollision -= onCollision;
        }

        #region Private methods
        private void onCollision(GameObject collider)
        {
            // Explode.
            if (collider != null)
                collider.SendMessage("TakeDamage", new object[] { Damage, _damageType });
            GameObject.IsActive = false;
            GameObject.Dispose();
        }
        #endregion
    }
}
