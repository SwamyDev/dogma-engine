using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class RotateAroundPivot : Component
    {
        #region Private attribues
        private PhysicsComponent _physics;
        #endregion

        #region Public properties
        public GameObject Pivot { get; set; }
        public float RotationSpeed { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 diff = Pivot.Position - GameObject.Position;
            float angle = (float)Math.Atan2(diff.Y, diff.X);
            angle += RotationSpeed * (MathHelper.Pi / 180.0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 newPos = new Vector2((float)Math.Cos(angle) * diff.Length(), (float)Math.Sin(angle) * diff.Length());

            if (_physics != null)
                _physics.MoveTo(newPos);
            else
                GameObject.Position = newPos;
        }
        #endregion
    }
}
