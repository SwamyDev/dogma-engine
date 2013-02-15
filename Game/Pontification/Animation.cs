#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification
{
    public struct AnimationData
    {
        public string Name;
        public float Duration;
        public int Priority;
        public int Frames;
        public int Column;
        public bool IsLooping;

        public AnimationData(string name, float duration, int priority, int frames, int column, bool isLooping)
        {
            this.Name = name;
            this.Duration = duration;
            this.Priority = priority;
            this.Frames = frames;
            this.Column = column;
            this.IsLooping = isLooping;
        }
    }

    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public class Animation
    {
        #region Private attributes
        private static int amountOfDropsCalled = 0;

        private AnimationData _currentAnimation = new AnimationData("None", 1, int.MinValue, 1, 0, false);
        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float time;

        private bool _isFirstCall = true;
        private bool _callbackFired;
        private bool _isStopped;
        #endregion

        #region Public properties
        public Dictionary<string, AnimationData> AnimationDictionary { get; set; }
        public AnimationData CurrentAnimation { get { return _currentAnimation; } }
        /// <summary>
        /// The texture where the animations are stored
        /// </summary>
        public Texture2D Texture { get; private set; }

        public Vector2 Scale = Vector2.One;
        public float Rotation;

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

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(FrameWidth / 2.0f, FrameHeight  / 2.0f); }
        }

        // Define event
        public event EventHandler<AnimationEventArgs> AnimationFinished;
        #endregion

        public Animation(Texture2D texture, int frameWidth, int columns)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = texture.Height / columns;
            Columns = columns;
            FrameIndex = 0;
            time = 0;
            AnimationDictionary = new Dictionary<string, AnimationData>();
            AnimationDictionary.Add("None", new AnimationData("None", 1, int.MinValue, 1, 0, false));

            PlayAnimation("None");
        }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(string name)
        {
            if (AnimationDictionary.ContainsKey(name) == false)
                return;

            AnimationData animData = AnimationDictionary[name];

            // If this animation is already running, do not restart it.
            if (animData.Column == FrameColumn && FrameCount == animData.Frames)
                return;

            if (animData.Priority < CurrentAnimation.Priority)
                return;

            // Fire completion callbacks
            stoppingAnimation();

            // Start the new animation.
            _currentAnimation = animData;
            FrameColumn = animData.Column;
            FrameCount = animData.Frames;
            FrameTime = animData.Duration / FrameCount;
            bIsLooping = animData.IsLooping;
            FrameIndex = 0;

            time = 0;

            _isFirstCall = false;
            _callbackFired = false;
            _isStopped = false;
        }

        public void StopAnimation()
        {
            _isStopped = true;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, float alpha = 1f)
        {
            if (Texture == null)
                throw new NotSupportedException("No animation texture is currently loaded.");

            // Process passing time.
            if (gameTime != null && !_isStopped)
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isStopped)
                stoppingAnimation();

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
                    if (FrameIndex == (FrameCount - 1))
                    {
                        stoppingAnimation();
                    }
                }
            }

            // Calculate the source rectangle of the current frame.
            //FrameWidth = (int)(FrameWidth * Scale.X);
            //FrameHeight = (int)(FrameHeight * Scale.Y);
            Rectangle source = new Rectangle(FrameIndex * FrameWidth, FrameColumn * FrameHeight, FrameWidth, FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(Texture, position, source, new Color(alpha, alpha, alpha, alpha), Rotation, Origin, Scale, spriteEffects, 0.0f);
        }

        private void stoppingAnimation()
        {
            _currentAnimation.Priority = int.MinValue;

            if (!_callbackFired)
            {
                _callbackFired = true;
                EventHandler<AnimationEventArgs> animFinished = AnimationFinished;

                if (animFinished != null)
                    animFinished(this, new AnimationEventArgs(_currentAnimation));
            }
        }
    }

    #region Animation event class
    public class AnimationEventArgs : EventArgs
    {
        public AnimationData AnimData { get; private set; }

        public AnimationEventArgs(AnimationData animData)
        {
            AnimData = animData;
        }
    }
    #endregion
}
