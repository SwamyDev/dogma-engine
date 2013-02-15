using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class MoveAlongPath : Component
    {
        #region Private attributes
        private PhysicsComponent _physics;
        private Vector2[] _path;
        private Vector2[] _resetPath;
        private Vector2 _currentVelocity;
        private Vector2 _resetPos;
        private int _currentVertexIdx;
        private int _threshold = 2;
        #endregion

        #region Public properties
        public List<Vector2> Path { get; set; }
        public float Speed { get; set; }
        public bool IsPatroling { get; set; }
        public bool IsMoving { get; set; }
        #endregion

        #region Callbacks
        public delegate void FinishedPathCallback();
        public FinishedPathCallback OnFinishedPath;
        #endregion

        public MoveAlongPath()
        {
            Path = new List<Vector2>();
        }

        #region Public methods
        public override void Start()
        {
            if (Path.Count <= 1)
                throw new InvalidOperationException("Path for MoveAlongPath component must have 2 or more vertices");

            _path = Path.ToArray();
            _resetPath = _path;
            _physics = GetComponent<PhysicsComponent>();
            //_physics.OnCollision += onCollision;

            // Cleanup
            Path.Clear();
            Path = null;

            if (SceneInfo.ResetInfoSealed == false)
            {
                SceneInfo.MovingPlatforms.Add(this);
            }

            if (IsPatroling)
            {
                Vector2 vertex = _path[_currentVertexIdx];
                if (GameObject.Position != vertex)
                    _currentVelocity = ConvertUnits.ToSimUnits(Vector2.Normalize(vertex - GameObject.Position) * Speed);
            }
        }

        public override void Update(GameTime gameTime)
        {                
            // Check if we reached way point.
            Vector2 vertex = _path[_currentVertexIdx];
            if ((GameObject.Position - vertex).Length() <= _threshold && IsMoving)
            {
                if (IsPatroling)
                {   // If patroling the next waypoint after the last is the first waypoint
                    _currentVertexIdx = (_currentVertexIdx + 1) % _path.Length;
                }
                else
                {
                    // Stop at last way point.
                    if (_currentVertexIdx == _path.Length - 1)
                    {
                        if (OnFinishedPath != null)
                            OnFinishedPath();
                        IsMoving = false;
                    }
                    else
                    {
                        _currentVertexIdx = _currentVertexIdx + 1;
                    }
                }

                // Calculate direction
                if (_path[_currentVertexIdx] != vertex)
                    _currentVelocity = ConvertUnits.ToSimUnits(Vector2.Normalize(_path[_currentVertexIdx] - vertex) * Speed);
                else
                    _currentVelocity = Vector2.Zero;
            }

            // Move towards current vertex.
            if (IsMoving)
            {
                // setPosition(GameObject.Position + _currentMovementDirection * (float)gameTime.ElapsedGameTime.TotalSeconds);
                _physics.SetVelocity(_currentVelocity);
            }
            else
            {
                _physics.SetVelocity(Vector2.Zero);
            }
        }

        public void FollowPath()
        {
            IsMoving = true;

            // If we reached the end reverse path.
            float length = (GameObject.Position - _path[_path.Length - 1]).Length();
            if (length <= _threshold)
            {
                var tmp = new List<Vector2>(_path);
                tmp.Reverse();
                _path = tmp.ToArray();

                _currentVertexIdx = (_path.Length - 1) - _currentVertexIdx;
                _currentVelocity = ConvertUnits.ToSimUnits(Vector2.Normalize(_path[_currentVertexIdx] - GameObject.Position) * Speed);
            }
        }

        public void ResetPath()
        {
            _currentVertexIdx = 0;
            _currentVelocity = Vector2.Zero;
            _path = _resetPath;
            setPosition(_path[_currentVertexIdx]);
            if (IsPatroling == false)
                IsMoving = false;
        }
        #endregion

        #region Private methods
        private void setPosition(Vector2 position)
        {
            if (_physics != null)
            {
                _physics.MoveTo(position);
            }
            else
            {
                GameObject.Position = position;
            }
        }

        private void onCollision(GameObject go)
        {
            go.SendMessage("AddVelocity", new object[] { _currentVelocity });
        }
        #endregion
    }
}
