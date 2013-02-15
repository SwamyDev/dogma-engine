using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification
{
    static class GeometryFunctions
    {
        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }

        public static Point ToPoint(this Vector2 v)
        {
            return new Point((int)Math.Round(v.X), (int)Math.Round(v.Y));
        }

        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static float DistanceTo(this Vector2 v0, Vector2 v)
        {
            return (v - v0).Length();
        }

        public static float DistanceToLineSegment(this Vector2 v, Vector2 a, Vector2 b)
        {
            Vector2 x = b - a;
            x.Normalize();
            float t = Vector2.Dot(x, v - a);
            if (t < 0) return (a - v).Length();
            float d = (b - a).Length();
            if (t > d) return (b - v).Length();
            return (a + x * t - v).Length();

        }

        public static Rectangle Transform(this Rectangle r, Matrix m)
        {
            Vector2[] poly = new Vector2[2];
            poly[0] = new Vector2(r.Left, r.Top);
            poly[1] = new Vector2(r.Right, r.Bottom);
            Vector2[] newpoly = new Vector2[2];
            Vector2.Transform(poly, ref m, newpoly);

            Rectangle result = new Rectangle();
            result.Location = newpoly[0].ToPoint();
            result.Width = (int)(newpoly[1].X - newpoly[0].X);
            result.Height = (int)(newpoly[1].Y - newpoly[0].Y);
            return result;
        }

        public static Vector2[] ToPolygon(this Rectangle r)
        {
            Vector2[] poly = new Vector2[4];
            poly[0] = new Vector2(r.Left, r.Top);
            poly[1] = new Vector2(r.Right, r.Top);
            poly[2] = new Vector2(r.Right, r.Bottom);
            poly[3] = new Vector2(r.Left, r.Bottom);
            return poly;
        }

        public static Rectangle RectangleFromVectors(Vector2 v1, Vector2 v2)
        {
            Vector2 distance = v2 - v1;
            Rectangle result = new Rectangle();
            result.X = (int)Math.Min(v1.X, v2.X);
            result.Y = (int)Math.Min(v1.Y, v2.Y);
            result.Width = (int)Math.Abs(distance.X);
            result.Height = (int)Math.Abs(distance.Y);
            return result;
        }

        public static Vector2 GetCentroid(List<Vector2> vertices)
        {
            float area = GetSignedArea(vertices);

            area = Math.Abs(area);

            float cx = 0;
            float cy = 0;

            /// USING INDEXED ACCESS IS NOT A GOOD IDEA WITH LIST ITEM -> IMPROVE THAT BY USING LINQ OR DIFFERENT DATASTRUCTURE
            for (int i = 0; i < vertices.Count; i++)
            {
                int ii = (i + 1) % vertices.Count;
                cx += (vertices[i].X + vertices[ii].X) * (vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y);
            }
            cx = (1 / (6 * area)) * cx;
            for (int i = 0; i < vertices.Count; i++)
            {
                int ii = (i + 1) % vertices.Count;
                cy += (vertices[i].Y + vertices[ii].Y) * (vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y);
            }
            cy = (1 / (6 * area)) * cy;

            return new Vector2(cx, cy);
        }

        public static float GetSignedArea(List<Vector2> vertices)
        {
            float area = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                int ii = (i + 1) % vertices.Count;
                area += vertices[i].X * vertices[ii].Y - vertices[ii].X * vertices[i].Y;
            }
            area *= 0.5f;


            return area;
        }
    }
}
