using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Components
{
    public class SpiritStart : Component
    {
        private SpiritController _controller;

        public int Facing { get; set; }

        public override void Awake()
        {
            var anim = AddComponent<AnimationComponent>();
            var phys = AddComponent<SpiritPhysicsComponent>();
            var stats = AddComponent<CharacterStats>();
            var ballMorph = AddComponent<BallMorphAbility>();
            var spiritMorph = AddComponent<SpiritMorphAbility>();
            var sensitive = AddComponent<CursorSensitive>();
            var effects = AddComponent<SpiritEffectManager>();

            // Add default values.
            var animSprite = new Animation(GameObject.Content.Load<Texture2D>("CharacterSprites\\spirit_sprite"), 116, 9);
            animSprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 1.4f, 0, 14, 0, true));
            animSprite.AnimationDictionary.Add("Moving", new AnimationData("Moving", 1.4f, 0, 14, 1, true));
            animSprite.AnimationDictionary.Add("ChaseStart", new AnimationData("ChaseStart", 1.4f, 0, 14, 2, false));
            animSprite.AnimationDictionary.Add("Chase", new AnimationData("Chase", 1.4f, 0, 14, 3, true));
            animSprite.AnimationDictionary.Add("ChaseEnd", new AnimationData("ChaseEnd", 1.4f, 0, 14, 4, false));
            animSprite.AnimationDictionary.Add("BallIdle", new AnimationData("BallIdle", 0.86f, 0, 6, 5, true));
            animSprite.AnimationDictionary.Add("ToSpirit", new AnimationData("ToSpirit", 1.4f, 0, 14, 6, false));
            animSprite.AnimationDictionary.Add("ToBall", new AnimationData("ToBall", 1.5f, 0, 15, 7, false));
            animSprite.AnimationDictionary.Add("Die", new AnimationData("Die", 1.4f, 0, 14, 8, false));
            anim.Sprite = animSprite;
            anim.Facing = Facing;
            anim.Alpha = 1f;

            phys.HoverSpeed = 1f;
            phys.ChaseSpeed = 3.5f;

            stats.Health = 100f;
            stats.Category = CharacterCategory.CC_SPIRIT;

            ballMorph.MorphTime = 1.4f;
            spiritMorph.MorphTime = 1.4f;

            var vertices = new List<Vector2>(4);
            vertices.Add(GameObject.Position + new Vector2(-29, -29));
            vertices.Add(GameObject.Position + new Vector2(29, -29));
            vertices.Add(GameObject.Position + new Vector2(29, 29));
            vertices.Add(GameObject.Position + new Vector2(-29, 29));
            sensitive.Polygon = vertices;
            sensitive.CursorEffect = CursorState.CS_ALERT;

            effects.MorphAnimations.Add(SpiritForm.SF_BALL, "ToBall");
            effects.MorphAnimations.Add(SpiritForm.SF_SPIRIT, "ToSpirit");
            effects.MorphAnimations.Add(SpiritForm.SF_CHASER, "ChaseStart");

            effects.MovingAnimations.Add(SpiritMovements.SM_IDLE, "Idle");
            effects.MovingAnimations.Add(SpiritMovements.SM_BALLIDLE, "BallIdle");
            effects.MovingAnimations.Add(SpiritMovements.SM_MOVING, "Moving");
            effects.MovingAnimations.Add(SpiritMovements.SM_CHASING, "Chase");
            
            _controller = AddComponent<SpiritController>();
            _controller.IsActive = false;
        }

        public override void Start()
        {
            _controller.IsActive = true;
        }
    }
}
