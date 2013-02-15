using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.AI;
using Pontification.Interfaces;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class GuardController : Component, IController
    {
        #region Private attributes
        private CharacterPhysicsComponent _physics;
        private AbilityManager _abilities;
        private GuardTree _behaviourTree;
        private CharacterStats _stats;
        private AIMemorySheet _memory = new AIMemorySheet();
        private float _memoryBuffer = 2f;
        private float _visionRange = 700.0f;
        private float _visionAngle = 90.0f;
        private int _direction;
        private bool _wantToJump;
        private bool _movementLocked;
        #endregion

        #region Public properties
        public AIMemorySheet Memory { get { return _memory; } }
        #endregion

        #region Public methods
        public override void Start()
        {
            _physics = GetComponent<CharacterPhysicsComponent>();

            // Populate memory sheet.
            _stats = GetComponent<CharacterStats>();
            _memory.Health = _stats.Health;
            _memory.MemoryBuffer = _memoryBuffer;
            _memory.VisionRange = _visionRange;
            _memory.VisionAngle = _visionAngle;
            _memory.Category = _stats.Category;
            _memory.Enemies = new List<GameObject>();
            _memory.Facing = 1;

            // Create behaviour tree.
            _behaviourTree = new GuardTree(this);

            SceneInfo.AIEnteties.Add(this);

            // Get ability manager.
            _abilities = GetComponent<AbilityManager>();
            if (_abilities == null)
                Scheduler.Instance.AddTask(addAbilityManager());
        }

        public override void Update(GameTime gameTime)
        {
            if (!_movementLocked)
            {
                SendMessage("Walk", new object[] { _direction });

                if (_wantToJump)
                {
                    SendMessage("Jump");
                    _wantToJump = false;
                }
            }

            if (_abilities != null)
            {
                _abilities.ResetAbilityState();
                _memory.CurrentPrimary = _abilities.ActivePrimary;
            }

            // Update AI memory sheet.
            _memory.Position = GameObject.Position;
            _memory.Velocity = _physics.Velocity;

            if (_memory.StandingJumpVelocity == Vector2.Zero)
                _memory.StandingJumpVelocity = _physics.StandingJumpVelocity;
            if (_memory.RunningJumpVelocity == Vector2.Zero)
                _memory.RunningJumpVelocity = _physics.RunningJumpVelocity;

            if (_physics != null)
                _memory.Facing = _physics.Facing;

            // Update possible enemies within the vision range. The behaviour tree will figure out the details.
            _memory.Enemies.Clear();
            if (SceneInfo.Player != null)
            {
                var playerStats = SceneInfo.Player.GetComponent<CharacterStats>();
                if ((SceneInfo.Player.Position - GameObject.Position).Length() < _visionRange &&
                    (playerStats != null && (playerStats.Category == CharacterCategory.CC_PRIEST || playerStats.Category == CharacterCategory.CC_GUARD)))
                    _memory.Enemies.Add(SceneInfo.Player);
            }


            // Update behaviour tree.
            _movementLocked = false;
            _behaviourTree.Tick();
        }

        public void TakeDamage(float amount, DamageTypes type)
        {
            // Update memory sheet.
            _memory.Health = _stats.Health;
        }

        public void DeactivateAI()
        {
            IsActive = false;
        }

        public void LockMovement()
        {
            _movementLocked = true;
            SendMessage("Walk", new object[] { 0 });
        }

        public void UnlockMovement()
        {
            _movementLocked = false;
        }
        #endregion

        #region IController methods
        public void SetTarget(GameObject go)
        {
            _memory.Target = go;
        }

        public void Attack()
        {
            if (_memory.Target == null)
                return;

            Vector2 start = GameObject.Position + _physics.ViewPoint;
            Vector2 end = _memory.Target.Position;
            var direction = Vector2.Normalize((end - start));

            _physics.ViewRotation = (float)Math.Atan2(direction.Y, direction.X);

            if (_abilities != null)
                _abilities.UsePrimary();
        }

        public void Walk(int direction)
        {
            _direction = direction;
        }


        public void Jump()
        {
            _wantToJump = true;
        }

        public void ChangeFacing(int direction)
        {
            _physics.ChangeFacing(direction);
        }
        #endregion

        protected override void disposing()
        {
            SceneInfo.AIEnteties.Remove(this);
        }

        #region Private methods
        System.Collections.IEnumerator addAbilityManager()
        {
            yield return TimeSpan.FromSeconds(0.1f);
            _abilities = GetComponent<AbilityManager>();
        }
        #endregion
    }
}
