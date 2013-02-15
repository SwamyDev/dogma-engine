using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Components;
using Pontification.Interfaces;

namespace Pontification.AI
{
    public struct AIMemorySheet
    {
        public List<GameObject> Enemies;
        public CharacterCategory Category;
        public GameObject Target;
        public IAbility CurrentPrimary;
        public Vector2 StandingJumpVelocity;
        public Vector2 RunningJumpVelocity;
        public Vector2 Velocity;
        public Vector2 Position;
        public float MemoryBuffer;
        public float VisionAngle;
        public float VisionRange;
        public float Health;
        public int Facing;
    }
}
