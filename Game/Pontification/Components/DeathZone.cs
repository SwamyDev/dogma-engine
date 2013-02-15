using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class DeathZone : Component
    {
        private PhysicsComponent _sensor;

        #region Public methods
        public override void Start()
        {
            // Get sensor.
            _sensor = GetComponent<PhysicsComponent>();

            if (_sensor == null)
                throw new ArgumentNullException("DeathZone needs a physic component attached");
            if (!_sensor.IsSensor)
                throw new ArgumentException("Attached physics component needs to be a sensor");

            // Hook up collision events.
            _sensor.OnCollisionEnter += onEnter;
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            var lifeDrain = go.GetComponent<LifeDrainAbility>();
            if (lifeDrain != null)
            {
                var drainer = lifeDrain.GetDrainer();
                lifeDrain.ResetToDrainer();
                drainer.SendMessage("Kill");
            }

            go.SendMessage("Kill");
        }
        #endregion
    }
}
