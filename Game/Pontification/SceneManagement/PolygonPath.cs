using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pontification.Components;
using Pontification.Physics;

namespace Pontification.SceneManagement
{
    public class PolygonPath : PathItem
    {
        public PolygonPath()
        {
        }

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);

            prop.SetValue(comp, new List<Vector2>(WorldPoints), null);
        }
    }
}
