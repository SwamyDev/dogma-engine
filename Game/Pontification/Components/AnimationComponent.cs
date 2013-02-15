using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Physics;

namespace Pontification.Components
{
    class AnimationComponent : Component
    {
        private CharacterPhysicsComponent _charPhysics;
        private SpiritPhysicsComponent _spiritPhysics;

        #region Public properties.
        public Animation Sprite { get; set; }
        public string PlayingAnimation { get; set; }
        public float Alpha { get; set; }
        public int Facing { get; set; }
        #endregion

        public AnimationComponent()
            : base()
        {
            Alpha = 1f;
            PlayingAnimation = "None";
        }

        public override void Start()
        {
            _charPhysics = GetComponent<CharacterPhysicsComponent>();
            _spiritPhysics = GetComponent<SpiritPhysicsComponent>();

            if (PlayingAnimation != "None" || PlayingAnimation != "")
                SetAnimation(PlayingAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            if (_charPhysics != null)
                Facing = _charPhysics.Facing;
            if (_spiritPhysics != null)
                Facing = _spiritPhysics.Facing;
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            SpriteEffects flip = SpriteEffects.None;
            if (Facing > 0)
                flip = SpriteEffects.FlipHorizontally;

            Sprite.Draw(gameTime, sb, GameObject.Position, flip, Alpha);
        }

        /// <summary>
        /// Set the animation with the specified name.
        /// </summary>
        /// <param name="name">Animation name</param>
        public void SetAnimation(string name)
        {
            Sprite.PlayAnimation(name);
        }

        /// <summary>
        /// Stops animation currentyl playing.
        /// </summary>
        public void StopAnimation()
        {
            Sprite.StopAnimation();
        }
    }
}
