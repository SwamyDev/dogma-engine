using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Interfaces;

namespace Pontification.Components
{
    public class LifeDrainAbility : Component, IAbility
    {
        #region Private attributes
        private AbilityManager _manager;
        private CharacterStats _ownerStats;
        private CharacterStats _drainerStats;
        private GameObject _drainer;
        private DamageTypes _damageType = DamageTypes.DT_SPIRIT;
        #endregion

        #region Public properties
        public Vector2 Offset { get; set; }
        public float Range { get; set; }
        public float DrainSpeed { get; set; }
        #endregion

        public LifeDrainAbility()
        {
            DrainSpeed = 1f;
        }

        #region Public methods
        public override void Start()
        {
            _manager = GetComponent<AbilityManager>();

            // If not present in the current game object add it.
            if (_manager == null)
                _manager = AddComponent<AbilityManager>();

            // Add ability to manager
            _manager.AddAbility(this, true);

            _ownerStats = GetComponent<CharacterStats>();
        }

        public override void Update(GameTime gameTime)
        {
            GetComponents<AnimationComponent>().ForEach((anim) => 
            {
                if (anim.Alpha < 1f)
                {
                    anim.Alpha += 0.001f;
                }
                else
                {
                    anim.Alpha = 1f;
                }
            });
        }

        public void SetDrainer(GameObject drainer)
        {
            _drainer = drainer;
            _drainerStats = _drainer.GetComponent<CharacterStats>();
        }

        public GameObject GetDrainer()
        {
            return _drainer;
        }

        public override string ToString()
        {
            return "LifeDrainAbility";
        }

        public void ResetToDrainer()
        {
            _drainer.MoveComponent(GetComponent<PlayerInput>());
            _drainer.MoveComponent(GetComponent<Camera>());
            _drainer.IsActive = true;
            _drainer.SendMessage("SetCharacterPhysicsActive", new object[] { true });
            _drainer.SendMessage("Teleport", new object[] { GameObject.Position });

            if (GameObject == SceneManagement.SceneInfo.Player)
                SceneManagement.SceneInfo.Player = _drainer;
        }
        #endregion

        #region IAbility methods
        public void Use()
        {
            if (_drainer != null)
            {
                _manager.Lock();
                GetComponents<AnimationComponent>().ForEach((anim) => 
                {
                    if (anim.Sprite.AnimationDictionary.ContainsKey("Decay") == false)
                        anim.IsActive = false;
                });
                SendMessage("LockInput");
                Scheduler.Instance.AddTask(finishedDecay());
            }
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void ResetTimer()
        {
        }
        #endregion

        #region Private methods
        private System.Collections.IEnumerator finishedDecay()
        {
            yield return TimeSpan.FromSeconds(DrainSpeed);
            if (_drainerStats != null && _ownerStats != null)
            {
                _drainerStats.Health += _ownerStats.Health;
            }

            _manager.Unlock();

            // "Pop out" drainer.
            ResetToDrainer();

            _drainer.SendMessage("PlayCinematicAnimation", new object[] { "ConsumeEnergy" });

            // Remove drained.
            GameObject.Dispose();
        }
        #endregion
    }
}
