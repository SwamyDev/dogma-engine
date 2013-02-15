using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public enum SpiritMovements
    {
        SM_IDLE,
        SM_BALLIDLE,
        SM_MOVING,
        SM_CHASING
    }
    public class SpiritEffectManager : Component
    {
        private Bag<AnimationComponent> _animations;
        private SpiritPhysicsComponent _physics;
        private SpiritForm _currentForm;
        private bool _isChangingForm;

        public Dictionary<SpiritForm, string> MorphAnimations { get; set; }
        public Dictionary<SpiritMovements, string> MovingAnimations { get; set; }

        public SpiritEffectManager()
        {
            MorphAnimations = new Dictionary<SpiritForm, string>(3);
            MovingAnimations = new Dictionary<SpiritMovements, string>(4);
        }

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
            _physics = GetComponent<SpiritPhysicsComponent>();

            // Hook into physics events.
            if (_physics != null)
            {
                _physics.OnFormChange = onFormChanged;
                _physics.OnStateChange = onStateChanged;
            }

            _currentForm = SpiritForm.SF_BALL;
            playAnimation("BallIdle");
        }

        protected override void disposing()
        {
            _animations.ForEach((anim) => { anim.Sprite.AnimationFinished -= animationFinished; });
        }

        private void animationFinished(object sender, AnimationEventArgs e)
        {
            if (MorphAnimations.ContainsValue(e.AnimData.Name))
                _isChangingForm = false;
        }

        private void onFormChanged(SpiritForm form)
        {
            _currentForm = form;
            _isChangingForm = true;
            if (MorphAnimations.ContainsKey(form))
                playAnimation(MorphAnimations[form]);
        }

        private void onStateChanged(SpiritState state)
        {
            if (_isChangingForm == false)
            {
                if (state == SpiritState.SS_IDLE)
                {
                    if (_currentForm == SpiritForm.SF_BALL)
                        playAnimation(MovingAnimations[SpiritMovements.SM_BALLIDLE]);
                    else
                        playAnimation(MovingAnimations[SpiritMovements.SM_IDLE]);
                }
                else if (state == SpiritState.SS_MOVING)
                {
                    if (_currentForm == SpiritForm.SF_SPIRIT)
                        playAnimation(MovingAnimations[SpiritMovements.SM_MOVING]);
                    else if (_currentForm == SpiritForm.SF_CHASER)
                        playAnimation(MovingAnimations[SpiritMovements.SM_CHASING]);
                }
            }
        }

        void playAnimation(string name)
        {
            _animations.ForEachWith((anim) => { anim.SetAnimation(name); }, (anim) => { return anim.IsActive; });
        }
    }
}
