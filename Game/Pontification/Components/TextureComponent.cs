using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Components
{
    /// <summary>
    /// Adds a static texture to a game object which will be displayed at the game objects position,
    /// with the specified offset.
    /// </summary>
    class TextureComponent : Component
    {
        public Texture2D Texture { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Scale { get; set; }

        private Vector2 _textureCenter;

        public TextureComponent()
            : base()
        {
            Scale = Vector2.One;
        }

        public override void Start()
        {
            _textureCenter = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Draw(Texture, GameObject.Position, null, Color.White, GameObject.Rotation, _textureCenter - Offset, Scale, SpriteEffects.None, 0f);
        }
    }
}
