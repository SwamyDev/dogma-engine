using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pontification.Physics
{
    public struct CellValues
    {
        public int Row;
        public int Column;
        public int Index;   // Index of this object within the cell.
    }

    public abstract class PhysicsObject
    {
        public int WorldIndex; // This is the index of this physics object of the world's bag list.

        public bool IsActive = true;

        public GameObject GameObject { get; set; }
        public World WorldInfo { get; private set; }
        public Vector2 Origin { get; protected set; }
        public AABB BoundingBox;
        public Shape CollisionShape;
        public virtual bool IsStatic { get { return true; } }
        public virtual bool IsSensor { get { return false; } }
        public bool IsProjectile { get; set; }
        public bool IsEthereal { get; set; }

        //Meta properties
        public List<CellValues> ContainingCells = new List<CellValues>();    // List of cells containing this physics object.

        //Material properties
        public float Mass;
        public float GroundFriction;
        public float AirFriction;

        public Vector2 Velocity;

        public delegate void CollisionCallback(PhysicsObject A, PhysicsObject B, Vector2 projectionVector);
        public CollisionCallback OnCollision;

        public Vector2 Position 
        { 
            get { return _position; } 
            set 
            { 
                _position = value;
                BoundingBox.Position = _position + BoundingBox.Offset;
            } 
        }
        protected Vector2 _position;

        public float Rotation;

        public bool TestAABBOnly { get; protected set; }

        public bool IsCollisionChecked = false;

        public PhysicsObject(World world, Vector2 position)
        {
            WorldInfo = world;
            _position = position;

            Velocity = Vector2.Zero;
        }

        /**
         * Remove this physics object form the worlds bag list
         */
        public virtual void Dispense()
        {
            WorldInfo.RemoveFromWorld(this);
        }


        public virtual void SetActive(bool bActive)
        {
            IsActive = bActive;
        }

        public virtual void Update(float deltaTime) 
        {
        }
        public virtual void Draw(SpriteBatch sb) 
        {
            if (CollisionShape != null)
            {
                CollisionShape.Draw(sb);
            }
        }

        public virtual void AddForce(Vector2 force) { }
        public virtual void ApplyImpulse(Vector2 impulse) { }

        public virtual void Collided(PhysicsObject collider, Edge collidingEdge, Vector2 projectionVector, float deltaTime) { }
    }
}
