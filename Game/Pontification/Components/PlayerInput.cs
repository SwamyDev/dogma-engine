using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pontification.ScreenManagement;

namespace Pontification.Components
{
    public class PlayerInput : Component
    {
        #region Public attributes
        public bool IsInteracting;
        #endregion

        #region Private attributes
        private float _stepCooldown = 0.1f;
        private float _timeSinceStep;
        private float _timeWalkPressed;
        private bool _stepBlocked;
        private bool _inputLocked;
        private bool _movementLocked;
        private bool _blockPrimary;
        #endregion

        // Delta time needs to be passed to invoke a step instead of walk
        public float StepDeltaTime { get; set; }

        public PlayerInput()
        {
        }

        public override void Start()
        {
            StepDeltaTime = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            // The game object with this input class attached is the player.
            Pontification.SceneManagement.SceneInfo.Player = GameObject;

            if (_inputLocked)
            {
                SendMessage("Walk", new object[] { 0 });
                return;
            }

            if (!_movementLocked)
            {
                int direction = 0;
                if (InputState.IsDown("WalkLeft"))    // Move left
                {
                    direction = -1;
                    if (!_stepBlocked)
                        _timeWalkPressed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (InputState.IsDown("WalkRight"))    // Move right
                {
                    direction = 1;
                    if (!_stepBlocked)
                        _timeWalkPressed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (InputState.IsNewUp("WalkLeft") || InputState.IsNewUp("WalkRight"))
                {
                    if ((_timeWalkPressed > 0 && _timeWalkPressed <= StepDeltaTime) && !_stepBlocked)
                    {
                        SendMessage("Step", new object[] { direction });
                        _timeSinceStep = 0.0f;
                        _stepBlocked = true;
                    }
                    _timeWalkPressed = 0.0f;
                }

                if (_stepBlocked)
                    _timeSinceStep += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_timeSinceStep > _stepCooldown)
                    _stepBlocked = false;

                if (!_stepBlocked)
                    SendMessage("Walk", new object[] { direction });

                if (InputState.IsNewDown("Jump"))    // Jump
                    SendMessage("Jump");
            }
            else
            {
                SendMessage("Walk", new object[] { 0 });
            }

            SendMessage("ResetAbilityState");
            UnlockMovement();
            if (_blockPrimary == false)
            {
                if (InputState.IsDown("Primary"))
                    SendMessage("UsePrimary");
            }
            if (InputState.IsDown("Secondary"))
                SendMessage("UseSecondary");

            if (InputState.IsNewUp("Primary"))
            {
                SendMessage("ResetPrimaryTimer");
                _blockPrimary = false;
            }
            if (InputState.IsNewUp("Secondary"))
                SendMessage("ResetSecondaryTimer");

            IsInteracting = InputState.IsNewDown("Use");
        }

        public void LockInput()
        {
            _inputLocked = true;
            SendMessage("Walk", new object[] { 0 });
        }

        public void UnlockInput()
        {
            _inputLocked = false;
        }

        public void LockMovement()
        {
            _movementLocked = true;
            SendMessage("Walk", new object[] { 0 });
        }

        public void UnlockMovement()
        {
            _movementLocked = false;
        }

        public void BlockPrimary()
        {
            _blockPrimary = true;
        }
    }
}
