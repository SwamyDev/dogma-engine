using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public enum TriggerTypes
    {
        TT_ON_ENTER,
        TT_ON_LEAVE,
        TT_ON_USE
    }
    public class TriggerComponent : Component
    {
        private PhysicsComponent _sensor;

        public TriggerTypes TriggerType { get; set; }
        public GameObject Target { get; set; }

        #region Public methods
        public override void Start()
        {
            _sensor = GetComponent<PhysicsComponent>();
            if (_sensor == null || _sensor.IsSensor == false)
                throw new ArgumentNullException("Trigger components need a sensor component");

            // Hook up to callbacks
            if (TriggerType == TriggerTypes.TT_ON_ENTER)
                _sensor.OnCollisionEnter += onEnter;
            if (TriggerType == TriggerTypes.TT_ON_LEAVE)
                _sensor.OnCollisionLeave += onLeave;
            if (TriggerType == TriggerTypes.TT_ON_USE)
                _sensor.OnCollision += onCollision;
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            if (go == SceneInfo.Player)
            {
                Target.SendMessage("Trigger");
            }
        }

        private void onLeave(GameObject go)
        {
            if (go == SceneInfo.Player)
            {
                Target.SendMessage("Trigger");
            }
        }

        private void onCollision(GameObject go)
        {
            if (go != null && go == SceneInfo.Player)
            {
                if (go.GetComponent<PlayerInput>().IsInteracting)
                {
                    GetComponents<TextureComponent>().ForEach((texture) => { texture.IsActive = !texture.IsActive; });
                    Target.SendMessage("Trigger");
                }
            }
        }
        #endregion
    }
}
