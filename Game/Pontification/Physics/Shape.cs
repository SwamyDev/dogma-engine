using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pontification.Physics
{
    public abstract class Shape
    {
        public enum ShapeType
        {
            SH_POLYGON,
            SH_ROUND,
            SH_CIRCLE,
        }

        public PhysicsObject Owner;

        public ShapeType Type { get; protected set; }

        public Shape(PhysicsObject owner)
        {
            Owner = owner;
        }

        public virtual void Draw(SpriteBatch sb)
        {
        }
    }
}
