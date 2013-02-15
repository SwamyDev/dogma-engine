using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Physics
{
    public class Line
    {
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }
        public Vector2 Segment { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Length { get; private set; }
        public Vector2 Normal { get; private set; } //The normal vector of the edge.

        // Hesse normal form
        public float A { get; private set; }
        public float B { get; private set; }
        public float C { get; private set; }

        public Line(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;

            Vector2 diff = p2 - p1;
            Length = diff.Length();
            Segment = diff;
            Direction = Vector2.Normalize(diff);
            Normal = Vector2.Normalize(new Vector2(diff.Y, -diff.X));

            A = p2.Y - p1.Y;
            B = p1.X - p2.X;
            C = A * p1.X + B * p1.Y;
        }

        public void SetLine(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;

            Vector2 diff = p1 - p2;
            Length = diff.Length();
            Segment = diff;
            Normal = Vector2.Normalize(new Vector2(-diff.Y, diff.X));

            A = p2.Y - p1.Y;
            B = p1.X - p2.X;
            C = A * p1.X + B * p1.Y;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 pxP1 = ConvertUnits.ToDisplayUnits(P1);
            Vector2 pxP2 = ConvertUnits.ToDisplayUnits(P2);

            // Draw line.
            Primitives.Instance.DrawLine(sb, pxP1, pxP2, Color.Yellow, 1);

            // Draw normal
            Vector2 middle = ConvertUnits.ToDisplayUnits(P1 + ((P2 - P1) * (Length / 2f)));
            Primitives.Instance.DrawLine(sb, middle, middle + ConvertUnits.ToDisplayUnits(Normal) * 20, Color.Blue, 1);
        }
    }
}
