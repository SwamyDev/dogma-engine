using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pontification.Components;

namespace Pontification.ScreenManagement
{
    public class InputState
    {
        /// <summary>
        /// Used to map mouse inputs as well. So that now it's for instance possible
        /// to assign a keyboard key OR a mouse key to an input event like "Jump".
        /// </summary>
        private enum MouseButtons
        {
            None,
            MB_LEFT,
            MB_RIGHT
        }
        #region Private attributes
        private static readonly string _configFileEnding = "ini";
        private static MouseState _curMouseState;
        private static MouseState _prvMouseState;
        private static KeyboardState _curKeyboardState;
        private static KeyboardState _prvKeyboardState;
        private static Dictionary<string, Keys> _keyboardDictionary = new Dictionary<string, Keys>();
        private static Dictionary<string, MouseButtons> _mouseDictionary = new Dictionary<string, MouseButtons>();
        private static float _cameraTranslationX;
        private static float _cameraTranslationY;
        private static float _oldCameraTranslationX;
        private static float _oldCameraTranslationY;
        #endregion

        #region Public properties
        public static Vector2 MousePosition { get { return new Vector2(_curMouseState.X, _curMouseState.Y); } }
        #endregion

        public InputState()
        {
            readFromConfigFile();
            _curMouseState = Mouse.GetState();
        }

        #region Internal methods
        internal void Update()
        {
            _prvMouseState = _curMouseState;
            _curMouseState = Mouse.GetState();
            _prvKeyboardState = _curKeyboardState;
            _curKeyboardState = Keyboard.GetState();

            _oldCameraTranslationX = _cameraTranslationX;
            _oldCameraTranslationY = _cameraTranslationY;
            _cameraTranslationX = Camera.Position.X;
            _cameraTranslationY = Camera.Position.Y;
        }
        #endregion

        #region Public methods
        public static void SetMouse(Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);
        }
        public static bool IsDown(string input)
        {
            Keys key; MouseButtons button;
            if (_keyboardDictionary.TryGetValue(input, out key))
                return isKeyPressed(key);
            else if (_mouseDictionary.TryGetValue(input, out button))
                return isMouseButtonPressed(button);
            else
                return false;
        }
        public static bool IsNewDown(string input)
        {
            Keys key; MouseButtons button;
            if (_keyboardDictionary.TryGetValue(input, out key))
                return isNewKeyPressed(key);
            else if (_mouseDictionary.TryGetValue(input, out button))
                return isNewMouseButtonPressed(button);
            else
                return false;
        }
        public static bool IsUp(string input)
        {
            Keys key; MouseButtons button;
            if (_keyboardDictionary.TryGetValue(input, out key))
                return isKeyUp(key);
            else if (_mouseDictionary.TryGetValue(input, out button))
                return isMouseButtonUp(button);
            else
                return false;
        }
        public static bool IsNewUp(string input)
        {
            Keys key; MouseButtons button;
            if (_keyboardDictionary.TryGetValue(input, out key))
                return isNewKeyUp(key);
            else if (_mouseDictionary.TryGetValue(input, out button))
                return isNewMouseButtonUp(button);
            else
                return false;
        }

        /// <summary>
        /// Returns relative movement along the X-Axis.
        /// Also takes Camera position into account.
        /// </summary>
        /// <returns>Relative movement</returns>
        public static float GetXAxis()
        {
            return 0.0f;
        }

        /// <summary>
        /// Returns relative movement along the Y-Axis.
        /// Also takes Camera position into account.
        /// </summary>
        /// <returns>Relative movement</returns>
        public static float GetYAxis()
        {
            return 0.0f;
        }
        #endregion

        #region Private methods
        private void readFromConfigFile()
        {
            using (StreamReader sr = new StreamReader(string.Format("Config/Input.{0}", _configFileEnding)))
            {
                // Read file line by line
                while (!sr.EndOfStream)
                {
                    // Read in scene graph
                    //
                    string line = sr.ReadLine().Replace(" ", string.Empty);

                    // Split node name from neightbours
                    string[] split = line.Split(':');
                    string name = split[0];
                    string control = split[1];

                    // Parse for keyboard key and add to dictionary
                    Keys key = parseToKey(control);
                    MouseButtons button = parseToMouseBtn(control);

                    if (key != Keys.None)
                        _keyboardDictionary.Add(name, key);
                    else if (button != MouseButtons.None)
                        _mouseDictionary.Add(name, button);
                }
                sr.Close();
            }
        }

