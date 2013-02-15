using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    /// <summary>
    /// Adds typical player components to the game object with standard values.
    /// </summary>
    public class PlayerStart :  Component
    {
        public int Facing { get; set; }

        public override void Awake()
        {
            var anim = AddComponent<AnimationComponent>();
            var phys = AddComponent<CharacterPhysicsComponent>();
            var input = AddComponent<PlayerInput>();
            var sound = AddComponent<SoundComponent>();
            var stats = AddComponent<CharacterStats>();
            var possess = AddComponent<PossessionAbility>();
            var effects = AddComponent<CharacterEffektManager>();

            // Add default values.
            var animSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\player_sprite"), 116, 12);
            animSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.75f, 0, 12, 0, true));
            animSprite.AnimationDictionary.Add("Walking", new AnimationData("Walking", 1f, 0, 24, 1, true));
            animSprite.AnimationDictionary.Add("Running", new AnimationData("Running", 0.79f, 0, 22, 2, true));
            animSprite.AnimationDictionary.Add("JumpOff", new AnimationData("JumpOff", 0.9f, 0, 25, 3, false));
            animSprite.AnimationDictionary.Add("MidAir", new AnimationData("MidAir", 1f, 0, 1, 4, true));
            animSprite.AnimationDictionary.Add("Landing", new AnimationData("Landing", 0.45f, 0, 9, 5, false));
            animSprite.AnimationDictionary.Add("SlipStart", new AnimationData("SlipStart", 0.15f, 0, 3, 6, false));
            animSprite.AnimationDictionary.Add("SlipIdle", new AnimationData("SlipIdle", 1f, 0, 1, 7, true));
            animSprite.AnimationDictionary.Add("SlipEnd", new AnimationData("SlipEnd", 0.2f, 0, 5, 8, false));
            animSprite.AnimationDictionary.Add("Falling", new AnimationData("Falling", 0.58f, 0, 14, 9, true));
            animSprite.AnimationDictionary.Add("PossessStart", new AnimationData("PossessStart", 1.3f, 0, 13, 10, false));
            animSprite.AnimationDictionary.Add("ConsumeEnergy", new AnimationData("ConsumeEnergy", 0.6f, 1, 30, 11, false));
            anim.Sprite = animSprite;
            anim.Facing = Facing;
            anim.Alpha = 0.5f;

            phys.StandingJumpCells = new Vector2(3f, 2.5f);
            phys.RunningJumpCells = new Vector2(6f, 2.5f);
            phys.CollisionShapeCells = new Vector2(1, 3);
            phys.ViewPoint = new Vector2(1, 3);
            phys.Mass = 5f;
            phys.Facing = Facing;

            var soundEffect = new SoundEffects();
            soundEffect.SoundDictionary.Add("Running", new SoundData("Running", "CharacterSounds\\running", 0.2f, 0, 0, 0, true));
            soundEffect.SoundDictionary.Add("JumpOff", new SoundData("JumpOff", "CharacterSounds\\start-jump", 1f, 0, 0, 0, false));
            soundEffect.SoundDictionary.Add("Landing", new SoundData("Landing", "CharacterSounds\\landing", 1f, 0, 0, 0, false));
            soundEffect.SoundDictionary.Add("Possess", new SoundData("Landing", "CharacterSounds\\possess", 1f, 0, 0, 0, false));
            soundEffect.LoadSoundEffects(GameObject.Content);
            sound.Sounds = soundEffect;

            stats.Health = 100f;
            stats.Category = CharacterCategory.CC_SPIRIT;

            possess.Range = 450;
            possess.EffectSprite = GameObject.Content.Load<Texture2D>("Effects\\possess-beam"); ;

            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_IDLE, "Idle");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_WALKING, "Running");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_RUNNING, "Running");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_JUMPOFF, "JumpOff");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_MIDAIR, "MidAir");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_LANDING, "Landing");
            effects.PhysicAnimations.Add(Physics.CharacterPhysicsState.CPS_PUSHING, "Walking");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_IDLE, "");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_WALKING, "Running");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_RUNNING, "Running");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_JUMPOFF, "JumpOff");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_MIDAIR, "");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_LANDING, "Landing");
            effects.PhysicSounds.Add(Physics.CharacterPhysicsState.CPS_PUSHING, "");
            effects.AbilityStartAnimations.Add("PossessionAbility", "PossessStart");
            effects.CinematicAnimations.Add("ConsumeEnergy", "ConsumeEnergy");
        }

        public override void Start()
        {
            SendMessage("ChangeFacing", new object[] { Facing });
            SendMessage("UpdateStartFacing", new object[] { Facing });
        }
    }
}
