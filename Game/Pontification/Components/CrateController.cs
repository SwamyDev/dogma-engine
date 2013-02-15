using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class CrateController : Component
    {
        private PhysicsComponent _physics;

        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
            if (_physics == null)
                throw new ArgumentNullException("A crate needs a physics component attached");

            _physics.AddGameObject(GameObject);
            _physics.StorePreviousVelocity = true;
            _physics.OnCollision += onCollision;
        }

        private void onCollision(GameObject go)
        {
            if (go != null && SceneInfo.Player == go)
            {
                var stats = go.GetComponent<CharacterStats>();
                if (stats != null && stats.Category == CharacterCategory.CC_GUARD)
                {
                    var phys = go.GetComponent<CharacterPhysicsComponent>();
                    int facing = phys.Facing;
                    if (phys.State == Physics.CharacterPhysicsState.CPS_PUSHING)
                    {
                        if (Math.Sign(GameObject.Position.X - go.Position.X) == facing)
                            _physics.ApplyForce(Vector2.UnitX * 100f * facing);
                    }
                }
            }
        }
    }
}
