using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.Monitoring;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public enum CharacterCategory
    {
        CC_NONE,
        CC_SPIRIT,
        CC_GUARD,
        CC_PRIEST
    }
    public class CharacterStats : Component
    {
        #region Private attributes
        private float _resetHealth;
        #endregion

        #region Public attributes
        public int ResetInfoIdx;
        #endregion

        #region Public properties
        public float Health { get; set; }
        public CharacterCategory Category { get; set; }
        #endregion

        public CharacterStats()
        {
            Health = 100.0f;
        }

        #region Public methods
        public override void Start()
        {
            // Add game object to AI SceneInfo class.

            if (Category == CharacterCategory.CC_SPIRIT)
            {
                SceneInfo.Spirits.Add(GameObject);
            }

            if (Category == CharacterCategory.CC_PRIEST)
            {
                SceneInfo.Priests.Add(GameObject);
            }

            if (Category == CharacterCategory.CC_GUARD)
            {
                SceneInfo.Guards.Add(GameObject);
            }

            if (SceneInfo.ResetInfoSealed == false)
            {
                var resetInfo = new ResetInfo();
                resetInfo.Category = Category;
                resetInfo.Layer = GameObject.Layer;
                resetInfo.Scene = GameObject.Scene;
                resetInfo.Position = GameObject.Position;
                resetInfo.Name = GameObject.Name;
                resetInfo.Facing = GetComponent<AnimationComponent>().Facing;
                resetInfo.IsPlayer = GetComponent<PlayerInput>() != null;
                SceneInfo.ResetInfos.Add(resetInfo);
                ResetInfoIdx = SceneInfo.ResetInfos.Count - 1;

                _resetHealth = Health;
            }
        }

        public void UpdateStartFacing(int direction)
        {
            var info = SceneInfo.ResetInfos[ResetInfoIdx];
            info.Facing = direction;
            SceneInfo.ResetInfos[ResetInfoIdx] = info;

            if (ResetInfoIdx == 0)
                Console.WriteLine("Priest reset info.");
        }

        public void TakeDamage(float amount, DamageTypes type)
        {
            Console.WriteLine(string.Format("{0} takes {1} damage.", GameObject, amount));
            Health -= amount;

            if (Health <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            if (SceneInfo.Player == GameObject)
            {
                if (Category == CharacterCategory.CC_SPIRIT)
                {
                    SceneInfo.Reset();
                    Logger.Instance.Log("Reset player", MessageType.MT_STATISTICS);
                }
                else
                {
                    SendMessage("ResetToDrainer");
                    removeFromWorld();
                }
            }
            else
            {
                removeFromWorld();
            }
        }

        public void ResetStats()
        {
            Health = _resetHealth;
        }
        #endregion

        protected override void disposing()
        {
            // Clean AI SceneInfo class when this GameObject get's destroyed.
            if (Category == CharacterCategory.CC_SPIRIT)
            {
                Pontification.SceneManagement.SceneInfo.Spirits.Remove(GameObject);
            }

            if (Category == CharacterCategory.CC_PRIEST)
            {
                Pontification.SceneManagement.SceneInfo.Priests.Remove(GameObject);
            }

            if (Category == CharacterCategory.CC_GUARD)
            {
                Pontification.SceneManagement.SceneInfo.Guards.Remove(GameObject);
            }
        }

        #region Private methods
        private void removeFromWorld()
        {
            GameObject.IsActive = false;
            GameObject.Dispose();
        }
        #endregion
    }
}
