using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.ScreenManagement
{
    public class GameScreen
    {
        #region Private attrobutes
        private IScreenContent _screenContent;
        private bool _isNotInFocus;
        #endregion

        #region Internal properties
        internal ScreenState ScreenState { get; private set; }
        internal TimeSpan TransitionOffDuration { get; private set; }
        internal TimeSpan TransitionOnDuration { get; private set; }
        internal bool IsPopup { get; private set; }
        internal bool IsTile { get; private set; }

        internal ScreenManager ScreenManager { get; set; }
        #endregion

        #region Public properties
        // Use these attributes to controle transition effects like fades and swipes.
        public float TransitionPosition { get; private set; }
        public float TransitionAlpha { get { return 1f - TransitionPosition; } }

        public bool IsExiting { get; internal set; }

        public bool IsActive
        {
            get { return !_isNotInFocus && (ScreenState == ScreenState.TRANSITION_ON || ScreenState == ScreenState.ACTIVE); }
        }
        #endregion

        // Construtors.
        public GameScreen(IScreenContent content, bool isPopup = false, bool isTile = false)
            : this (content, TimeSpan.Zero, TimeSpan.Zero, isPopup, isTile)
        {
        }

        public GameScreen(IScreenContent content, TimeSpan transitionOnDuration, TimeSpan transitionOffDuration, bool isPopup = false, bool isTile = false)
        {
            _screenContent = content;
            _screenContent.CurrentScreen = this;    // Link back.

            ScreenState = ScreenState.TRANSITION_ON;
            TransitionPosition = 1f;
            TransitionOnDuration = transitionOnDuration;
            TransitionOffDuration = transitionOffDuration;
            IsPopup = isPopup;
        }

        #region Public methods
        // Exit the current screen.
        public void ExitScreen()
        {
            if (TransitionOffDuration == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                IsExiting = true;
            }
        }
        #endregion

        #region Internal methods
        internal void LoadContent()
        {
            // Pass along to the content class
            _screenContent.LoadContent();
        }

        internal void UnloadContent()
        {
            // Pass along to the content class
            _screenContent.UnloadContent();
        }

        // Updates screen transitons and activations.
        internal void Update(GameTime gameTime, bool isFocused, bool isCoveredByScreen)
        {
            _isNotInFocus = isCoveredByScreen;

            if (IsExiting)
            {
                ScreenState = ScreenState.TRANSITION_OFF;

                if (UpdateTransition(gameTime, TransitionOffDuration, 1) == false)
                {
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (_isNotInFocus)
            {
                if (UpdateTransition(gameTime, TransitionOffDuration, 1))
                {
                    ScreenState = ScreenState.TRANSITION_OFF;
                }
                else
                {
                    ScreenState = ScreenState.HIDDEN;
                }
            }
            else
            {
                if (UpdateTransition(gameTime, TransitionOnDuration, -1))
                {
                    ScreenState = ScreenState.TRANSITION_ON;
                }
                else
                {
                    ScreenState = ScreenState.ACTIVE;

                    // Update content if current screen is active.
                    _screenContent.Update(gameTime);
                }
            }
        }

        internal void HandleInput(GameTime gameTime, InputState input)
        {
            // Pass along to the content class
            _screenContent.HandleInput(gameTime, input);
        }

        internal void Draw(SpriteBatch sb, GameTime gameTime)
        {
            // Pass along to the content class
            _screenContent.Draw(sb, gameTime);
        }
        #endregion

        #region Private methods
        // Handles screen transitions and updates transition status and regarded public attributes.
        private bool UpdateTransition(GameTime gameTime, TimeSpan duration, int direction)
        {
            float transDelta;

            if (duration == TimeSpan.Zero)
                transDelta = 1;
            else
                transDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / duration.TotalMilliseconds);

            // Update transition position.
            TransitionPosition += transDelta * direction;

            // Clamp if we reached the end of one of the directions and return false as we finished transition.
            if (((direction < 0) && (TransitionPosition <= 0)) || ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0f, 1f);
                return false;
            }

            // We are transitioning!
            return true;
        }
        #endregion
    }
}
