using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.Components;
using Pontification.SceneManagement;
using System.Diagnostics;

namespace Pontification.Monitoring
{
    /// <summary>
    /// NOTE: Use statistics for replays and stuff by feeding it back to the engine. Might re-use for squadron commander in the future.
    /// </summary>
    public class PlayerStatistics
    {
        private static readonly PlayerStatistics _instance = new PlayerStatistics();
        public static PlayerStatistics Instance { get { return _instance; } }

        #region Private attributes
        private CharacterStats _currentStats;
        private StreamWriter _fileStream;
        private GameTime _lastGameTime;
        private CharacterCategory _oldType;
        private Vector2 _oldPlayerPosition;
        private string _folderName = "Logs";
        private string _currentAbility = "None";
        private float _updatePositionTime = 0.1f;
        private float _oldHealth;
        private float _elapsedTime;
        private int _dialogs;
        private int _resets;
        private int _jumps;
        private bool _dialogTriggered;
        private bool _reseted;
        private bool _jumped;
        #endregion

        #region Log format attributes
        private int _timeWidth = 10;
        private int _typeWidth = 10;
        private int _xPosWidth = 7;
        private int _yPosWidth = 7;
        private int _healthWidth = 6;
        private int _jumpedWidth = 6;
        private int _usingAbilityWidth = 20;
        private int _checkpointWidth = 10;
        private int _resetWidth = 5;
        #endregion

        public PlayerStatistics()
        {
            // Check for log folder.
            if (!Directory.Exists(_folderName))
                Directory.CreateDirectory(_folderName);

            // Create log file in folder.
            _fileStream = new StreamWriter(string.Format("{0}/PlayerStats_{1}.csv", _folderName, DateTime.Now.ToString("dd-mm-yy_HH-mm-ss")));
            createHeader();

            // Hook up to log event
            Logger.Instance.LogHandle += new EventHandler<LoggerEventArgs>(logMessage);
        }

        #region Public methods
        [Conditional("DEBUG")]
        public void Update(GameTime gameTime)
        {
            _lastGameTime = gameTime;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;

            _elapsedTime += deltaTime;

            if (_elapsedTime >= _updatePositionTime)
            {
                if (SceneInfo.Player != null)
                {
                    _currentStats = SceneInfo.Player.GetComponent<CharacterStats>();
                    Vector2 newPlayerPos = SceneInfo.Player.Position;
                    if (_oldPlayerPosition != newPlayerPos)
                    {
                        logPlayerStatus(totalTime);
                    }
                    else if (_oldHealth != _currentStats.Health)
                    {
                        logPlayerStatus(totalTime);
                    }
                    else if (_oldType != _currentStats.Category)
                    {
                        logPlayerStatus(totalTime);
                    }
                    _oldPlayerPosition = newPlayerPos;
                    _oldHealth = _currentStats.Health;
                    _oldType = _currentStats.Category;
                }
                _elapsedTime = 0;
            }
        }

        [Conditional("DEBUG")]
        public void Unload()
        {
            createFooter();
            _fileStream.Flush();
            _fileStream.Close();
            _fileStream.Dispose();
        }
        #endregion

        #region Private methods
        private void logMessage(object sender, LoggerEventArgs e)
        {
            if (e.Type == MessageType.MT_STATISTICS)
            {
                if (e.Message == "Reset player")
                {
                    _resets++;
                    _reseted = true;
                }
                else if (e.Message == "Jumped")
                {
                    _jumps++;
                    _jumped = true;
                }
                else if (e.Message == "Dialog triggered")
                {
                    _dialogs++;
                    _dialogTriggered = true;
                }
                else
                {
                    string[] strParams = e.Message.Split('_');
                    if (strParams.Length == 2 && strParams[0] == "Ability")
                    {
                        _currentAbility = strParams[1];
                    }
                }

                logPlayerStatus((float)_lastGameTime.TotalGameTime.TotalSeconds);
            }
        }

        private void logPlayerStatus(float totalGameTime)
        {
            int xPos = (int)SceneInfo.Player.Position.X;
            int yPos = (int)SceneInfo.Player.Position.Y;
            int health = (int)_currentStats.Health;
            string checkpoint = "None";
            if (SceneInfo.CurrentCheckpoint != null)
                checkpoint = SceneInfo.CurrentCheckpoint.GetComponent<Checkpoint>().GetCheckpointNumber().ToString();
            string reset = " ";
            if (_reseted)
            {
                reset = "x";
                _reseted = false;
            }
            string jump = " ";
            if (_jumped)
            {
                jump = "x";
                _jumped = false;
            }
            string dialog = " ";
            if (_dialogTriggered)
            {
                dialog = "x";
                _dialogTriggered = false;
            }

            var lineBuilder = new StringBuilder();

            lineBuilder.Append(totalGameTime.ToString());
            lineBuilder.Append(",");
            lineBuilder.Append(_currentStats.Category.ToString());
            lineBuilder.Append(",");
            lineBuilder.Append(xPos.ToString());
            lineBuilder.Append(",");
            lineBuilder.Append(yPos.ToString());
            lineBuilder.Append(",");
            lineBuilder.Append(health.ToString());
            lineBuilder.Append(",");
            lineBuilder.Append(jump);
            lineBuilder.Append(",");
            lineBuilder.Append(_currentAbility);
            lineBuilder.Append(",");
            lineBuilder.Append(checkpoint);
            lineBuilder.Append(",");
            lineBuilder.Append(reset);
            lineBuilder.Append(",");
            lineBuilder.Append(dialog);

            // Append to file.
            _fileStream.WriteLine(lineBuilder.ToString());
        }

        private void createHeader()
        {
            _fileStream.WriteLine("Time,Type,X,Y,Health,Jumped,Using Ability,Checkpoint,Reset,Dialog");
        }

        private void createFooter()
        {
            var strBuilder = new StringBuilder();
            _fileStream.WriteLine("Statistics Summary");
            _fileStream.WriteLine("Time Played,Deaths,Jumps,Dialogs Triggered");

            strBuilder.Append(_lastGameTime.TotalGameTime.TotalSeconds);
            strBuilder.Append(",");
            strBuilder.Append(_resets);
            strBuilder.Append(",");
            strBuilder.Append(_jumps);
            strBuilder.Append(",");
            strBuilder.Append(_dialogs);

            _fileStream.WriteLine(strBuilder.ToString());
        }

        private string placeInCenter(string text, int width)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (text.Length > width)
                throw new InvalidOperationException();

            float diff = width - text.Length;
            int left = (int)(diff / 2f);

            return text.PadLeft(left + text.Length).PadRight(width);
        }
        #endregion
    }
}
