using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class FuseBox : Component
    {
        #region Private attributes
        private PhysicsComponent _physics;
        private bool _isBlocked;
        private bool _isOn = true;
        #endregion

        #region Public properties
        public GameObject Target { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();

            if (_physics == null || _physics.IsSensor == false)
                throw new ArgumentNullException("FuseBox needs a sensor attached to it");

            _physics.OnCollision += onCollision;
        }
        #endregion

        #region Private methods
        private void onCollision(GameObject go)
        {
            if (go != null && go == SceneManagement.SceneInfo.Player)
            {
                if (go.GetComponent<PlayerInput>().IsInteracting && _isBlocked == false)
                {
                    if (_isOn)
                    {
                        GetComponents<TextureComponent>().ForEach((tex) => { tex.IsActive = !tex.IsActive; });
                        Target.SendMessage("PowerOff");
                        _isOn = false;
                    }
                    else
                    {
                        GetComponents<TextureComponent>().ForEach((tex) => { tex.IsActive = !tex.IsActive; });
                        Target.SendMessage("PowerOn");
                        _isOn = true;
                    }

                    _isBlocked = true;
                    Scheduler.Instance.AddTask(unlock());
                }
            }
        }

        private System.Collections.IEnumerator unlock()
        {
            yield return TimeSpan.FromSeconds(0.1f);
            _isBlocked = false;
        }
        #endregion
    }
}
