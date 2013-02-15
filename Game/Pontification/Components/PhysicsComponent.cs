using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Physics;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    /// <summary>
    /// To move game objects with Physics component attached immediately use MoveTo method. Changing the GameObject's 
    /// Position won't have any effect because the position is controlled by the physics engine.
    /// </summary>
    public class PhysicsComponent : Component
    {
        #region Private attributes
        private PhysicsObject _physics;
        private GameObject _pendingGameObject;
        private Dictionary<GameObject, bool> _collidingGameObjects = new Dictionary<GameObject, bool>(6);
        private List<GameObject> _removeCollidingObjects = new List<GameObject>(6);
        private Vector2 _startPosition;
        private bool _preUpdateEventRegistered;
        #endregion

        #region Public properties.
        public Vector2 PreviousVelocity;
        public bool StorePreviousVelocity;
        public List<Vector2> Polygon { get; set; }
        public float Mass { get; set; }
        public float Friction { get; set; }
        public float Restitution { get; set; }
        public bool IgnoreGravity { get; set; }
        public bool IsStatic { get; set; }
        public bool IsSensor { get; set; }
        public bool IsProjectile { get; set; }
        public bool IsEthereal { get; set; }
        #endregion

        #region Delegates
        public delegate void CollisionCallback(GameObject go);
        public CollisionCallback OnCollision;
        public CollisionCallback OnCollisionEnter;
        public CollisionCallback OnCollisionLeave;
        #endregion

        public PhysicsComponent()
        {
        }

        #region Public methods
        public override void Start()
        {
            // Convert Polygon to physic units.

            var vertices = new List<Vector2>();

            // Get centroid first
            Polygon.ForEach((v) => { vertices.Add(ConvertUnits.ToSimUnits(v)); });
            float area = GeometryFunctions.GetSignedArea(vertices);

            if (area < 0)
                vertices.Reverse();

            Vector2 position = GeometryFunctions.GetCentroid(vertices);

            GameObject.Position = ConvertUnits.ToDisplayUnits(position);

            Polygon = vertices;

            if (IsSensor)
            {
                _physics = new Sensor(GameObject.Scene.WorldInfo, ConvertUnits.ToSimUnits(GameObject.Position), Polygon);
            }
            else
            {
                if (IsStatic)
                {
                    _physics = new StaticObject(GameObject.Scene.WorldInfo, ConvertUnits.ToSimUnits(GameObject.Position), Mass, Friction, Restitution, Polygon);
                }
                else
                {
                    var dynamicObject = new DynamicObject(GameObject.Scene.WorldInfo, ConvertUnits.ToSimUnits(GameObject.Position), Mass, Friction, Restitution, Polygon);
                    dynamicObject.IgnoreGravity = IgnoreGravity;

                    _physics = dynamicObject;
                }
            }

            _physics.IsProjectile = IsProjectile;
            _physics.IsEthereal = IsEthereal;

            // Hook up collision callbacks
            _physics.OnCollision += (A, B, p) => 
            {
                if (A == _physics)
                {
                    if (OnCollision != null)
                        OnCollision(B.GameObject);

                    if (B.GameObject != null)
                    {
                        if (OnCollisionEnter != null && !_collidingGameObjects.ContainsKey(B.GameObject))
                        {
                            OnCollisionEnter(B.GameObject);
                        }

                        if (OnCollisionEnter != null || OnCollisionLeave != null)
                            _collidingGameObjects[B.GameObject] = true;
                    }
                }
                else if (B == _physics)
                {
                    if (OnCollision != null)
                        OnCollision(A.GameObject);

                    if (A.GameObject != null)
                    {
                        if (OnCollisionEnter != null && !_collidingGameObjects.ContainsKey(A.GameObject))
                        {
                            OnCollisionEnter(A.GameObject);
                        }

                        if (OnCollisionEnter != null || OnCollisionLeave != null)
                            _collidingGameObjects[A.GameObject] = true;
                    }
                }

                // If we collide with object subscripe to pre update event.
                if (_preUpdateEventRegistered == false && ((_collidingGameObjects.Count != 0 && (OnCollisionEnter != null || OnCollisionLeave != null)) || StorePreviousVelocity))
                {
                    _preUpdateEventRegistered = true;
                    GameObject.Scene.PreUpdate += preUpdate;
                }
            };

            // Set reset info.
            if (IsStatic == false && IsProjectile == false)
            {
                _startPosition = GameObject.Position;
                SceneInfo.DynamicPhysicObjects.Add(this);
            }

            if (_pendingGameObject != null)
                _physics.GameObject = _pendingGameObject;
        }

        public override void Update(GameTime gameTime)
        {
            // Update the game objects position.
            if (_physics.IsStatic == false && _physics.IsSensor == false)
                GameObject.Position = ConvertUnits.ToDisplayUnits(_physics.Position);

            // Check if some objects have left the collision
            foreach (var pair in _collidingGameObjects)
            {
                if (pair.Value == false)
                {
                    if (OnCollisionLeave != null)
                        OnCollisionLeave(pair.Key);

                    _removeCollidingObjects.Add(pair.Key);
                }
            }

            _removeCollidingObjects.ForEach((key) => { _collidingGameObjects.Remove(key); });
            _removeCollidingObjects.Clear();

            // Unsubscribe if there is no colliding object
            if (_collidingGameObjects.Count == 0 && _preUpdateEventRegistered && StorePreviousVelocity == false)
            {
                _preUpdateEventRegistered = false;
                GameObject.Scene.PreUpdate -= preUpdate;
            }
        }

        public void AddGameObject(GameObject go)
        {
            if (_physics == null)
                _pendingGameObject = go;
            else
                _physics.GameObject = go;
        }

        /// <summary>
        /// Moves the physics object to the specified position -> Changing position of the owning game object
        /// won't change the position becuase it is controlled by the physics engine.
        /// </summary>
        /// <param name="position">Position to move the object to</param>
        public void MoveTo(Vector2 position)
        {
            if (_physics != null)
            {
                _physics.Position = ConvertUnits.ToSimUnits(position);
                GameObject.Position = position;
            }
        }

        public void ApplyForce(Vector2 force)
        {
            if (_physics.IsStatic == false)
            {
                _physics.AddForce(force);
            }
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (_physics.IsStatic == false)
            {
                _physics.Velocity = velocity;
            }
        }

        public Vector2 GetVelocity()
        {
            return _physics.Velocity;
        }

        public void ChangeMotionState(Pontification.Physics.DynamicObject.MotionStates state)
        {
            var dyn = _physics as DynamicObject;
            if (dyn != null)
                dyn.MotionState = state;
        }

        public void AddVelocity(Vector2 velocity)
        {
            if (_physics.IsStatic == false)
            {
                _physics.Velocity += velocity;
            }
        }

        public void ResetPhysicObject()
        {
            if (IsStatic == false && IsProjectile == false)
            {
                MoveTo(_startPosition);
            }
        }

        public Vector2[] GetPolygon()
        {
            Vector2[] vertices;
            (_physics.CollisionShape as Polygon).GetVertices(out vertices);

            return vertices;
        }
        #endregion

        #region Protected methods
        protected override void disposing()
        {
            if (_preUpdateEventRegistered)
                GameObject.Scene.PreUpdate -= preUpdate;

            _physics.Dispense();
            _physics = null;
        }
        #endregion

        #region Private methods
        private void preUpdate(object sender, EventArgs e)
        {
            if (_physics != null)
                PreviousVelocity = _physics.Velocity;
            // Set all colliding objects to false
            foreach (var key in _collidingGameObjects.Keys.ToList())
            {
                _collidingGameObjects[key] = false;
            }
        }
        #endregion
    }
}
