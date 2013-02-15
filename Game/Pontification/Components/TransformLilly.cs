using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class TransformLilly : Component
    {
        private PhysicsComponent _sensor;
        private Vector2 _targetPosition;
        private float _moveSpeed = 300.0f;
        private bool _moveToTarget;
        private bool _triggered;

        public GameObject Target { get; set; }

        public override void Start()
        {
            _sensor = GetComponent<PhysicsComponent>();
            _sensor.OnCollisionEnter += onEnter;
            _targetPosition = Target.Position;
        }

        public override void Update(GameTime gameTime)
        {
            if (_moveToTarget)
            {
                Vector2 difference = _targetPosition - Cursor.Instance.GameObject.Position;

                if (difference.Length() > 10)
                {
                    var velocity = Vector2.Normalize(difference) * _moveSpeed;
                    Cursor.Instance.GameObject.Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Cursor.Instance.GameObject.Position = _targetPosition + Vector2.UnitY * 20;
                    Cursor.Instance.SwitchAnimationComponents();
                    Cursor.Instance.SendMessage("SetAnimation", new object[] { "to_human" });
                    _moveToTarget = false;
                    Scheduler.Instance.AddTask(transformBack());
                }
            }
        }

        private System.Collections.IEnumerator transformBack()
        {
            yield return TimeSpan.FromSeconds(3f);
            Cursor.Instance.SendMessage("SetAnimation", new object[] { "to_ball" });
            yield return TimeSpan.FromSeconds(1.16f);
            Cursor.Instance.SwitchAnimationComponents();
            Cursor.Locked = false;
            SceneManagement.SceneInfo.Player.SendMessage("UnlockInput");
        }

        private void onEnter(GameObject go)
        {
            if (go == SceneManagement.SceneInfo.Player && _triggered == false)
            {
                go.SendMessage("LockInput");
                Cursor.Locked = true;
                _moveToTarget = true;
                _triggered = true;
            }
        }
    }
}
