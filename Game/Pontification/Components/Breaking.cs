using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class Breaking : Component
    {
        #region Private attributes
        private PhysicsComponent _physics;
        private Vector2 _previousVelocity;
        private float _treasholdVelocity = 7f;
        private int _impulses;
        #endregion

        #region Public properties
        public Dictionary<string, string> BreakingAnimations { get; set; }
        public int BreakImpules { get; set; }
        #endregion

        #region Public methods
        public Breaking()
        {
            BreakingAnimations = new Dictionary<string, string>(4);
        }

        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();

            if (_physics == null)
                throw new ArgumentNullException("Breaking needs a physics component attached");

            _physics.OnCollisionEnter += onEnter;

            if (BreakingAnimations.ContainsKey("0"))
                SendMessage("SetAnimation", new object[] { BreakingAnimations["0"] });
        }

        public override void Update(GameTime gameTime)
        {
            if (_physics.IsStatic == false)
            {
                var velocity = _physics.GetVelocity();
                if (velocity.Y == 0 && _previousVelocity.Y >= _treasholdVelocity)
                    AddImpulse();
                _previousVelocity = velocity;
            }
        }

        public void AddImpulse()
        {
            _impulses++;
            if (BreakingAnimations.ContainsKey(_impulses.ToString()))
                SendMessage("SetAnimation", new object[] { BreakingAnimations[_impulses.ToString()] });
            if (_impulses >= BreakImpules)
            {
                _physics.IsActive = false;
                GameObject.IsActive = false;
                GameObject.Dispose();
            }
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            if (go != null)
            {
                var crate = go.GetComponent<CrateController>();
                if (crate != null)
                {
                    var phys = go.GetComponent<PhysicsComponent>();
                    if (phys.PreviousVelocity.Y > _treasholdVelocity)
                        AddImpulse();
                }
                var stats = go.GetComponent<CharacterStats>();
                if (stats == null || stats.Category == CharacterCategory.CC_SPIRIT)
                    return;

                var charPhysics = go.GetComponent<CharacterPhysicsComponent>();
                if (charPhysics != null)
                    if (charPhysics.PreviouseVelocity.Y > _treasholdVelocity)
                        AddImpulse();
            }
        }
        #endregion
    }
}
