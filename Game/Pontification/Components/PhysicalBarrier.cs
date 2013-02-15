using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class PhysicalBarrier : Component
    {
        #region Private attributes
        private PhysicsComponent _physics;
        private DamageTypes _damageType = DamageTypes.DT_PHYSICAL;
        private bool _indestructible;
        #endregion

        #region Public properties
        public float Health { get; set; }
        public float Damage { get; set; }
        #endregion

        #region Public methods
        public PhysicalBarrier()
        {
            Health = 100.0f;
            Damage = 10.0f;
        }

        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
            if (_physics == null)
                throw new ArgumentNullException("PhysicalBarrier needs a physics component attached");

            _physics.AddGameObject(GameObject);
            _physics.OnCollision += onCollision;

            if (Health == 0)
                _indestructible = true;
        }

        public void TakeDamage(float amount, DamageTypes type)
        {
            if (_indestructible)
                return;

            if (type == DamageTypes.DT_PHYSICAL)
            {
                Health -= amount;
                if (Health <= 0)
                {
                    GameObject.IsActive = false;
                    GameObject.Dispose();
                }
            }
        }
        #endregion

        #region Private methods
        public void onCollision(GameObject go)
        {
            if (go != null)
                go.SendMessage("TakeDamage", new object[] { Damage, _damageType });
        }
        #endregion
    }
}
