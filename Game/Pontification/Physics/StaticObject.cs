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
    public class StaticObject : PhysicsObject
    {
        public StaticObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, float radius)
            : base(worldInfo, position)
        {
        }
        public StaticObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, Vector2 rectangle)
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

            WorldInfo.AddToWorld(this);
        }
        public StaticObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, List<Vector2> polygon)
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
            var idx = 0;
            polygon.ForEach((p) => { vertices[idx] = p - position; idx++; });

            BoundingBox = new AABB();
            BoundingBox.Offset = boxCenter - position;
            BoundingBox.Position = position + BoundingBox.Offset;
            BoundingBox.XHalfWidth = new Vector2((maxX - minX) / 2, 0);
            BoundingBox.YHalfWidth = new Vector2(0, (maxY - minY) / 2);

            CollisionShape = new Polygon(vertices, this);

            WorldInfo.AddToWorld(this);
        }
        public StaticObject(World worldInfo, Vector2 position, float mass, float friction, float restitution, Texture2D texture)
            : base(worldInfo, position)
        {
            var rectangle = ConvertUnits.ToSimUnits(new Vector2(texture.Width, texture.Height));

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

            WorldInfo.AddToWorld(this);
        }

        public override void AddForce(Vector2 force)
        {
        }
        public override void ApplyImpulse(Vector2 impulse)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            
            /*foreach (var cell in ContainingCells)
            {
                int x = (int)ConvertUnits.ToDisplayUnits(cell.Column * 3f + WorldInfo.Origin.X);
                int y = (int)ConvertUnits.ToDisplayUnits(cell.Row * 3f + WorldInfo.Origin.Y);
                Primitives.Instance.DrawBox(sb,
                    new Rectangle(x, y, (int)ConvertUnits.ToDisplayUnits(3f), (int)ConvertUnits.ToDisplayUnits(3f)),
                    new Color(100, 0, 0, 10));
            }*/
            
        }
    }
}
