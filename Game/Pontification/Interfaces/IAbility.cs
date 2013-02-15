using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Interfaces
{
    public interface IAbility
    {
        Vector2 Offset { get; set; }
        float Range { get; set; }

        void Use();
        void Activate();
        void Deactivate();
        void ResetTimer();
    }
}
