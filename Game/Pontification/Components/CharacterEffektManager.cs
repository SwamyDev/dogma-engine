using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;

namespace Pontification.Components
{
    /// <summary>
    /// Manages the characres effects like sound and animation according
    /// to specific game events like the use of abilities or states of 
    /// the physic character component.
    /// </summary>
    public class CharacterEffektManager : Component
    {
        #region Private attributes
        private Bag<AnimationComponent> _animations;
        private CharacterPhysicsComponent _physics;
        private CharacterPhysicsState _oldPhysicState;
        private bool _resetPhysics;

        private AbilityManager _abilities;
        private AbilityState _oldAbilityState;
        private bool _resetAbilities;

        private bool _isPlayingAbility;
        private bool _isPlayingStoppingAnimation;
        private bool _isPlayingCinematic;
        #endregion

        #region Public properties
        public Dictionary<CharacterPhysicsState, string> PhysicAnimations { get; set; }
        public Dictionary<CharacterPhysicsState, string> PhysicSounds { get; set; }
        public Dictionary<string, string> AbilityStartAnimations { get; set; }
        public Dictionary<string, string> AbilityStartSounds { get; set; }
        public Dictionary<string, string> AbilityIdleAnimations { get; set; }
        public Dictionary<string, string> AbilityIdleSounds { get; set; }
        public Dictionary<string, string> AbilityEndAnimations { get; set; }
        public Dictionary<string, string> AbilityEndSounds { get; set; }
        public Dictionary<string, string> CinematicAnimations { get; set; }
        public Dictionary<string, string> CinematicSounds { get; set; }
        #endregion

        public CharacterEffektManager()
        {
            PhysicAnimations = new Dictionary<CharacterPhysicsState, string>(6);
            PhysicSounds = new Dictionary<CharacterPhysicsState, string>(6);
            AbilityStartAnimations = new Dictionary<string, string>(4);
            AbilityStartSounds = new Dictionary<string, string>(4);
            AbilityIdleAnimations = new Dictionary<string, string>(4);
            AbilityIdleSounds = new Dictionary<string, string>(4);
            AbilityEndAnimations = new Dictionary<string, string>(4);
            AbilityEndSounds = new Dictionary<string, string>(4);
            CinematicAnimations = new Dictionary<string, string>(4);
            CinematicSounds = new Dictionary<string, string>(4);
        }

        #region Public methods
        public override void Start()
        {
            // Get animation components.
            var anims = GetComponents<AnimationComponent>();
            _animations = new Bag<AnimationComponent>(anims.Count);
            anims.ForEach((anim) => 
            {
                anim.Sprite.AnimationFinished += animationFinished;
                _animations.Add(anim); 
            });

            // Get source components.
            _physics = GetComponent<CharacterPhysicsComponent>();
            _abilities = GetComponent<AbilityManager>();

            // Hook into physics events.
            if (_physics != null)
                _physics.StateChanged += changePhysicsAnimations;

            if (_abilities == null)
            {
                // Try a few frames later again.
                Scheduler.Instance.AddTask(getAbilityManager());
            }
            else
            {
                _abilities.StateChanged += changeAbilityAnimations;
            }
        }

        public void PlayCinematicAnimation(string name)
        {
            playAnimation(CinematicAnimations[name]);
            _isPlayingCinematic = true;
            SendMessage("LockInput");
        }

        public void ResetEffects()
        {
            _isPlayingCinematic = false;
            SendMessage("UnlockInput");
            playAnimation(PhysicAnimations[CharacterPhysicsState.CPS_IDLE]);
            SendMessage("Play", new object[] { PhysicSounds[CharacterPhysicsState.CPS_IDLE] });
        }
        #endregion

        #region Protected methods
        protected override void disposing()
        {
            _animations.ForEach((anim) => 
            {
                anim.Sprite.AnimationFinished -= animationFinished;
            });

            if (_physics != null)
                _physics.StateChanged -= changePhysicsAnimations;

            if (_abilities != null)
                _abilities.StateChanged -= changeAbilityAnimations;
        }
        #endregion

        #region Private methods
        private System.Collections.IEnumerator getAbilityManager()
        {
            yield return TimeSpan.FromSeconds(0.1f);
            _abilities = GetComponent<AbilityManager>();
            _abilities.StateChanged += changeAbilityAnimations;
        }

        private void animationFinished(object sender, AnimationEventArgs e)
        {
            // Stop cinematics if finished
            string animName = e.AnimData.Name;

            if (_isPlayingStoppingAnimation == false)
                _isPlayingAbility = false;

            _isPlayingStoppingAnimation = false;

            if (CinematicAnimations.ContainsKey(animName))
            {
                _isPlayingCinematic = false;
                SendMessage("UnlockInput");
            }
            if (AbilityStartAnimations.ContainsValue(animName))
            {
                if (_abilities.State != AbilityState.AS_IDLE)
                {
                    if (AbilityIdleAnimations.ContainsKey(_abilities.UsingAbility))
                    {
                        _isPlayingAbility = true;
                        playAnimation(AbilityIdleAnimations[_abilities.UsingAbility]);
                    }
                }
            }
        }

        private void changePhysicsAnimations(CharacterPhysicsState newState)
        {
            if (newState != CharacterPhysicsState.CPS_IDLE || _isPlayingAbility == false)
                playAnimation(PhysicAnimations[newState]);
            SendMessage("Play", new object[] { PhysicSounds[newState] });
        }

        private void changeAbilityAnimations(AbilityState newState, string animName)
        {
            if (newState != AbilityState.AS_IDLE)
            {
                if (AbilityStartAnimations.ContainsKey(_abilities.UsingAbility))
                {
                    playAnimation(AbilityStartAnimations[_abilities.UsingAbility]);
                    _isPlayingAbility = true;
                }
                else if (AbilityIdleAnimations.ContainsKey(_abilities.UsingAbility))
                {
                    _isPlayingAbility = true;
                    playAnimation(AbilityIdleAnimations[_abilities.UsingAbility]);
                }

                //if (AbilityStartSounds.ContainsKey(_abilities.UsingAbility))
                    //SendMessage("Play", new object[] { AbilityStartSounds[_abilities.UsingAbility] });
                //if (AbilityIdleSounds.ContainsKey(_abilities.UsingAbility))
                    //SendMessage("Play", new object[] { AbilityIdleSounds[_abilities.UsingAbility] });
            }
            else
            {
                if (AbilityEndAnimations.ContainsKey(animName))
                {
                    _isPlayingAbility = true;
                    _isPlayingStoppingAnimation = true;
                    playAnimation(AbilityEndAnimations[animName]);
                }
                else if (!_isPlayingCinematic)
                {
                    _isPlayingAbility = false;
                    SendMessage("StopAnimation");
                    if (_physics != null)
                        changePhysicsAnimations(_physics.State);
                }

                if (AbilityEndSounds.ContainsKey(animName))
                {
                    playAnimation(AbilityEndSounds[animName]);
                }
                else if (!_isPlayingCinematic)
                {
                    // SendMessage("Stop");
                    if (_physics != null)
                        changePhysicsAnimations(_physics.State);
                }
            }
        }

        void playAnimation(string name)
        {
            _animations.ForEachWith((anim) => { anim.SetAnimation(name); }, (anim) => { return anim.IsActive; });
        }
        #endregion
    }
}
