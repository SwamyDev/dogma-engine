using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pontification.AI;
using Pontification.Components;
using Pontification.SceneManagement;

namespace Pontification.Monitoring
{
    public enum DebugDisplays
    {
        NONE = 0x0,
        CPU_USAGE = 0x1,
        RAM_USAGE = 0x2,
        PHYSICS = 0x4,
        GRID = 0x8,
        AI = 0x10
    }
    /// <summary>
    /// Provides different flags to display certain animation like the grid, current memory usage etc...
    /// Also provides a shorthand for the Logger's Log method.
    /// </summary>
    public class Debug
    {
        private static readonly Debug _instance = new Debug();
        public static Debug Instance { get { return _instance; } }

        /// <summary>
        /// Use the | operator to add multiple flags
        /// </summary>
        public static DebugDisplays DisplayOptions { get; set; }

        private Debug()
        {
        }


        [Conditional("DEBUG")]
        public static void Log(object msg)
        {
            Log(msg.ToString());
        }

        [Conditional("DEBUG")]
        public static void Log(string msg)
        {
            Logger.Instance.Log(msg, MessageType.MT_DEBUG);
        }

        public void LoadContent(ContentManager cm)
        {
            PerformancePanel.Instance.LoadContent(cm);
        }

        public void Draw(SpriteBatch sb)
        {
            // Performance
            var showPerformancePanel = false;
            PerformancePanel.Instance.ShowCPUUsage = false;
            PerformancePanel.Instance.ShowRAMUsage = false;
            if ((DisplayOptions & DebugDisplays.CPU_USAGE) == DebugDisplays.CPU_USAGE)
            {
                PerformancePanel.Instance.ShowCPUUsage = true;
                showPerformancePanel = true;
            }
            if ((DisplayOptions & DebugDisplays.RAM_USAGE) == DebugDisplays.RAM_USAGE)
            {
                PerformancePanel.Instance.ShowRAMUsage = true;
                showPerformancePanel = true;
            }
            if (showPerformancePanel)
            {
                PerformancePanel.Instance.Draw(sb);
            }

            // Physics
            if ((DisplayOptions & DebugDisplays.PHYSICS) == DebugDisplays.PHYSICS)
            {
                if (SceneManager.Instance != null && SceneManager.Instance.FocusScene != null)
                    SceneManager.Instance.FocusScene.WorldInfo.Draw(sb);
            }

            // Grid
            if ((DisplayOptions & DebugDisplays.GRID) == DebugDisplays.GRID)
                DrawGrid(sb);

            // AI
            if ((DisplayOptions & DebugDisplays.AI) == DebugDisplays.AI)
                DrawAI(sb);
        }

        private void DrawGrid(SpriteBatch sb)
        {
            int cellSize = Game.CellSize;
            int max = Game.NumberOfGridLines / 2;

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.NormalView);
            for (int x = 0; x <= max; x++)
            {
                var start = new Vector2(x, -max) * cellSize;
                var end = new Vector2(x, max) * cellSize;
                Primitives.Instance.DrawLine(sb, start, end, Color.White, 1);

                start = new Vector2(-x, -max) * cellSize;
                end = new Vector2(-x, max) * cellSize;
                Primitives.Instance.DrawLine(sb, start, end, Color.White, 1);
            }
            for (int y = 0; y <= max; y++)
            {
                var start = new Vector2(-max, y) * cellSize;
                var end = new Vector2(max, y) * cellSize;
                Primitives.Instance.DrawLine(sb, start, end, Color.White, 1);

                start = new Vector2(-max, -y) * cellSize;
                end = new Vector2(max, -y) * cellSize;
                Primitives.Instance.DrawLine(sb, start, end, Color.White, 1);
            }
            sb.End();
        }

        private void DrawAI(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.NormalView);
            SceneInfo.AIEnteties.ForEach((ai) => 
            {
                var memory = ai.Memory;

                Vector2 start = memory.Position;
                float yOffset = memory.VisionRange * (float)Math.Sin((memory.VisionAngle / 2f) * (MathHelper.Pi / 180));
                float xOffset = memory.VisionRange * memory.Facing * (float)Math.Cos((memory.VisionAngle / 2f) * (MathHelper.Pi / 180));
                var end1 = start + Vector2.UnitX * xOffset + Vector2.UnitY * yOffset;
                var end2 = start + Vector2.UnitX * xOffset - Vector2.UnitY * yOffset;

                Primitives.Instance.DrawLine(sb, start, end1, Color.Azure, 2);
                Primitives.Instance.DrawLine(sb, start, end2, Color.Azure, 2);

                if (memory.Target != null)
                    Primitives.Instance.DrawPoint(sb, memory.Target.Position, Color.Red, 16);
            });
            sb.End();
        }
    }
}
