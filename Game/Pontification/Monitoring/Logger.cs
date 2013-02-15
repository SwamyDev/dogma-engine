using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Pontification.Monitoring
{
    public enum MessageType
    {
        MT_STATISTICS,
        MT_DEBUG,
        MT_ERROR,
        MT_EXCEPTION
    }
    /// <summary>
    /// Distributes messages through the event system to all classes which sign up
    /// for the LogHandle event. Also updates current CPU and Memory usage.
    /// </summary>
    public class Logger
    {
        private static readonly Logger _instance = new Logger();
        public static Logger Instance { get { return _instance; } }

        #region Private attributes
        private System.Diagnostics.PerformanceCounter _cpuCounter;
        private System.Diagnostics.PerformanceCounter _ramCounter;
        private System.Diagnostics.Process _currentProcess;
        private float _elapsedTime;
        #endregion

        #region Public properties
        public event EventHandler<LoggerEventArgs> LogHandle;
        public float CPUUsage { get; private set; }
        public long CurrentProcessMemoryUsage { get; private set; }
        public long CurrentAllocatedManagedMemory { get; private set; }
        public float RAMavailable { get; private set; }
        #endregion

        private Logger()
        {
            _cpuCounter = new System.Diagnostics.PerformanceCounter(
                "Processor",
                "% Processor Time",
                "_Total",
                true);

            _ramCounter = new System.Diagnostics.PerformanceCounter(
                "Memory",
                "Available MBytes",
                true);

            _currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        }

        [Conditional("DEBUG")]
        public void Log(string msg, MessageType type)
        {
            // Use local variable to avoid problems with mutlithread access.
            EventHandler<LoggerEventArgs> logHandle = LogHandle;
            if (logHandle != null)
            {
                logHandle(this, new LoggerEventArgs(msg, type));
            }
        }
        
        [Conditional("DEBUG")]
        public void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= 1.0f)
            {
                CPUUsage = _cpuCounter.NextValue();
                RAMavailable = _ramCounter.NextValue();
                _elapsedTime = 0;
            }
            CurrentProcessMemoryUsage = _currentProcess.PrivateMemorySize64;
            CurrentAllocatedManagedMemory = GC.GetTotalMemory(false);
        }
    }

    public class LoggerEventArgs : EventArgs
    {
        public MessageType Type { get; private set; }
        public string Message { get; private set; }

        public LoggerEventArgs(string msg, MessageType type)
        {
            Message = msg;
            Type = type;
        }
    }
}
