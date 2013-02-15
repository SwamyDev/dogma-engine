using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Monitoring;
using Pontification.Components;
#if WINDOWS || XBOX
using XNAGameConsole;
#endif

namespace Pontification.ScreenManagement
{
    public class ScreenManager : DrawableGameComponent
    {
        private static ScreenManager _instace;

        #region Private attributes
        private List<GameScreen> _screens = new List<GameScreen>();
        private List<GameScreen> _tempScreens = new List<GameScreen>();
        private InputState _input = new InputState();
		#if WINDOWS || XBOX
        private GameConsole _console;
		#endif
        private bool _isInitialized;
        private bool _logStats = false;
        #endregion

        #region Public properties
        public static ScreenManager Instance
        {
            get 
            {
                if (_instace == null)
                    throw new ArgumentNullException("Game hasn't finished initialisation. ScreenManger class is not loaded yet.");
                return _instace; 
            }
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont Font { get; private set; }
        public Texture2D BlankTexture { get; private set; }
        #endregion


        // Contructors.

        // Public static method initializes screen manager. To make sure we only have one instance going per game.
        public static ScreenManager Init(Game game)
        {
            if (_instace == null)
                return new ScreenManager(game);
            else
                return _instace;
        }

        private ScreenManager(Game game)
            : base (game)
        {
            _instace = this;
        }

        #region Public methods

        // Overrides.

        public override void Initialize()
        {
            base.Initialize();

            //Debug.DisplayOptions = DebugDisplays.RAM_USAGE | DebugDisplays.CPU_USAGE | DebugDisplays.AI | DebugDisplays.PHYSICS;
			
			#if WINDOWS || XBOX
            _console = new GameConsole(Game, SpriteBatch);
            addConsoleCommands();
			#endif

            _isInitialized = true;

			Game.IsMouseVisible = false;
        }

        public override void Update(GameTime gameTime)
        {
            Time.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (Time.DeltaTime > 0.1f)	// Don't update when lagging.
				return;

			#if WINDOWS || XBOX
            if (_console.Opened || _console.Opening)
                return;
			#endif

            Scheduler.Instance.Run();
            
            _input.Update();
            
            //_tempScreens.Clear();

            // Update temporary screens.
            //_screens.ForEach((screen) => { _tempScreens.Add(screen); });

            bool otherScreenHasFocus = !Game.IsActive;
            bool isCoveredByOtherScreen = false;

            // Update logger class to keep memory and cpu usage up to date.
            Logger.Instance.Update(gameTime);

            if (_logStats)
                PlayerStatistics.Instance.Update(gameTime);

            if (InputState.IsNewDown("Quit"))
                Game.Exit();

            // Update screens.
            // May use copied temp screen here.
            _screens.ForEach((screen) =>
            {
                screen.Update(gameTime, otherScreenHasFocus, isCoveredByOtherScreen);

                if (screen.ScreenState != ScreenState.TRANSITION_ON && screen.ScreenState != ScreenState.TRANSITION_OFF)
                    return;

                if (otherScreenHasFocus == false)
                {
                    screen.HandleInput(gameTime, _input);
                    otherScreenHasFocus = true;
                }

                if (screen.IsPopup == false && screen.IsTile == false)
                {
                    isCoveredByOtherScreen = true;
                }
            });

            SceneManagement.Subtitles.Update(gameTime);

            //_tempScreens.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            // Clear screen.
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            // Invoke draw event in all current screens.
            _screens.ForEach((screen) =>
            {
                if (screen.ScreenState == ScreenState.HIDDEN)
                    return;

                screen.Draw(SpriteBatch, gameTime);
            });

            SceneManagement.Subtitles.Draw(SpriteBatch);
            Debug.Instance.Draw(SpriteBatch);
        }

        // Custom.

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we screen manger is already init, freshly load content of screen.
            if (_isInitialized)
            {
                screen.LoadContent();
            }

            _screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
        }

        public void SetFocusOnScreen(GameScreen screen)
        {
            if (_screens.Count > 0 && _screens[0] != screen)
            {
                _screens.Remove(screen);
                _screens.Add(screen);
            }
        }

        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BlankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            SpriteBatch.End();
        }
        #endregion


        #region Protected methods
        // Overrides.

        protected override void LoadContent()
        {
            var cm = Game.Content;

            Debug.Instance.LoadContent(cm);

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = cm.Load<SpriteFont>("ScreenManagement/menufont");
            BlankTexture = cm.Load<Texture2D>("ScreenManagement/blank");

            Music.Load(cm);

            SceneManagement.Subtitles.LoadContent(cm);

            // Call load event for all game screen currently active
            _screens.ForEach((screen) => { screen.LoadContent(); });
        }

        protected override void UnloadContent()
        {
            // Call unload content for every active screen
            _screens.ForEach((screen) => { screen.UnloadContent(); });

            Music.Unload();

            if (_logStats)
                PlayerStatistics.Instance.Unload();
        }
        #endregion

        #region Private methods
        private void addConsoleCommands()
        {
			#if WINDOWS || XBOX
            _console.AddCommand("debug", (arguments) =>
            {
                if (string.Compare(arguments[0], "CPU", true) == 0)
                {
                    Debug.DisplayOptions ^= DebugDisplays.CPU_USAGE;
                    return "CPU usage information toggled";
                }
                else if (string.Compare(arguments[0], "RAM", true) == 0)
                {
                    Debug.DisplayOptions ^= DebugDisplays.RAM_USAGE;
                    return "RAM usage information toggled";
                }
                else if (string.Compare(arguments[0], "PHYSICS", true) == 0)
                {
                    Debug.DisplayOptions ^= DebugDisplays.PHYSICS;
                    return "Physics information toggled";
                }
                else if (string.Compare(arguments[0], "GRID", true) == 0)
                {
                    Debug.DisplayOptions ^= DebugDisplays.GRID;
                    return "Grid information toggled";
                }
                return arguments[0] + " is not a valid command (CPU, RAM, PHYSICS, GRID)";
            }, "Toggles debug information (CPU, RAM, PHYSICS, GRID)");

            _console.AddCommand("move", (arguments) =>
            {
                if (arguments.Length < 2)
                    return "You must specify x and y values by which the players should be moved.";

                int xValue, yValue;
                bool xValid = int.TryParse(arguments[0], out xValue);
                bool yValid = int.TryParse(arguments[1], out yValue);

                if (!xValid)
                    return arguments[0] + " is not valid. Only integer values are valid";
                if (!yValid)
                    return arguments[1] + " is not valid. Only integer values are valid";

                // Teleport player by specified amount.
                var physics = Pontification.SceneManagement.SceneInfo.Player.GetComponent<CharacterPhysicsComponent>();

                if (physics != null)
                {
                    physics.Teleport(physics.GameObject.Position + new Vector2(xValue, yValue));
                    return string.Format("Teleporting the player: ({0}, {1})", arguments[0], arguments[1]);
                }
                else
                {
                    return "Can't teleport player. No character physics component specified!";
                }

            }, "Moves the player character by the specified pixel values.");
			#endif
        }
        #endregion
    }
}
