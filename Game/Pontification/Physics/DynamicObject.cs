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
    public class DynamicObject : PhysicsObject
    {
        public enum MotionStates
        {
            MS_MIDAIR,
            MS_LANDED
        }

        private Vector2 _force;

        public Vector2 StandOnSurface       // Returns the surface normal of the surface this object is currently standing on
        {
            get 
            {
                if (MotionState == MotionStates.MS_LANDED)
                    return _surfaceNormal;
                else
                    return Vector2.Zero;
            }
        }

        public float Restitution = 0.3f;
        public float Friction = 1f;

        public bool IgnoreGravity;
        public bool bSleeps;

        public override bool IsStatic { get { return false; } }

        public MotionStates MotionState;

        private float _restLamda = 0.001f;

        private Vector2 _oldPosition = Vector2.Zero;

        protected Vector2 _bounceVector;
        protected Vector2 _frictionVector;
        protected Vector2 _reflectionVector;
        protected Vector2 _projectionVector;
        protected Vector2 _surfaceNormal;

        protected float _airDrag = 0.8f;
        protected float _groundDrag = 0.65f;

        public DynamicObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, float radius)
            : base(worldInfo, position)
        {
        }
        public DynamicObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, Vector2 rectangle)
            : base(worldInfo, position)
        {
            BoundingBox = new AABB();
            BoundingBox.Offset = Vector2.Zero;
            BoundingBox.Position = position;
            BoundingBox.XHalfWidth = new Vector2(rectangle.X / 2, 0);
            BoundingBox.YHalfWidth = new Vector2(0, rectangle.Y / 2);

            var polygon = new Vector2[4];
            var upperLeft = Vector2.Zero;
            var upperRight = new Vector2(rectangle.X, 0);
            var lowerRight = rectangle;
            var lowerLeft = new Vector2(0, rectangle.Y);

            polygon[0] = upperLeft;
            polygon[1] = upperRight;
            polygon[2] = lowerRight;
            polygon[3] = lowerLeft;

            CollisionShape = new Polygon(polygon, this);
            TestAABBOnly = true;

            Mass = mass;
            Friction = friction;
            Restitution = restitution;

            if (Friction > 1)
                Friction = 1;
            if (Mass <= 0)
                Mass = 1;

            WorldInfo.AddToWorld(this);
        }

        public DynamicObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, List<Vector2> polygon)
            : base(worldInfo, position)
        {
            float minX = float.PositiveInfinity; float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity; float maxY = float.NegativeInfinity;

            polygon.ForEach((vertex) =>
            {
                if (vertex.X < minX)
                    minX = vertex.X;
                if (vertex.X > maxX)
                    maxX = vertex.X;
                if (vertex.Y < minY)
                    minY = vertex.Y;
                if (vertex.Y > maxY)
                    maxY = vertex.Y;
            });

            var ul = new Vector2(minX, minY);
            var lr = new Vector2(maxX, maxY);

            Vector2 boxCenter = ul + (lr - ul) * 0.5f;

            // Convert to local points
            var vertices = new Vector2[polygon.Count];
            int idx = 0;
            polygon.ForEach((p) => { vertices[idx] = (p - position); idx++; });

            BoundingBox = new AABB();
            BoundingBox.Offset = boxCenter - position;
            BoundingBox.Position = position + BoundingBox.Offset;
            BoundingBox.XHalfWidth = new Vector2((maxX - minX) / 2, 0);
            BoundingBox.YHalfWidth = new Vector2(0, (maxY - minY) / 2);

            CollisionShape = new Polygon(vertices, this);

            Mass = mass;
            Friction = friction;
            Restitution = restitution;

            if (Friction > 1)
                Friction = 1;
            if (Mass <= 0)
                Mass = 1;

            WorldInfo.AddToWorld(this);

        }
        public DynamicObject(World worldInfo, Vector2 positon, float mass, float friction, float restitution, Texture2D texture)
            : base(worldInfo, positon)
        {
        }

        public override void AddForce(Vector2 force)
        {
            _force = force;
        } 

        public override void ApplyImpulse(Vector2 impulse)
        {
        }

        public override void Update(float deltaTime)
        {
            if (!IsActive)
                return;

            Velocity += (_force / Mass) * deltaTime;
            _force = Vector2.Zero;

            // Update Position.
            if (Math.Abs(Velocity.X) < _restLamda)
                Velocity.X = 0;
            if (Math.Abs(Velocity.Y) < _restLamda)
                Velocity.Y = 0;

            Position += Velocity * deltaTime;

            if (Math.Abs((_oldPosition - Position).Length()) < _restLamda)
            {
                bSleeps = true;
            }
            else
            {
                bSleeps = false;
            }
            BoundingBox.Position = _position;
            _oldPosition = Position;
            
            // Apply gravity.
            if (MotionState != MotionStates.MS_LANDED && IgnoreGravity == false)
                Velocity += WorldInfo.Gravity * deltaTime;
            
            if (MotionState == MotionStates.MS_LANDED)
            {
                /*if (Velocity.Y > 0)
                    Velocity.Y = 0;*/

                //Velocity.X *= _groundDrag;
            }
            if (MotionState == MotionStates.MS_MIDAIR)
            {
                //Velocity.X *= _airDrag;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            Vector2 p1 = ConvertUnits.ToDisplayUnits(_position);

            Primitives.Instance.DrawLine(sb, p1, p1 + ConvertUnits.ToDisplayUnits(_projectionVector) * 10, Color.BlueViolet, 1);
            Primitives.Instance.DrawLine(sb, p1, p1 + ConvertUnits.ToDisplayUnits(Velocity), Color.DarkCyan, 1);

            /*
            if (GameObject == null || GameObject.Name != "Player")
                return;

            foreach (var cell in ContainingCells)
            {
                int x = (int)ConvertUnits.ToDisplayUnits(cell.Column * 3f + WorldInfo.Origin.X);
                int y = (int)ConvertUnits.ToDisplayUnits(cell.Row * 3f + WorldInfo.Origin.Y);
                Primitives.Instance.DrawBox(sb,
                    new Rectangle(x, y, (int)ConvertUnits.ToDisplayUnits(3f), (int)ConvertUnits.ToDisplayUnits(3f)),
                    new Color(0, 100, 0, 10));
            }*/

            /*float angleBounce = (float)Math.Atan2(_bounceVector.Y, _bounceVector.X);
            sb.Draw(_bounceTexture, new Rectangle((int)p1.X, (int)p1.Y, (int)ConvertUnits.ToDisplayUnits(_bounceVector.Length()), 1),
                null, Color.White, angleBounce, Vector2.Zero, SpriteEffects.None, 0f);

            float angleFrict = (float)Math.Atan2(_frictionVector.Y, _frictionVector.X);
            sb.Draw(_frictionTexture, new Rectangle((int)p1.X, (int)p1.Y, (int)ConvertUnits.ToDisplayUnits(_frictionVector.Length()), 1),
                null, Color.White, angleFrict, Vector2.Zero, SpriteEffects.None, 0f);*/
        }

        public override void Collided(PhysicsObject collider, Edge collidingEdge, Vector2 projectionVector, float deltaTime)
        {
            if (!IsActive)
                return;

            if (collider is CharacterObject)
                return;

            if (IsEthereal || collider.IsEthereal)
                return;

            _projectionVector = projectionVector;
            // After collision project out of colliding surface.

            Position -= projectionVector;

            if (projectionVector.Length() == 0.0f)  // Return when no projection.
                return;
            // Split reflection velocity into friction and bounce vector.
            var surfaceNorm = Vector2.Normalize(projectionVector);
            var surfaceDir = new Vector2(surfaceNorm.Y, -surfaceNorm.X);

            _surfaceNormal = surfaceNorm;

            Vector2 bounce = surfaceNorm * Vector2.Dot(Velocity, surfaceNorm) * Restitution * -1;
            Vector2 friction = surfaceDir * Vector2.Dot(Velocity, surfaceDir) * (1 - Friction);

            if (Vector2.Dot(surfaceNorm, new Vector2(0, 1)) > 0.3f && bounce.Length() < 0.5f)
            {
                MotionState = MotionStates.MS_LANDED;
                Velocity = friction;
            }
            else
            {
                _bounceVector = bounce;
                _frictionVector = friction;

                // Add up to reflection vector.
                _reflectionVector = bounce + friction;
                Velocity = _reflectionVector;
            }
        }
    }
}
