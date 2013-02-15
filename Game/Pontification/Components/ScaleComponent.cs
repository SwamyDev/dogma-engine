using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    class ScaleComponent : Component
    {
        #region Private properties
        private TextureComponent _texture;
        private AnimationComponent _animation;
        private float _height;
        private float _width;
        private float _currentX;
        private float _currentY;
        #endregion

        #region Public properties
        public float XMin { get; set; }
        public float XMax { get; set; }
        public float YMin { get; set; }
        public float YMax { get; set; }
        public float XSpeed { get; set; }
        public float YSpeed { get; set; }
        public float XStart { get; set; }
        public float YStart { get; set; }
        public bool Looping { get; set; }
        #endregion

        #region Public methods
        public ScaleComponent()
        {
            XMin = 1.0f;
            XMax = 1.0f;
            YMin = 1.0f;
            YMax = 1.0f;
        }

        public override void Start()
        {
            _texture = GetComponent<TextureComponent>();
            _animation = GetComponent<AnimationComponent>();

            if (_texture != null)
            {
                _height = _texture.Texture.Height;
                _width = _texture.Texture.Width;
            }
            else if (_animation != null)
            {
                _height = _animation.Sprite.FrameHeight;
                _width = _animation.Sprite.FrameWidth;
            }

            _currentX = XStart;
            _currentY = YStart;
        }
        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentX += XSpeed * dt;
            _currentY += YSpeed * dt;

            // Clamp.
            _currentX = MathHelper.Clamp(_currentX, XMin, XMax);
            _currentY = MathHelper.Clamp(_currentY, YMin, YMax);

            // Apply to texture.
            if (_texture != null)
                _texture.Scale = new Vector2(_currentX, _currentY);
            if (_animation != null)
                _animation.Sprite.Scale = new Vector2(_currentX, _currentY);

            // Loop.
            if (Looping)
            {
                if (_currentX <= XMin || _currentX >= XMax)
                    XSpeed *= -1;
                if (_currentY <= YMin || _currentY >= YMax)
                    YSpeed *= -1;
            }
        }
        #endregion
    }
}
