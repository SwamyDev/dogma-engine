using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Interfaces;
using Pontification.Components;

namespace Pontification.SceneManagement
{
    public struct ResetInfo
    {
        public CharacterCategory Category;
        public Layer Layer;
        public Scene Scene;
        public Vector2 Position;
        public string Name;
        public int Facing;
        public bool IsPlayer;
    }
    /// <summary>
    /// Contains information about the current scene like, how many spirits, priests or guards there,
    /// a references to the player etc... . All sorts of info that the AI needs to make decisions.
    /// </summary>
    public static class SceneInfo
    {
        public static GameObject Player;
        public static GameObject Cursor;
        public static GameObject CurrentCheckpoint;
        public static List<ResetInfo> ResetInfos = new List<ResetInfo>();
        public static Bag<CursorSensitive> CursorSensitives = new Bag<CursorSensitive>();
        public static Bag<PhysicsComponent> DynamicPhysicObjects = new Bag<PhysicsComponent>();
        public static Bag<MoveAlongPath> MovingPlatforms = new Bag<MoveAlongPath>();
        public static Bag<IController> AIEnteties = new Bag<IController>();
        public static Bag<GameObject> Spirits = new Bag<GameObject>();
        public static Bag<GameObject> Priests = new Bag<GameObject>();
        public static Bag<GameObject> Guards = new Bag<GameObject>();
        public static bool ResetInfoSealed;

        public static void Reset()
        {
            // Remove all enemies.
            Spirits.ForEach((spirit) => { resetCharacter(spirit); });
            Priests.ForEach((priest) => { resetCharacter(priest); });
            Guards.ForEach((guard) => { resetCharacter(guard); });

            Spirits.Clear();
            Priests.Clear();
            Guards.Clear();

            Spirits.Add(Player);

            // Respawn characers.
            int idx = 0;
            ResetInfos.ForEach((resetInfo) => 
            {
                if (resetInfo.IsPlayer)
                {
                    if (CurrentCheckpoint == null)
                        Player.SendMessage("Teleport", new object[] { resetInfo.Position });
                    else
                        Player.SendMessage("Teleport", new object[] { CurrentCheckpoint.Position });
                }
                else
                {
                    var gameObject = new GameObject(resetInfo.Name, resetInfo.Position, resetInfo.Scene, resetInfo.Layer);
                    if (resetInfo.Category == CharacterCategory.CC_GUARD)
                    {
                        var guardStart = gameObject.AddComponent<GuardStart>();
                        guardStart.Facing = resetInfo.Facing;
                    }
                    if (resetInfo.Category == CharacterCategory.CC_PRIEST)
                    {
                        var priestStart = gameObject.AddComponent<PriestStart>();
                        priestStart.Facing = resetInfo.Facing;
                    }
                    if (resetInfo.Category == CharacterCategory.CC_SPIRIT)
                    {
                        var spiritStart = gameObject.AddComponent<SpiritStart>();
                        spiritStart.Facing = resetInfo.Facing;
                    }
                    gameObject.Awake();
                    gameObject.GetComponent<CharacterStats>().ResetInfoIdx = idx;

                    gameObject.Start();
                }
                idx++;
            });

            // Reset platforms.
            MovingPlatforms.ForEach((platform) => 
            { 
                platform.ResetPath();
                platform.GameObject.SendMessage("ResetLiftController");
            });

            // Reset physic objects.
            DynamicPhysicObjects.ForEach((po) => { po.ResetPhysicObject(); });

            // Bring cursor to the front.
            Cursor.ToFront();
        }

        private static void resetCharacter(GameObject go)
        {
            if (go == Player)
            {
                go.SendMessage("ResetToDrainer");
                go.SendMessage("ResetStats");
                go.SendMessage("SetVelocity", new object[] { Vector2.Zero });
                go.SendMessage("ResetEffects");
            }
            else
            {
                go.Dispose();
            }
        }
    }
}
