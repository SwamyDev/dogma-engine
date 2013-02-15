using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;

namespace Pontification.Components
{
    public class Liquid : Component
    {
        #region Private attributes
        private List<GameObject> _swimmers = new List<GameObject>();
        private PhysicsComponent _physics;
        #endregion

        #region Public properties
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
            if (_physics == null)
                throw new ArgumentNullException("Liquid component needs a physics component attached");

            _physics.OnCollisionEnter += onEnter;
            _physics.OnCollisionLeave += onLeave;
        }

        public override void Update(GameTime gameTime)
        {
            _swimmers.ForEach((swimmer) => 
            {
                float yDiff = GameObject.Position.Y - swimmer.Position.Y;
                if (yDiff <= 0.0f)
                {
                    swimmer.SendMessage("ChangeMotionState", new object[] { Pontification.Physics.DynamicObject.MotionStates.MS_MIDAIR });
                    swimmer.SendMessage("SetVelocity", new object[] { _physics.GetVelocity() });
                }
            });
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            if (go.GetComponent<MoveAlongPath>() == null)
                _swimmers.Add(go);
        }

        private void onLeave(GameObject go)
        {
            _swimmers.Remove(go);
        }
        #endregion
    }
}
