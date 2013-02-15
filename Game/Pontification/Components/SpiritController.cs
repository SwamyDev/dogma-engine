using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.AI;
using Pontification.Interfaces;

namespace Pontification.Components
{
    public class SpiritController : Component, IController
    {
        private SpiritPhysicsComponent _physics;
        private AbilityManager _abilities;
        private CharacterStats _stats;
        private AIMemorySheet _memory;
        private SpiritTree _behaviourTree;
        private Vector2 _direction;
        private float _memoryBuffer = 2f;
        private float _visionRange = 700.0f;
        private float _visionAngle = 360.0f;
        private float _chaseTime = 3f;
        private float _timeAsSpirit;
        private bool _movementLocked;

        public AIMemorySheet Memory { get { return _memory; } }

        public override void Start()
        {
            _physics = GetComponent<SpiritPhysicsComponent>();

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
            _behaviourTree = new SpiritTree(this);

            Pontification.SceneManagement.SceneInfo.AIEnteties.Add(this);

            // Get ability manager.
            _abilities = GetComponent<AbilityManager>();
            if (_abilities == null)
                Scheduler.Instance.AddTask(addAbilityManager());
        }

        public override void Update(GameTime gameTime)
        {
            if (_movementLocked == false)
            {
                SendMessage("Fly", new object[] { _direction });
            }

            if (_abilities != null)
            {
                _abilities.ResetAbilityState();
                _memory.CurrentPrimary = _abilities.ActivePrimary;
            }

            // Update AI memory sheet.
            _memory.Position = GameObject.Position;
            _memory.Velocity = _physics.GetVelocity();

            // Update possible enemies within the vision range. The behaviour tree will figure out the details.
            _memory.Enemies.Clear();
            var player = Pontification.SceneManagement.SceneInfo.Player;
            if ((player.Position - GameObject.Position).Length() <= _visionRange)
                _memory.Enemies.Add(player);

            // Update behaviour tree.
            //_movementLocked = false;
            _behaviourTree.Tick();
        }

        public void TakeDamage(float amount)
        {
            // Update memory sheet.
            _memory.Health = _stats.Health;
        }

        public void LockMovement()
        {
            _movementLocked = true;
        }

        public void UnlockMovement()
        {
            _movementLocked = false;
        }

        #region IController methods
        public void SetTarget(GameObject go)
        {
            _memory.Target = go;
        }

        public void Attack()
        {
            if (_memory.Target == null || _abilities == null)
                return;

            if (_physics.Form == SpiritForm.SF_BALL)
                _abilities.UsePrimary();
            if (_physics.Form == SpiritForm.SF_SPIRIT)
            {
                if (_timeAsSpirit >= _chaseTime)
                    _abilities.UseSecondary();
                else
                    _timeAsSpirit += Time.DeltaTime;
            }
            else
            {
                _timeAsSpirit = 0;
            }

            _direction = _memory.Target.Position - GameObject.Position;
        }

        public void Walk(int direction)
        {
        }

        public void Jump()
        {
        }

        public void ChangeFacing(int direction)
        {
        }
        #endregion

        protected override void disposing()
        {
            Pontification.SceneManagement.SceneInfo.AIEnteties.Remove(this);
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
