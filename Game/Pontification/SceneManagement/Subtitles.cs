using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.SceneManagement
{
    public static class Subtitles
    {
        #region Private attributes
        private static SpriteFont _font;
        private static Rectangle _backgroundRect;
        private static Vector2[] _textPositions;
        private static Color _backgroundColor = new Color(0, 0, 100, 25);
        private static string[] _text;
        private static float _displayTime = 5f;
        private static float _fadeTime = 2.5f;
        private static float _timeDisplayed;
        private static float _transperencePercentage;
        private static int _horizontalSpacing = 10;
        private static int _paragraphSpacing = 18;
        private static int _pannelHeight = 80;
        private static bool _isShowing;
        #endregion

        #region Public methods
        public static void ShowDialog(string text)
        {
            _timeDisplayed = 0f;
            _transperencePercentage = 1f;

            Vector2 _textDimensions = _font.MeasureString(text);
            // Break up lines if text line to long for screen.
            int paragraphs = (int)Math.Ceiling(_textDimensions.X / (_backgroundRect.Width - 2 * _horizontalSpacing));
            _text = new string[paragraphs];

            int curParagraph = 0;
            int curWord = 0;
            string[] words = text.Split(' ');
            while (curParagraph < paragraphs)
            {
                StringBuilder newLineBuilder = new StringBuilder();
                string newLine = "";

                while (_font.MeasureString(newLineBuilder.ToString()).X < _backgroundRect.Width - 2 * _horizontalSpacing && curWord < words.Length)
                {
                    newLine = newLineBuilder.ToString();
                    newLineBuilder.Append(string.Format(" {0}", words[curWord]));
                    curWord++;

                    if (curWord == words.Length)
                    {
                        newLine = newLineBuilder.ToString();
                    }
                }

                _text[curParagraph] = newLine;
                curWord--;
                curParagraph++;
            }

            _textPositions = new Vector2[paragraphs];
            var center = new Vector2(_backgroundRect.X, _backgroundRect.Y) + (new Vector2(_backgroundRect.Width, _backgroundRect.Height) * 0.5f) - Vector2.UnitY * (_paragraphSpacing / 2) * (paragraphs - 1);

            for (int i = 0; i < paragraphs; i++)
            {
                _textPositions[i] = center - _font.MeasureString(_text[i]) * 0.5f + Vector2.UnitY * _paragraphSpacing * i;
            }

            _isShowing = true;
        }
        
        public static void LoadContent(ContentManager cm)
        {
            _font = cm.Load<SpriteFont>("Monitoring/UbuntuMonoR");

            var graphics = ScreenManagement.ScreenManager.Instance.GraphicsDevice;
            _backgroundRect = new Rectangle(0, graphics.Viewport.Height - _pannelHeight, graphics.Viewport.Width, _pannelHeight);
        }

        public static void Update(GameTime gameTime)
        {
            if (_isShowing)
            {
                _timeDisplayed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                float timeToDisplay = _displayTime - _timeDisplayed;

                if (timeToDisplay <= _fadeTime)
                {
                    _transperencePercentage = timeToDisplay / _fadeTime;
                }

                if (_timeDisplayed >= _displayTime)
                    _isShowing = false;
            }
        }

        public static void Draw(SpriteBatch sb)
        {
            sb.Begin();
            if (_isShowing)
            {
                Primitives.Instance.DrawBox(sb, _backgroundRect, _backgroundColor * _transperencePercentage);
                for (int i = 0; i < _text.Length; i++)
                {
                    sb.DrawString(_font, _text[i], _textPositions[i], Color.White * _transperencePercentage);
                }
            }
            sb.End();
        }
        #endregion
    }
}
