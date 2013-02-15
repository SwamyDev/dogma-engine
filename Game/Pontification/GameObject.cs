using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pontification.SceneManagement;
using Pontification.Components;

namespace Pontification
{
    public enum SendMessageObtions
    {
        REQUIRE_RECEIVER,
        DONT_REQUIRE_RECEIVER,
    }
    /// <summary>
    /// This is the core of the engine. Eveyr object within the game is essentially a GameObject. It basically holds 
    /// only a position value and a list of components. The components defie the behaviour of the game object,
    /// like for instance display an animation or apply physics to the game object.
    /// </summary>
    public class GameObject : IDisposable
    {
        #region Private attributes
        private static int _maxID;
        private int _id;
        private List<Component> _components = new List<Component>();
        private Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();
        private bool _isDisposed;
        #endregion
        
        #region Public properties
        public Scene Scene { get; private set; }
        public Layer Layer { get; private set; }
        public string Name { get; private set; }
        public int BagIndex { get; private set; }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public bool IsActive { get; set; }

        public ContentManager Content { get { return SceneManagement.SceneManager.Instance.FocusScene.Content; } }
        #endregion

        public GameObject(string name, Scene scene, Layer layer)
            : this(name, Vector2.Zero, scene, layer)
        {
        }
        public GameObject(string name, Vector2 position, Scene scene, Layer layer)
        {
            _id = _maxID;
            _maxID++;
            Scene = scene;
            Layer = layer;
            Name = name;
            Position = position;
            IsActive = true;

            // Add game object to layer.
            BagIndex = layer.GameObjects.Add(this);
        }

        #region Public methods

        /// <summary>
        /// Invoked when the game object is created. The game might not be fully initialized yet.
        /// </summary>
        public void Awake()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called Awake method on disposed object");
        }

        /// <summary>
        /// Invoked when the game has started.
        /// </summary>
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called Start method on disposed object");

