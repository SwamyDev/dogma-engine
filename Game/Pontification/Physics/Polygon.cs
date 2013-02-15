using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Physics
{
    public class Polygon : Shape
    {
        public Edge[] Edges;
        public Vector2 Centroid;

        private Vector2[] _vertices;

        public Polygon(Vector2[] vertices, PhysicsObject owner)
            : base(owner)
        {
            Type = ShapeType.SH_POLYGON;

            Centroid = GetCentroid(vertices);
            Edges = new Edge[vertices.Length];
            _vertices = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i] - Centroid;

                if (i == 0)
                {
                    // First vertex.
                    Vector2 prevVertex = vertices[vertices.Length - 1] - Centroid;
                    Edges[i] = new Edge(prevVertex, vertex, this);
                    _vertices[i] = vertex;
                }
                else
                {
                    Vector2 prevVertex = _vertices[i - 1];
                    Edges[i] = new Edge(prevVertex, vertex, this);
                    _vertices[i] = vertex;
                }
            }
        }

        private Vector2 GetCentroid(Vector2[] vertices)
        {
            float area = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                int ii = (i + 1) % vertices.Length;
                area += vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y;
            }
            area *= 0.5f;

            area = Math.Abs(area);

            float cx = 0;
            float cy = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                int ii = (i + 1) % vertices.Length;
                cx += (vertices[i].X + vertices[ii].X) * (vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y);
            }
            cx = (1 / (6 * area)) * cx;
            for (int i = 0; i < vertices.Length; i++)
            {
                int ii = (i + 1) % vertices.Length;
                cy += (vertices[i].Y + vertices[ii].Y) * (vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y);
            }
            cy = (1 / (6 * area)) * cy;

            return new Vector2(cx, cy);
        }

        public void GetVertices(out Vector2[] vertices)
        {
            vertices = new Vector2[_vertices.Length];
            _vertices.CopyTo(vertices, 0);
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (Vector2 vertex in _vertices)
            {
                var drawPos = ConvertUnits.ToDisplayUnits(vertex + Owner.Position);
                Primitives.Instance.DrawPoint(sb, drawPos, Color.Blue, 4);
            }
            foreach (Edge edge in Edges)
            {
                edge.Draw(sb);
            }

            AABB box = Owner.BoundingBox;
            Vector2 ul = ConvertUnits.ToDisplayUnits(box.Position - box.XHalfWidth - box.YHalfWidth);
            Vector2 rect = ConvertUnits.ToDisplayUnits(box.XHalfWidth * 2f + box.YHalfWidth * 2f);


            Primitives.Instance.DrawBox(sb, new Rectangle((int)ul.X, (int)ul.Y, (int)rect.X, (int)rect.Y), new Color(0.5f, 0.5f, 0.5f, 0.5f));
            Primitives.Instance.DrawPoint(sb, ConvertUnits.ToDisplayUnits(Owner.Position), Color.Blue, 4);
        }
    }


    public class Edge
    {
        public Polygon Owner;
        public Vector2 P1 
        { 
            get 
            {
                if (Owner.Owner != null)
                    return _p1 + Owner.Owner.Position;
                else
                    return _p1 + Owner.Centroid;
            } 
        }
        public Vector2 P2 
        { 
            get
            {
                if (Owner.Owner != null)
                    return _p2 + Owner.Owner.Position;
                else
                    return _p2 + Owner.Centroid;
            } 
        }
        public Vector2 Segment { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Length { get; private set; }
        public Vector2 Normal { get; private set; } //The normal vector of the edge.

        // Hesse normal form.
        public float A { get { return P2.Y - P1.Y; } }
        public float B { get { return P1.X - P2.X; } }
        public float C { get { return A * P1.X + B * P1.Y; } }

        private Vector2 _p1;
        private Vector2 _p2;

        public Edge(Vector2 p1, Vector2 p2, Polygon owner)
        {
            Owner = owner;

            _p1 = p1;
            _p2 = p2;

            Vector2 diff = p2 - p1;
            Length = diff.Length();
            Segment = diff;
            Direction = Vector2.Normalize(diff);
            Normal = Vector2.Normalize(new Vector2(diff.Y, -diff.X));
        }

        public void SetEdge(Vector2 p1, Vector2 p2)
        {
            _p1 = p1;
            _p2 = p2;

            Vector2 diff = p1 - p2;
            Length = diff.Length();
            Normal = Vector2.Normalize(new Vector2(-diff.Y, diff.X));
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 pxP1 = ConvertUnits.ToDisplayUnits(P1);
            Vector2 pxP2 = ConvertUnits.ToDisplayUnits(P2);

            // Draw line.
            Primitives.Instance.DrawLine(sb, pxP1, pxP2, Color.Yellow, 1);

            // Draw normal
            Vector2 direction = Vector2.Normalize(P2 - P1);
            Vector2 middle = ConvertUnits.ToDisplayUnits(P1 + (direction * (Length / 2f)));
            Primitives.Instance.DrawLine(sb, middle, middle + ConvertUnits.ToDisplayUnits(Normal) * 0.2f, Color.Blue, 1);
        }
    }
}
