using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pontification.ScreenManagement;
using System.Xml.Serialization;
using System.IO;
using Pontification.Components;

namespace Pontification
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Private attributes
        private static Game _instance;
        private GraphicsDeviceManager _graphics;
        private ScreenManager _screenManager;
        private string _configFileName = "Settings.ini";
        private int _screenWidth;
        private int _screenHeight;
        private bool _isFullScreen;
        #endregion

        #region Puplic properties
        public static Game Instance { get { return _instance; } }
        public static readonly int CellSize = 58;
        public static readonly int NumberOfGridLines = 200;

        public ScreenManager ScreenManager { get { return _screenManager; } }
        #endregion

        // Contructors.
        public Game()
        {
			_instance = this;
            Content.RootDirectory = "Content";
            readFromConfigFile();

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = _isFullScreen;


            // Initialize screen manager -> and thereby game.
            _screenManager = ScreenManager.Init(this);
            
            // Add to update and draw loop.
            Components.Add(_screenManager);

            AddInitialScreens();
        }

        #region Private methods
        // Adds the first screen to the screen manager, so the game can start.
        private void AddInitialScreens()
        {
            SceneManagement.SceneManager.Instance.UpdateScenes();
        }

        private void readFromConfigFile()
        {
            using (StreamReader sr = new StreamReader(string.Format("Config/{0}", _configFileName)))
            {
                // Read file line by line
                while (!sr.EndOfStream)
                {
                    // Remove spaces
                    string line = sr.ReadLine().Replace(" ", string.Empty);

                    // Split node name from neightbours
                    string[] split = line.Split(':');
                    string name = split[0];
                    string control = split[1];

                    // Parse for configurations
                    if (name == "Resultion")
                    {
                        string[] resultion = control.Split('x');
                        _screenWidth = int.Parse(resultion[0]);
                        _screenHeight = int.Parse(resultion[1]);
                    }
                    if (name == "Fullscreen")
                    {
                        _isFullScreen = bool.Parse(control);
                    }
                }
                sr.Close();
            }
        }
        #endregion

        // DISCLAIMER
        // Initial, LoadContent, UnloadConent, Update and Draw events are iluded because
        // they are all handled now by the screen manager which is a child of DrawableGameObject
        // and part of the game classe's Components
    }
}