            _components.ForEach((c) => { c.Start(); });
        }

        public void Update(GameTime gameTime)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called Update method on disposed object");

            _components.ForEach((c) => 
            {
                if (c.IsActive)
                    c.Update(gameTime); 
            });
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called Draw method on disposed object");

            _components.ForEach((c) => 
            { 
                if (c.IsActive)
                    c.Draw(sb, gameTime); 
            });
        }

        /// <summary>
        /// Adds a new component to the specified game object
        /// </summary>
        /// <typeparam name="T">Type of the component to be added</typeparam>
        public T AddComponent<T>() where T : Component, new()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called AddComponent method on disposed object");

            T component = new T();
            component.GameObject = this;

            // Invoke start event.
            component.Awake();

            _components.Add(component);

            // If game has already started invoke start event of the component.
            /*if (Scene.HasStarted)
                component.Start();*/

            return component;
        }

        /// <summary>
        /// Removes the given component from the game object
        /// and invokes its remove event.
        /// </summary>
        /// <param name="component">The component to remove</param>
        /// <returns>Wether it has been removed successfully or not</returns>
        public bool RemoveComponent(Component component)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called AddComponent method on disposed object");

            if (_components.Remove(component))
            {
                component.Removed();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves the passed component to this game object. It removes the
        /// component from the former parent game obect.
        /// </summary>
        /// <param name="component">Component to move</param>
        public void MoveComponent(Component component)
        {

            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called AddComponent method on disposed object");

            // Remove from parten.
            component.GameObject.RemoveComponent(component);

            component.GameObject = this;
            _components.Add(component);
        }

        /// <summary>
        /// Returns the first occurance of the component with the specified type or 
        /// child of that type within the GameObject
        /// </summary>
        /// <typeparam name="T">Type must be child of Component</typeparam>
        /// <returns>Requested component or Null if it is not found</returns>
        public T GetComponent<T>() where T : Component, new()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called GetComponent method on disposed object");

            return _components.Find((component) => { return component is T; }) as T;
        }

        /// <summary>
        /// Returns a list of the components with the specified type or 
        /// child of that type within the GameObject
        /// </summary>
        /// <typeparam name="T">Type must be child of Component</typeparam>
        /// <returns>Requested components or empty list if it is not found</returns>
        public List<T> GetComponents<T>() where T : Component
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called GetComponents method on disposed object");

            return _components.FindAll((component) => { return component is T; }).Cast<T>().ToList();
        }

        /// <summary>
        /// Invokes all methods with the given name found in all the components of this GameObjects. Passes along the value 
        /// as well if specified. It uses boxing to pass the value, so avoid using SendMessage with parameters in performance
        /// intese sections. You can specify if send message should throw an error when it doesn't find a mathing method.
        /// This is the default behaviour. 
        /// </summary>
        /// <param name="methodName">Name of the method to invoke</param>
        /// <param name="value">Value to be passed to the method</param>
        /// <param name="options">Specify wether an error should be thrown if no method could be found</param>
        public void SendMessage(string methodName, object[] args = null, SendMessageObtions options = SendMessageObtions.DONT_REQUIRE_RECEIVER)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called SendMessage method on disposed object");

            bool hasReceiver = false;
            _components.ForEach((component) =>
            {
                if (!component.IsActive)
                    return;

                MethodInfo method;
                StringBuilder keyBuilder = new StringBuilder(component.ToString());
                keyBuilder.Append(".");
                keyBuilder.Append(methodName);
                string key = keyBuilder.ToString();

                // First try to get the cached method.
                if (!_methodCache.TryGetValue(key, out method))
                {
                    // We haven't used this method before so search for it in the component
                    Type componentType = component.GetType();
                    method = componentType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

                    // Cache method info -> Cache null as well so we don't look up the component later if it simply doesn't contain the method
                    if (method != null)
                        _methodCache.Add(key, method);
                }

                // Invoke method with the given parameter
                if (method != null)
                {
                    method.Invoke(component, args);
                    hasReceiver = true;
                }
            });

            if (options == SendMessageObtions.REQUIRE_RECEIVER && !hasReceiver)
            {
                throw new NullReferenceException(string.Format("Couldn't find a method with the method name {0} in {1}", methodName, this.ToString()));
            }
        }

        /// <summary>
        /// Searched for the first occurence of the game object with the given name.
        /// </summary>
        /// <param name="name">Name of the game object</param>
        /// <returns>The game object if found; Null otherwise</returns>
        public GameObject FindGameObject(string name)
        {
            return Scene.FindGameObject(name);
        }


        /// <summary>
        /// Searched for the game objects with the given name
        /// </summary>
        /// <param name="name">Name of the game objects</param>
        /// <returns>A list of game objects if found; Empty list otherwise</returns>
        public List<GameObject> FindGameObjects(string name)
        {
            return Scene.FindGameObjects(name);
        }

        public override string ToString()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called ToString method on disposed object");

            return string.Format("Game Object {0}, with ID: {1}", Name, _id.ToString());
        }

        /// <summary>
        /// Brings this game object to the front of the layer.
        /// </summary>
        public void ToFront()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("GameObject", "Called ToFront method on disposed object");

            if (BagIndex >= 0 && Layer.GameObjects[BagIndex] != null)
            {
                Layer.GameObjects.Remove(BagIndex);
                if (Layer.GameObjects.Count != BagIndex)
                    Layer.GameObjects[BagIndex].BagIndex = BagIndex;

                // Add back at front.
                BagIndex = Layer.GameObjects.Add(this);
            }
        }

        public void Dispose()
        {
            // Don't clean up if we already did it.
            if (_isDisposed)
                return;

            // Dispose components.
            _components.ForEach((c) => { c.Dispose(); });
            _components.Clear();

            // Remove from Bag if necessary.
            if (BagIndex >= 0 && Layer.GameObjects[BagIndex] != null)
            {
                Layer.GameObjects.Remove(BagIndex);
                if (Layer.GameObjects.Count != BagIndex)
                    Layer.GameObjects[BagIndex].BagIndex = BagIndex;

                BagIndex = -1;
            }
        }
        #endregion
    }
}
