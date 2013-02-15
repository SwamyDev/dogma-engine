using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.ScreenManagement;
using Pontification.SceneManagement;
using Pontification.Physics;

namespace Pontification.Components
{
    public enum CursorState
    {
        CS_IDLE,
        CS_ALERT
    }

    public class Cursor : Component
    {
        #region Private attributes
        private static CursorState _oldState;
        private static Vector2 _cameraTranslation;
        private static bool _initialized;
        private Dictionary<CursorState, string> _cursorAnimations = new Dictionary<CursorState, string>(2);
        #endregion

        #region Public properties
        public static Cursor Instance { get; private set; }
        public static CursorState State { get; set; }
        public static bool Locked { get; set; }
        #endregion

        public Cursor()
        {
            if (_initialized)
                throw new NotSupportedException("You can only have one Cursor Component in one game");

            _initialized = true;
            Instance = this;
        }

        #region Public methods
        public override void Awake()
        {
            var anim = AddComponent<AnimationComponent>();

            var animSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\cursor_sprite"), 70, 2);
            animSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.7f, 0, 8, 0, true));
            animSprite.AnimationDictionary.Add("Alert", new AnimationData("Alert", 0.5f, 0, 10, 1, true));
            anim.Sprite = animSprite;
            anim.Alpha = 1f;

            _cursorAnimations.Add(CursorState.CS_IDLE, "Idle");
            _cursorAnimations.Add(CursorState.CS_ALERT, "Alert");
        }

        public override void Start()
        {
            State = CursorState.CS_IDLE;
            Pontification.SceneManagement.SceneInfo.Cursor = GameObject;
            SendMessage("SetAnimation", new object[] { _cursorAnimations[State] });
        }

        public override void Update(GameTime gameTime)
        {
            if (Locked == false)
            {
                var translation = Camera.NormalView.Translation;
                _cameraTranslation.X = translation.X;
                _cameraTranslation.Y = translation.Y;

                GameObject.Position = InputState.MousePosition - _cameraTranslation;

                foreach (var sensitive in SceneInfo.CursorSensitives)
                {
                    if (sensitive.IsActive == false)
                        continue;

                    if (sensitive.CheckHovering(GameObject.Position))
                    {
                        State = CursorState.CS_ALERT;
                        break;
                    }
                    State = CursorState.CS_IDLE;
                }

                if (State != _oldState)
                {
                    SendMessage("SetAnimation", new object[] { _cursorAnimations[State] });
                }
                _oldState = State;
            }
        }

        public void SwitchAnimationComponents()
        {
            foreach (var anim in GetComponents<AnimationComponent>())
                anim.IsActive = !anim.IsActive;
        }
        #endregion
    }
}
