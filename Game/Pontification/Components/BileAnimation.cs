using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class BileAnimation : Component
    {
        public override void Start()
        {
            var anim = GetComponent<AnimationComponent>();
            if (anim != null)
                anim.Sprite.Rotation = (float)Math.PI / 2;
        }
    }
}
