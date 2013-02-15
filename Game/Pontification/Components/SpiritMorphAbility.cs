using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Interfaces;

namespace Pontification.Components
{
    public class SpiritMorphAbility : Component, IAbility
    {
        #region Private attributes
        private AbilityManager _manager;
        private SpiritPhysicsComponent _physics;
        private bool _isDisposed;
        #endregion

        #region Public properties
        public Vector2 Offset { get; set; }
        public float Range { get; set; }
        public float MorphTime { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<SpiritPhysicsComponent>();
            if (_physics == null)
                throw new ArgumentNullException("Spirits need a SpiritPhysicsComponent to work properly");

            _manager = GetComponent<AbilityManager>();

            // If not present in the current game object add it.
            if (_manager == null)
                _manager = AddComponent<AbilityManager>();

            // Add ability to manager
            _manager.AddAbility(this, true);
        }

        public override string ToString()
        {
            return "BallMorphAbility";
        }
        #endregion

        #region IAbility methods
        public void Use()
        {
            _manager.Lock();
            SendMessage("LockMovement");
            if (_physics.Form == SpiritForm.SF_SPIRIT)
                _physics.ChangeForm(SpiritForm.SF_CHASER);
            Scheduler.Instance.AddTask(morph());
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void ResetTimer()
        {
        }
        #endregion

        protected override void disposing()
        {
            _isDisposed = true;
        }

        #region Private methods
        System.Collections.IEnumerator morph()
        {
            yield return TimeSpan.FromSeconds(MorphTime);
            /*else if (_physics.Form == SpiritFrom.SF_SPIRIT)
                _physics.ChangeForm(SpiritFrom.SF_BALL);*/
            if (_isDisposed == false)
            {
                _manager.Unlock();
                SendMessage("UnlockMovement");
            }
        }
        #endregion
    }
}
