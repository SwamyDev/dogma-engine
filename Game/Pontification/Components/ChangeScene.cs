using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.SceneManagement;
using Pontification.ScreenManagement;

namespace Pontification.Components
{
    public class ChangeScene : Component
    {
        private PhysicsComponent _physics;

        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
            if (_physics == null || _physics.IsSensor == false)
                throw new ArgumentNullException("ChangeScene needs a sensor added to it");

            _physics.OnCollisionEnter += onEnter;
        }

        private void onEnter(GameObject go)
        {
            if (SceneInfo.Player == go)
            {
                // Exit game for now.
                ScreenManager.Instance.Game.Exit();
            }
        }
    }
}
