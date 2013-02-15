using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class RotationComponent : Component
    {
        #region Public properties
        public float Speed { get; set; }
        #endregion

        #region Public methods
        public override void Update(GameTime gameTime)
        {
            float newRotation = (GameObject.Rotation + Speed * (MathHelper.Pi / 180.0f) * (float)gameTime.ElapsedGameTime.TotalSeconds) % MathHelper.TwoPi;
            GameObject.Rotation = newRotation;
        }
        #endregion
    }
}
