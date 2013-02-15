using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    class PriestDoorTrigger : Component
    {
        #region Private attributes
        private MoveAlongPath _movement;
        #endregion

        #region Public properties
        public GameObject Sensor { get; set; }
        public bool IsOpen { get; set; }
        #endregion

        public override void Start()
        {
            if (Sensor == null)
                throw new ArgumentNullException("PriestDoorTrigger needs a reference to a sensor object with a sensor physics component");

            var sensor = Sensor.GetComponent<PhysicsComponent>();

            if (sensor == null || sensor.IsSensor == false)
                throw new ArgumentNullException("PriestDoorTrigger needs a reference to a sensor object with a sensor physics component");

            _movement = GetComponent<MoveAlongPath>();
            if (_movement == null)
                throw new ArgumentNullException("PriestDoorTrigger needs a MoveAlongPath component");

            // Hook up coolsion event
            sensor.OnCollisionEnter += (go) =>
            {
                var stats = go.GetComponent<CharacterStats>();
                if (stats != null && stats.Category == CharacterCategory.CC_PRIEST)
                {
                    if (IsOpen == false)
                    {
                        IsOpen = true;
                        _movement.FollowPath();
                    }
                }
            };
        }
    }
}
