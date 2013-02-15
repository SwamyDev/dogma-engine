using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.ScreenManagement;

namespace Pontification
{
    public class Primitives
    {
        private static Primitives _instance;
        public static Primitives Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = new Primitives();

                return _instance;
            }
        }

        // Drawing utilities.
        private Texture2D _pixel;

        public Primitives()
        {
            _pixel = new Texture2D(ScreenManager.Instance.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void DrawBox(SpriteBatch sb, Vector2 upperLeft, Vector2 lowerRight, Color color, int lineWidth = 0)
        {
            DrawBox(sb, GeometryFunctions.RectangleFromVectors(upperLeft, lowerRight), color, lineWidth);
        }

        public void DrawBox(SpriteBatch sb, Rectangle rect, Color color, int lineWidth = 0)
        {
            if (lineWidth <= 0)
            {
                sb.Draw(_pixel, rect, color);
            }
            else
            {
                drawBoxBorder(sb, rect, color, lineWidth);
            }
        }

        public void DrawLine(SpriteBatch sb, float x1, float y1, float x2, float y2, Color color, int lineWidth)
        {
            var diffVector = new Vector2(x2 - x1, y2 - y1);
            var rotation = (float)Math.Atan2(y2 - y1, x2 - x1);

            sb.Draw(_pixel, new Vector2(x1, y1), new Rectangle(1, 1, 1, lineWidth), color, rotation, new Vector2(0, lineWidth / 2), new Vector2(diffVector.Length(), 1), SpriteEffects.None, 0);
        }

        public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int lineWidth)
        {
            DrawLine(sb, start.X, start.Y, end.X, end.Y, color, lineWidth);
        }

        public void DrawPoint(SpriteBatch sb, float x, float y, Color color, int pointSize)
        {
            sb.Draw(_pixel, new Vector2(x - (pointSize / 2), y - (pointSize / 2)), new Rectangle(0, 0, pointSize, pointSize), color);
        }

        public void DrawPoint(SpriteBatch sb, Vector2 vertex, Color color, int pointSize)
        {
            DrawPoint(sb, vertex.X, vertex.Y, color, pointSize);
        }

        private void drawBoxBorder(SpriteBatch sb, Rectangle rect, Color color, int lineWidth)
        {
            DrawLine(sb, rect.Left, rect.Top, rect.Right, rect.Top, color, lineWidth);
            DrawLine(sb, rect.Right, rect.Y, rect.Right, rect.Bottom, color, lineWidth);
            DrawLine(sb, rect.Right, rect.Bottom, rect.Left, rect.Bottom, color, lineWidth);
            DrawLine(sb, rect.Left, rect.Bottom, rect.Left, rect.Top, color, lineWidth);
        }
    }
}
