using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.Components
{
    /// <summary>
    /// This is the base class for all components used by the engine. It holds a reference to the GameObject the component
    /// is associated with. It also provides a method to send messages to other components and retreive oder components 
    /// from its GameObject. All components used in the engine must inherit from this class.
    /// </summary>
    public abstract class Component : IDisposable
    {
        private static int _maxID;
        private int _id;
        private bool _isDisposed;

        public bool IsInitialized { get; protected set; }
        public GameObject GameObject { get; set; }
        public bool IsActive { get; set; }

        // public ContentManager Content { get { return SceneManagement.SceneManager.Instance.FocusScene.Content; } }

        public Component()
        {
            _id = _maxID;
            _maxID++;

            IsActive = true;
        }

        #region Public methods

        #region Overrides
        /// <summary>
        /// Invoked when the component is created. The game might not be fully initialized yet.
        /// </summary>
        public virtual void Awake()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Awake method on disposed object");
        }

        /// <summary>
        /// Invoked when the game has started.
        /// </summary>
        public virtual void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Start method on disposed object");
        }

        /// <summary>
        /// Invoked when the component gets removed from the game object.
        /// </summary>
        public virtual void Removed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Removed method on disposed object");
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update method on disposed object");
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update Draw on disposed object");
        }
        #endregion

        /// <summary>
        /// Adds a new component to the specified game object
        /// </summary>
        /// <typeparam name="T">Type of the component to be added</typeparam>
        public T AddComponent<T>() where T : Component, new()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update AddComponent on disposed object");

            return GameObject.AddComponent<T>();
        }

        /// <summary>
        /// Moves the passed component to this game object. It removes the
        /// component from the former parent game obect.
        /// </summary>
        /// <param name="component">Component to move</param>
        public void MoveComponent(Component component)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update MoveComponent on disposed object");

            GameObject.MoveComponent(component);
        }

        /// <summary>
        /// Returns the first occurance of the component with the specified type or 
        /// child of that type within the GameObject. Shorthand to GameObject method.
        /// </summary>
        /// <typeparam name="T">Type must be child of Component</typeparam>
        /// <returns>Requested component or Null if it is not found</returns>
        public T GetComponent<T>() where T : Component, new()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update GetComponent on disposed object");

            return GameObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns a list of the components with the specified type or 
        /// child of that type within the GameObject. Shorthand to GameObject method.
        /// </summary>
        /// <typeparam name="T">Type must be child of Component</typeparam>
        /// <returns>Requested components or empty list if it is not found</returns>
        public List<T> GetComponents<T>() where T : Component, new()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update GetComponents on disposed object");

            return GameObject.GetComponents<T>();
        }

        /// <summary>
        /// Invokes all methods with the given name found in all the components of this GameObjects. Passes along the value 
        /// as well if specified. It uses boxing to pass the value, so avoid using SendMessage with parameters in performance
        /// intese sections. You can specify if send message should throw an error when it doesn't find a mathing method.
        /// This is the default behaviour. Shorthand to the GameObject method.
        /// </summary>
        /// <param name="methodName">Name of the method to invoke</param>
        /// <param name="value">Value to be passed to the method</param>
        /// <param name="options">Specify wether an error should be thrown if no method could be found</param>
        public void SendMessage(string methodName, object[] args = null, SendMessageObtions options = SendMessageObtions.DONT_REQUIRE_RECEIVER)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update SendMessage on disposed object");

            GameObject.SendMessage(methodName, args, options);
        }

        /// <summary>
        /// Searched for the first occurence of the game object with the given name.
        /// </summary>
        /// <param name="name">Name of the game object</param>
        /// <returns>The game object if found; Null otherwise</returns>
        public GameObject FindGameObject(string name)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update FindGameObject on disposed object");

            return GameObject.FindGameObject(name);
        }


        /// <summary>
        /// Searched for the game objects with the given name
        /// </summary>
        /// <param name="name">Name of the game objects</param>
        /// <returns>A list of game objects if found; Empty list otherwise</returns>
        public List<GameObject> FindGameObjects(string name)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update FindGameObjects on disposed object");

            return GameObject.FindGameObjects(name);
        }

        public override string ToString()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Component", "Called Update ToString on disposed object");

            return string.Format("Component with ID: {0}", _id.ToString());
        }
        #endregion

        public void Dispose()
        {
            _isDisposed = true;

            disposing();
        }

        protected virtual void disposing()
        {
        }
    }
}
