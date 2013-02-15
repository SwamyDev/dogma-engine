using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class ScaleToObject : Component
    {
        #region Private attributes
        private TextureComponent _texture;
        private Vector2 _oldPos;
        private float _height;
        private float _width;
        private float _currentX = 1f;
        private float _currentY = 1f;
        #endregion

        #region Public properties
        public GameObject Master { get; set; }
        public float Offset { get; set; }
        public bool UseYAxis { get; set; }
        #endregion

        #region Public methods
        
        public override void Start()
        {
            _texture = GetComponent<TextureComponent>();
            if (_texture == null)
                throw new ArgumentNullException("ScaleComponent needs a TextureComponent attached to it");

            _height = _texture.Texture.Height;
            _width = _texture.Texture.Width;

            _oldPos = Master.Position;
        }

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (UseYAxis)
            {
                var diff = _oldPos.Y - Master.Position.Y;
                _currentY -= (diff / _height);
            }
            else
            {
                var diff = _oldPos.X - Master.Position.X;
                _currentX -= (diff / _width);
            }

            _texture.Scale = new Vector2(_currentX, _currentY);

            _oldPos = Master.Position;
        }
        #endregion
    }
}
