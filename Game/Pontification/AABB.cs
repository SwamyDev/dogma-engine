using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification
{
    public struct AABB
    {
        public Vector2 Position;
        public Vector2 XHalfWidth;
        public Vector2 YHalfWidth;
        public Vector2 Offset;
    }
}
