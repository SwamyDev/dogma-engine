using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Components
{
    public class PriestStart : Component
    {
        private PriestController _controller;

        public int Facing { get; set; }

        public override void Awake()
        {
            var anim = AddComponent<AnimationComponent>();
            var possessAnim = AddComponent<AnimationComponent>();
            //var decayAnim = AddComponent<AnimationComponent>();
            var phys = AddComponent<CharacterPhysicsComponent>();
            var sound = AddComponent<SoundComponent>();
            var stats = AddComponent<CharacterStats>();
            var exorcise = AddComponent<ExorciseAbility>();
            var lifedrain = AddComponent<LifeDrainAbility>();
            var possessable = AddComponent<Possessable>();
            var sensitive = AddComponent<CursorSensitive>();
            var effects = AddComponent<CharacterEffektManager>();

            // Add default values.
            var animSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\priest_sprite"), 116, 11);
            animSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.85f, 0, 17, 0, true));
            animSprite.AnimationDictionary.Add("Walking", new AnimationData("Walking", 0.8f, 0, 16, 1, true));
            animSprite.AnimationDictionary.Add("Running", new AnimationData("Running", 0.66f, 0, 16, 2, true));
            animSprite.AnimationDictionary.Add("JumpOff", new AnimationData("JumpOff", 0.45f, 0, 9, 3, false));
            animSprite.AnimationDictionary.Add("MidAir", new AnimationData("MidAir", 1f, 0, 1, 4, true));
            animSprite.AnimationDictionary.Add("Landing", new AnimationData("Landing", 0.45f, 0, 9, 5, false));
            animSprite.AnimationDictionary.Add("ExorciseStart", new AnimationData("ExorciseStart", 0.25f, 0, 6, 6, false));
            animSprite.AnimationDictionary.Add("ExorciseIdle", new AnimationData("ExorciseIdle", 1f, 0, 1, 7, true));
            animSprite.AnimationDictionary.Add("ExorciseWalking", new AnimationData("ExorciseWalking", 0.79f, 0, 19, 8, true));
            animSprite.AnimationDictionary.Add("ExorciseStop", new AnimationData("ExorciseStop", 0.25f, 0, 6, 9, false));
            animSprite.AnimationDictionary.Add("Die", new AnimationData("Die", 1f, 0, 20, 10, false));
            anim.Sprite = animSprite;
            anim.Facing = Facing;
            anim.Alpha = 1f;

            var possessAnimSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\priest_sprite_possessed"), 116, 11);
            possessAnimSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.85f, 0, 17, 0, true));
            possessAnimSprite.AnimationDictionary.Add("Walking", new AnimationData("Walking", 0.8f, 0, 16, 1, true));
            possessAnimSprite.AnimationDictionary.Add("Running", new AnimationData("Running", 0.66f, 0, 16, 2, true));
            possessAnimSprite.AnimationDictionary.Add("JumpOff", new AnimationData("JumpOff", 0.45f, 0, 9, 3, false));
            possessAnimSprite.AnimationDictionary.Add("MidAir", new AnimationData("MidAir", 1f, 0, 1, 4, true));
            possessAnimSprite.AnimationDictionary.Add("Landing", new AnimationData("Landing", 0.45f, 0, 9, 5, false));
            possessAnimSprite.AnimationDictionary.Add("ExorciseStart", new AnimationData("ExorciseStart", 0.25f, 0, 6, 6, false));
            possessAnimSprite.AnimationDictionary.Add("ExorciseIdle", new AnimationData("ExorciseIdle", 1f, 0, 1, 7, true));
            possessAnimSprite.AnimationDictionary.Add("ExorciseWalking", new AnimationData("ExorciseWalking", 0.79f, 0, 19, 8, true));
            possessAnimSprite.AnimationDictionary.Add("ExorciseStop", new AnimationData("ExorciseStop", 0.25f, 0, 6, 9, false));
            possessAnimSprite.AnimationDictionary.Add("Decay", new AnimationData("Decay", 1.2f, 1, 29, 10, false));
            possessAnim.Sprite = possessAnimSprite;
            possessAnim.Facing = Facing;
            possessAnim.Alpha = 1f;
            possessAnim.IsActive = false;

            /*var decayAnimSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\priest_sprite_decay"), 116, 10);
            decayAnimSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.85f, 0, 17, 0, true));
            decayAnimSprite.AnimationDictionary.Add("Walking", new AnimationData("Walking", 0.8f, 0, 16, 1, true));
            decayAnimSprite.AnimationDictionary.Add("Running", new AnimationData("Running", 0.66f, 0, 16, 2, true));
            decayAnimSprite.AnimationDictionary.Add("JumpOff", new AnimationData("JumpOff", 0.45f, 0, 9, 3, false));
            decayAnimSprite.AnimationDictionary.Add("MidAir", new AnimationData("MidAir", 1f, 0, 1, 4, true));
            decayAnimSprite.AnimationDictionary.Add("Landing", new AnimationData("Landing", 0.45f, 0, 9, 5, false));
            decayAnimSprite.AnimationDictionary.Add("ExorciseStart", new AnimationData("ExorciseStart", 0.25f, 0, 6, 6, false));
            decayAnimSprite.AnimationDictionary.Add("ExorciseIdle", new AnimationData("ExorciseIdle", 1f, 0, 1, 7, true));
            decayAnimSprite.AnimationDictionary.Add("ExorciseWalking", new AnimationData("ExorciseWalking", 0.79f, 0, 19, 8, true));
            decayAnimSprite.AnimationDictionary.Add("ExorciseStop", new AnimationData("ExorciseStop", 0.25f, 0, 6, 9, false));
            decayAnim.Sprite = decayAnimSprite;
            decayAnim.Facing = Facing;
            decayAnim.Alpha = 0.0f;
            decayAnim.IsActive = false;*/

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
            stats.Category = CharacterCategory.CC_PRIEST;

            exorcise.EffectSprite = GameObject.Content.Load<Texture2D>("Effects\\holy-beam");
            exorcise.Offset = new Vector2(35, -8);
            exorcise.Damage = 2;
            exorcise.Range = 500;

            lifedrain.DrainSpeed = 1.2f;
            lifedrain.IsActive = false;

            possessable.PossessTime = 1f;

            var vertices = new List<Vector2>(4);
            vertices.Add(GameObject.Position + new Vector2(-29, -87));
            vertices.Add(GameObject.Position + new Vector2(29, -87));
            vertices.Add(GameObject.Position + new Vector2(29, 87));
            vertices.Add(GameObject.Position + new Vector2(-29, 87));
            sensitive.Polygon = vertices;
            sensitive.CursorEffect = CursorState.CS_ALERT;

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
            effects.AbilityStartAnimations.Add("LifeDrainAbility", "Decay");
            effects.AbilityStartAnimations.Add("ExorciseAbility", "ExorciseStart");
            effects.AbilityIdleAnimations.Add("ExorciseAbility", "ExorciseIdle");
            effects.AbilityEndAnimations.Add("ExorciseAbility", "ExorciseStop");

            _controller = AddComponent<PriestController>();
            _controller.IsActive = false;
        }

        public override void Start()
        {
            SendMessage("ChangeFacing", new object[] { Facing });
            SendMessage("UpdateStartFacing", new object[] { Facing });
            _controller.IsActive = true;
        }
    }
}
