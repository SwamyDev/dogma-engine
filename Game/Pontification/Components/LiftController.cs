using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class LiftController : Component
    {
        private MoveAlongPath _movement;
        private bool _isReturning;
        private bool _isReseted;
        private bool _isOn = true;

        public float IdleDelay { get; set; }
        public bool MoveBack { get; set; }

        #region Public methods
        public override void Start()
        {
            _movement = GetComponent<MoveAlongPath>();

            if (_movement == null)
                throw new ArgumentNullException("LiftController game object needs a move along path component to work.");

            if (MoveBack)
                _movement.OnFinishedPath += finishedPath;
        }

        public void Trigger()
        {
            if (_movement.IsMoving == false && _isOn)
            {
                _movement.FollowPath();
                _isReseted = false;
            }
        }

        public void PowerOn()
        {
            _isOn = true;
        }

        public void PowerOff()
        {
            _isOn = false;
        }

        public void ResetLiftController()
        {
            if (_isReturning && _movement.IsMoving == false)
                _isReseted = true;
        }
        #endregion

        #region Private methods
        private void finishedPath()
        {
            if (_isReturning == false)
            {
                _isReturning = true;
                Scheduler.Instance.AddTask(returnPath());
            }
            else
            {
                _isReturning = false;
            }
        }

        private System.Collections.IEnumerator returnPath()
        {
            yield return TimeSpan.FromSeconds(IdleDelay);

            if (_isReseted == false)
                _movement.FollowPath();
        }
        #endregion
    }
}
