using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PontAnimator
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
            _pixel = new Texture2D(Animator.Instance.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void DrawRectangle(SpriteBatch sb, float x1, float y1, float x2, float y2, Color color)
        {
            sb.Draw(_pixel, new Vector2(x1, y1), new Rectangle(0, 0, (int)x2 - (int)x1, (int)y2 - (int)y1), color);
        }

        public void DrawRectangle(SpriteBatch sb, Vector2 topLeft, Vector2 bottomRight, Color color)
        {
            DrawRectangle(sb, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, color);
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
            sb.Draw(_pixel, new Vector2(x - pointSize / 2, y - pointSize / 2), new Rectangle(0, 0, pointSize, pointSize), color);
        }

        public void DrawPoint(SpriteBatch sb, Vector2 vertex, Color color, int pointSize)
        {
            DrawPoint(sb, vertex.X, vertex.Y, color, pointSize);
        }
    }
}
