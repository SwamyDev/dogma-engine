using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Pontification.Monitoring
{
    public class PerformancePanel
    {
        private static readonly PerformancePanel _instance = new PerformancePanel();
        public static PerformancePanel Instance { get { return _instance; } }

        #region Private attributes
        private SpriteFont _font;
        private Rectangle _cpuOnlyRect;
        private Rectangle _ramOnlyRect;
        private Rectangle _bothOnlyRect;
        private Vector2 _margin = new Vector2(10, 3);
        private Vector2 _paragraph = new Vector2(0, 18);
        private Color _panelColor = new Color(10, 10, 100, 127);
        #endregion

        #region Public properties
        public Vector2 Offset { get; set; }
        public bool ShowCPUUsage { get; set; }
        public bool ShowRAMUsage { get; set; }
        #endregion

        private PerformancePanel()
        {
            Offset = new Vector2(20, 20);
            _cpuOnlyRect = new Rectangle((int)Offset.X, (int)Offset.Y, 402, 25);
            _ramOnlyRect = new Rectangle((int)Offset.X, (int)Offset.Y, 402, 60);
            _bothOnlyRect = new Rectangle((int)Offset.X, (int)Offset.Y, 402, 80);
        }

        [Conditional("DEBUG")]
        public void LoadContent(ContentManager cm)
        {
            _font = cm.Load<SpriteFont>("Monitoring/UbuntuMonoR");
        }

        [Conditional("DEBUG")]
        public void Draw(SpriteBatch sb)
        {
            if (ShowCPUUsage && !ShowRAMUsage)  // CPU only
            {
                sb.Begin();
                Primitives.Instance.DrawBox(sb, _cpuOnlyRect, _panelColor);
                drawCPUInfo(sb, Vector2.Zero);
                sb.End();
            }
            else if (!ShowCPUUsage && ShowRAMUsage) // RAM only
            {
                sb.Begin();
                Primitives.Instance.DrawBox(sb, _ramOnlyRect, _panelColor);
                drawRAMInfo(sb, Vector2.Zero);
                sb.End();
            }
            else if (ShowCPUUsage && ShowRAMUsage) // Show both
            {
                sb.Begin();
                Primitives.Instance.DrawBox(sb, _bothOnlyRect, _panelColor);
                drawCPUInfo(sb, Vector2.Zero);
                drawRAMInfo(sb, _paragraph);
                sb.End();
            }
        }

        private void drawCPUInfo(SpriteBatch sb, Vector2 padding)
        {
            int percValue = (int)Logger.Instance.CPUUsage;
            sb.DrawString(_font, string.Format("CPU usage: {0}%", percValue.ToString()), Offset + _margin + padding, Color.White);
        }

        private void drawRAMInfo(SpriteBatch sb, Vector2 padding)
        {
            int processMB = (int)(Logger.Instance.CurrentProcessMemoryUsage / (1024 * 1024));
            int managedMB = (int)(Logger.Instance.CurrentAllocatedManagedMemory / (1024 * 1024));
            int availableMB = (int)Logger.Instance.RAMavailable;

            sb.DrawString(_font, string.Format("Memory allocated by the current process: {0}MB", processMB.ToString()), Offset + _margin + padding, Color.White);
            sb.DrawString(_font, string.Format("Memory allocated in managed memory: {0}MB", managedMB.ToString()), Offset + _margin + padding + _paragraph, Color.White);
            sb.DrawString(_font, string.Format("Available RAM: {0}MB", availableMB.ToString()), Offset + _margin + padding + 2 * _paragraph, Color.White);
        }
    }
}
