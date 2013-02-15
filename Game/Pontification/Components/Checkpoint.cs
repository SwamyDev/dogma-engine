using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Monitoring;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class Checkpoint : Component
    {
        private static int _checkpointTotal;

        private PhysicsComponent _sensor;
        private int _checkpointNumber;

        #region Public methods
        public override void Start()
        {
            // Get sensor.
            _sensor = GetComponent<PhysicsComponent>();

            if (_sensor == null)
                throw new ArgumentNullException("Checkpoint needs a physic component attached");
            if (!_sensor.IsSensor)
                throw new ArgumentException("Attached physics component needs to be a sensor");

            // Hook up collision events.
            _sensor.OnCollisionEnter += onEnter;

            _checkpointTotal++;
            _checkpointNumber = _checkpointTotal;
        }

        public int GetCheckpointNumber()
        {
            return _checkpointNumber; 
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            if (go == SceneInfo.Player && SceneInfo.CurrentCheckpoint != GameObject)
            {
                SceneInfo.CurrentCheckpoint = GameObject;
                Logger.Instance.Log(string.Format("Set checkpoint {0}", GetCheckpointNumber()), MessageType.MT_STATISTICS);
            }
        }
        #endregion
    }
}