        private Keys parseToKey(string keyName)
        {
            Keys key;
            switch (keyName)
            {
                case "A":
                    key = Keys.A;
                    break;
                case "B":
                    key = Keys.B;
                    break;
                case "C":
                    key = Keys.C;
                    break;
                case "D":
                    key = Keys.D;
                    break;
                case "E":
                    key = Keys.E;
                    break;
                case "F":
                    key = Keys.F;
                    break;
                case "G":
                    key = Keys.G;
                    break;
                case "H":
                    key = Keys.H;
                    break;
                case "I":
                    key = Keys.I;
                    break;
                case "J":
                    key = Keys.J;
                    break;
                case "K":
                    key = Keys.K;
                    break;
                case "L":
                    key = Keys.L;
                    break;
                case "M":
                    key = Keys.M;
                    break;
                case "N":
                    key = Keys.N;
                    break;
                case "O":
                    key = Keys.O;
                    break;
                case "P":
                    key = Keys.P;
                    break;
                case "Q":
                    key = Keys.Q;
                    break;
                case "R":
                    key = Keys.R;
                    break;
                case "S":
                    key = Keys.S;
                    break;
                case "T":
                    key = Keys.T;
                    break;
                case "U":
                    key = Keys.U;
                    break;
                case "V":
                    key = Keys.V;
                    break;
                case "W":
                    key = Keys.W;
                    break;
                case "X":
                    key = Keys.X;
                    break;
                case "Y":
                    key = Keys.Y;
                    break;
                case "Z":
                    key = Keys.Z;
                    break;
                case "LeftShift":
                    key = Keys.LeftShift;
                    break;
                case "RightShift":
                    key = Keys.RightShift;
                    break;
                case "LeftControl":
                    key = Keys.LeftControl;
                    break;
                case "RightControl":
                    key = Keys.RightControl;
                    break;
                case "LeftAlt":
                    key = Keys.LeftAlt;
                    break;
                case "RightAlt":
                    key = Keys.RightAlt;
                    break;
                case "Space":
                    key = Keys.Space;
                    break;
                case "Left":
                    key = Keys.Left;
                    break;
                case "Right":
                    key = Keys.Right;
                    break;
                case "Up":
                    key = Keys.Up;
                    break;
                case "0":
                    key = Keys.NumPad0;
                    break;
                case "1":
                    key = Keys.NumPad1;
                    break;
                case "2":
                    key = Keys.NumPad2;
                    break;
                case "3":
                    key = Keys.NumPad3;
                    break;
                case "4":
                    key = Keys.NumPad4;
                    break;
                case "5":
                    key = Keys.NumPad5;
                    break;
                case "6":
                    key = Keys.NumPad6;
                    break;
                case "7":
                    key = Keys.NumPad7;
                    break;
                case "8":
                    key = Keys.NumPad8;
                    break;
                case "9":
                    key = Keys.NumPad9;
                    break;
                case "F1":
                    key = Keys.F1;
                    break;
                case "F2":
                    key = Keys.F2;
                    break;
                case "F3":
                    key = Keys.F3;
                    break;
                case "F4":
                    key = Keys.F4;
                    break;
                case "F5":
                    key = Keys.F5;
                    break;
                case "F6":
                    key = Keys.F6;
                    break;
                case "F7":
                    key = Keys.F7;
                    break;
                case "F8":
                    key = Keys.F8;
                    break;
                case "F9":
                    key = Keys.F9;
                    break;
                case "F10":
                    key = Keys.F10;
                    break;
                case "F11":
                    key = Keys.F11;
                    break;
                case "F12":
                    key = Keys.F12;
                    break;
                case "CapsLock":
                    key = Keys.CapsLock;
                    break;
                case "Delete":
                    key = Keys.Delete;
                    break;
                case "Back":
                    key = Keys.Back;
                    break;
                case "End":
                    key = Keys.End;
                    break;
                case "Pause":
                    key = Keys.Pause;
                    break;
                case "Escape":
                    key = Keys.Escape;
                    break;
                default:
                    key = Keys.None;
                    break;
            }

            return key;
        }

        private MouseButtons parseToMouseBtn(string btnName)
        {
            MouseButtons button;

            switch (btnName)
            {
                case "LeftMouseButton":
                    button = MouseButtons.MB_LEFT;
                    break;
                case "RightMouseButton":
                    button = MouseButtons.MB_RIGHT;
                    break;
                default:
                    button = MouseButtons.None;
                    break;
            }

            return button;
        }

        private static bool isKeyPressed(Keys key)
        {
            return _curKeyboardState.IsKeyDown(key);
        }

        private static bool isNewKeyPressed(Keys key)
        {
            return _curKeyboardState.IsKeyDown(key) && _prvKeyboardState.IsKeyUp(key);
        }

        private static bool isKeyUp(Keys key)
        {
            return _curKeyboardState.IsKeyUp(key);
        }

        private static bool isNewKeyUp(Keys key)
        {
            return _curKeyboardState.IsKeyUp(key) && _prvKeyboardState.IsKeyDown(key);
        }
        private static bool isMouseButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MB_LEFT:
                    return _curMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MB_RIGHT:
                    return _curMouseState.RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        private static bool isNewMouseButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MB_LEFT:
                    return _curMouseState.LeftButton == ButtonState.Pressed && _prvMouseState.LeftButton == ButtonState.Released;
                case MouseButtons.MB_RIGHT:
                    return _curMouseState.RightButton == ButtonState.Pressed && _prvMouseState.RightButton == ButtonState.Released;
                default:
                    return false;
            }
        }
        private static bool isMouseButtonUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MB_LEFT:
                    return _curMouseState.LeftButton == ButtonState.Released;
                case MouseButtons.MB_RIGHT:
                    return _curMouseState.RightButton == ButtonState.Released;
                default:
                    return false;
            }
        }
        private static bool isNewMouseButtonUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MB_LEFT:
                    return _curMouseState.LeftButton == ButtonState.Released && _prvMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MB_RIGHT:
                    return _curMouseState.RightButton == ButtonState.Released && _prvMouseState.RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        #endregion
    }
}
