#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PontAnimator
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public class Animation
    {
        public bool IsPaused { get; set; }
        /// <summary>
        /// The texture where the animations are stored
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        /// Get the current column of the animation
        /// </summary>
        public int FrameColumn { get; private set; }

        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }

        /// <summary>
        /// The frames of the current animation
        /// </summary>
        public int FrameCount { get; private set; }

        public float FrameTime { get; private set; }
        public bool bIsLooping { get; private set; }

        /// <summary>
        /// The columns of the animation texture
        /// </summary>
        public int Columns { get; private set; }

        public float CurrentTime { get { return time; } }

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float time;

        private bool _isFirstCall = true;
        private bool _callbackFired = false;

        static int amountOfDropsCalled = 0;

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2.0f, FrameHeight  / 2.0f); }
        }

        public delegate void AnimationCompleteCallback(int column);
        public AnimationCompleteCallback OnCompletition;

        public Animation(Texture2D texture, int frameWidth, int columns)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = texture.Height / columns;
            Columns = columns;
            FrameIndex = 0;
            time = 0;
        }

        public void Unload()
        {
            Texture.Dispose();
            Texture = null;
        }

        public void UpdateSettings(int frames, int framesPerSecond)
        {
            FrameWidth = Texture.Width / frames;
            FrameCount = frames;
            FrameTime = 1f / framesPerSecond;
            //time = 0;
        }

        public void Reset()
        {
            FrameIndex = 0;
            time = 0;
        }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(int column, int frameCount, float duration, bool bLooping)
        {
            // If this animation is already running, do not restart it.
            if (column == FrameColumn && FrameCount == frameCount)
                return;

            // Fire completion callbacks
            if (!_isFirstCall && !_callbackFired && OnCompletition != null)
            {
                OnCompletition(FrameColumn);
            }

            // Start the new animation.
            FrameColumn = column;
            FrameCount = frameCount;
            FrameTime = duration / frameCount;
            bIsLooping = bLooping;
            FrameIndex = 0;

            time = 0;

            _isFirstCall = false;
            _callbackFired = false;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, float alpha = 1f)
        {
            if (Texture == null)
                throw new NotSupportedException("No animation texture is currently loaded.");

            // Process passing time.
            if (!IsPaused)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > FrameTime)
                {
                    time -= FrameTime;

                    // Advance the frame index; looping or clamping as appropriate.
                    if (bIsLooping)
                    {
                        FrameIndex = (FrameIndex + 1) % FrameCount;
                    }
                    else
                    {
                        FrameIndex = Math.Min(FrameIndex + 1, FrameCount - 1);

                        // Fire callbacks
                        if (FrameIndex == (FrameCount - 1) && !_callbackFired && OnCompletition != null)
                        {
                            OnCompletition(FrameColumn);
                            _callbackFired = true;
                        }
                    }
                }
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * FrameWidth, FrameColumn * FrameHeight, FrameWidth, FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(Texture, position, source, new Color(alpha, alpha, alpha, alpha), 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
