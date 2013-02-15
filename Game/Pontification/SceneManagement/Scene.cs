using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Physics;
using Pontification.Components;
using Pontification.ScreenManagement;

namespace Pontification.SceneManagement
{
    public struct GameObjectReferenceInfo
    {
        public Component Component;
        public string Name;
        public System.Reflection.PropertyInfo Property;
    }
    public class Scene : IScreenContent
    {
        #region Private attributes
        private static readonly string _sceneFileEnding = "xml";
        private static readonly string _folder = "Scenes\\";
        private ContentManager _contentManager;
        private GameObject _player;
		private World _worldInfo = new World(new Vector2(0, 20f));
        #endregion

        #region Public properties
        // Serializeable attributes.
        [XmlAttribute()]
        public String Name;

        [XmlAttribute()]
        public bool Visible;

        public List<Layer> Layers;
        public SerializableDictionary CustomProperties;

        [XmlIgnore]
        public List<GameObjectReferenceInfo> PendingGameObjectRefereces = new List<GameObjectReferenceInfo>();
        [XmlIgnore]
        public string SceneName { get; private set; }
        [XmlIgnore]
        public Vector2 Origin { get; private set; }
        [XmlIgnore]
        public ContentManager Content { get { return _contentManager; } }
        [XmlIgnore]
        public World WorldInfo { get { return _worldInfo; } }
        [XmlIgnore]
        public GameObject Player { get { return _player; } }
        [XmlIgnore]
        public bool HasStarted { get; private set; }

        // Interface properties.
        [XmlIgnore]
        public GameScreen CurrentScreen { get; set; }
        #endregion

        #region Events
        public event EventHandler PreUpdate;
        public event EventHandler PostUpdate;
        #endregion

        // Loads the scene from an xml file
        public static Scene LoadFromFile(string filename)
        {
			string folder;

#if WINDOWS || XBOX
            folder = _folder;
#else
			folder = _folder.Replace("\\", "/");
#endif
            FileStream stream = File.Open(string.Concat(folder,filename,".",_sceneFileEnding), FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Scene));
            Scene scene = (Scene)serializer.Deserialize(stream);
            scene.SceneName = filename;
            stream.Close();

            return scene;
        }

        // Constructor

        private Scene()
        {
            Visible = true;
            Layers = new List<Layer>();
            CustomProperties = new SerializableDictionary();
        }

        #region Public methods
        // Interface methods.
        public void LoadContent()
        {
            if (_contentManager == null)
                _contentManager = new ContentManager(ScreenManager.Instance.Game.Services, "Content");

            Layers.ForEach((layer) => { layer.LoadContent(_contentManager, this); });

            _player = FindGameObject("Player");

            PendingGameObjectRefereces.ForEach((grInfo) => 
            {
                var go = grInfo.Component.FindGameObject(grInfo.Name);
                if (go != null)
                    grInfo.Property.SetValue(grInfo.Component, go, null);
            });
            PendingGameObjectRefereces.Clear();
            PendingGameObjectRefereces = null;

            // Finally call finished initialisation event on layers.
            Layers.ForEach((layer) => { layer.FinishedInitialisation(); });

            Cursor.Instance.GameObject.ToFront();
            Music.Play("background-music");

            HasStarted = true;
            SceneInfo.ResetInfoSealed = true;
        }

        public void UnloadContent()
        {
            if (_contentManager != null)
                _contentManager.Unload();
        }

        /// <summary>
        /// Searched for the first occurence of the game object with the given name.
        /// </summary>
        /// <param name="name">Name of the game object</param>
        /// <returns>The game object if found; Null otherwise</returns>
        public GameObject FindGameObject(string name)
        {
            foreach (var layer in Layers)
            {
                GameObject go = layer.FindGameObject(name);

                if (go != null)
                    return go;
            }

            return null;
        }


        /// <summary>
        /// Searched for the game objects with the given name
        /// </summary>
        /// <param name="name">Name of the game objects</param>
        /// <returns>A list of game objects if found; Empty list otherwise</returns>
        public List<GameObject> FindGameObjects(string name)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            foreach (var layer in Layers)
            {
                gameObjects.AddRange(layer.FindGameObjects(name));
            }

            return gameObjects;
        }

        public void Update(GameTime gameTime)
        {
            if (PreUpdate != null)
                PreUpdate(this, EventArgs.Empty);

            _worldInfo.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Layers.ForEach((layer) => { layer.Update(gameTime); });

            if (PostUpdate != null)
                PostUpdate(this, EventArgs.Empty);
        }

        public void HandleInput(GameTime gameTime, InputState input)
        {
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            Layers.ForEach((layer) =>
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetView(layer.ViewIdx));
                layer.Draw(sb, gameTime);
                sb.End();
            });
        }
        #endregion
    }

    public partial class Layer
    {
        private bool _hasStarted;

        #region Public properties
        /// <summary>
        /// The name of the layer.
        /// </summary>
        [XmlAttribute()]
        public String Name;

        /// <summary>
        /// Should this layer be visible?
        /// </summary>
        [XmlAttribute()]
        public bool Visible;

        /// <summary>
        /// The list of the items in this layer.
        /// </summary>
        public Bag<Item> Items;

        /// <summary>
        /// The Scroll Speed relative to the main camera. The X and Y components are 
        /// interpreted as factors, so (1;1) means the same scrolling speed as the main camera.
        /// Enables parallax scrolling.
        /// </summary>
        public Vector2 ScrollSpeed;

        /// <summary>
        /// A Dictionary containing any user-defined Properties.
        /// </summary>
        public SerializableDictionary CustomProperties;

        [XmlIgnore]
        public Bag<GameObject> GameObjects = new Bag<GameObject>();
        [XmlIgnore]
        public Scene Scene { get; private set; }
        [XmlIgnore]
        public int ViewIdx { get; private set; }
        #endregion

        /// TEST GARBAGE COLLECTION REMOVE LATER!!!
        private int _amountOfGarbage = 10000;
        private float _removeInSeconds = 5.0f;
        private float _elapsedTime;
        private List<GameObject> _bunchOfGarbage = new List<GameObject>();

        // Constructors

        private Layer()
        {
            Items = new Bag<Item>();
            ScrollSpeed = Vector2.One;
            CustomProperties = new SerializableDictionary();
        }

        #region Public methods
        public void LoadContent(ContentManager cm, Scene scene)
        {
            Scene = scene;

            Items.ForEach((item) => { item.LoadContent(cm, scene, this); });

            GameObjects.ForEach((go) => { go.Start(); });
            _hasStarted = true;
            // Generate some garbage.
            // generateSomeGarabge();
        }

        public void FinishedInitialisation()
        {
            ViewIdx = Camera.AddView(ScrollSpeed);
        }

        private void generateSomeGarabge()
        {
            var rand = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < _amountOfGarbage; i++)
            {
                Vector2 position = new Vector2(rand.Next(ScreenManager.Instance.GraphicsDevice.Viewport.Width), rand.Next(ScreenManager.Instance.GraphicsDevice.Viewport.Height));
                Texture2D texture = Scene.Content.Load<Texture2D>("Monitoring/garbage-texture-max-size");
                var gameObject = new GameObject(string.Format("GarbageObject{0}", i), position, Scene, this);

                var textureComponent = gameObject.AddComponent<TextureComponent>();
                textureComponent.Texture = texture;

                //gameObject.BagIndex = GameObjects.Add(gameObject);
                _bunchOfGarbage.Add(gameObject);
            }
        }

        /// <summary>
        /// Searched for the first occurence of the game object with the given name.
        /// </summary>
        /// <param name="name">Name of the game object</param>
        /// <returns>The game object if found; Null otherwise</returns>
        public GameObject FindGameObject(string name)
        {
            var gameObjects = FindGameObjects(name);

            if (gameObjects.Count > 0)
                return gameObjects[0];
            else
                return null;
        }


        /// <summary>
        /// Searched for the game objects with the given name
        /// </summary>
        /// <param name="name">Name of the game objects</param>
        /// <returns>A list of game objects if found; Empty list otherwise</returns>
        public List<GameObject> FindGameObjects(string name)
        {
            return (from go in GameObjects
                    where go.Name == name
                    select go).ToList<GameObject>();
        }

        public void Update(GameTime gameTime)
        {
            /*if (_elapsedTime >= _removeInSeconds && _bunchOfGarbage != null)
            {
                // Remove garbage.
                _bunchOfGarbage.ForEach((gameObject) =>
                {
                    gameObject.Delete();
                });

                _bunchOfGarbage.Clear();
                Scene.Content.Unload();
            }

            if (_elapsedTime >= _removeInSeconds + 5.0f)
            {
                generateSomeGarabge();
                _elapsedTime = 0.0f;
            }

            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;*/

            if (_hasStarted)
            {
                GameObjects.ForEach((go) =>
                {
                    if (go.IsActive)
                        go.Update(gameTime);
                });
            }
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (!Visible)
                return;

            // Items.ForEach((item) => { item.Draw(sb, gameTime); });
            GameObjects.ForEach((gameObject) => 
            {
                if (gameObject.IsActive)
                    gameObject.Draw(sb, gameTime); 
            });
        }
        #endregion
    }

    [XmlInclude(typeof(GameObjectItem))]
    [XmlInclude(typeof(GameObjectReference))]
    [XmlInclude(typeof(ComponentItemCollection))]
    [XmlInclude(typeof(PropertyBag))]
    [XmlInclude(typeof(PathItem))]
    [XmlInclude(typeof(PolygonPath))]
    [XmlInclude(typeof(PhysicsPath))]
    [XmlInclude(typeof(TextureItem))]
    [XmlInclude(typeof(AnimationItem))]
    [XmlInclude(typeof(AnimationDataItem))]
    [XmlInclude(typeof(PlayerStart))]

    public class DataObject
    {
        public virtual void Assign(ContentManager cm, Component comp, string binder) { }
    }

    public partial class Item : DataObject
    {
        #region Public properties
        /// <summary>
        /// The name of this item.
        /// </summary>
        [XmlAttribute()]
        public String Name;

        /// <summary>
        /// Should this item be visible?
        /// </summary>
        [XmlAttribute()]
        public bool Visible;

        /// <summary>
        /// The item's position in world space.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// A Dictionary containing any user-defined Properties.
        /// </summary>
        public SerializableDictionary CustomProperties;
        #endregion

        // Constructor.
        protected Item()
        {
            CustomProperties = new SerializableDictionary();
        }

        #region Public methods

        /// <summary>
        /// Called by Level.FromFile(filename) on each Item after the deserialization process.
        /// Should be overriden and can be used to load anything needed by the Item (e.g. a texture).
        /// </summary>
        public virtual void LoadContent(ContentManager cm, Scene scene, Layer layer)
        {
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
        }
        #endregion
    }

    public class GameObjectItem : Item
    {
        #region Public properties        
        /// <summary>
        /// The item's rotation in radians.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The item's scale vector.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// List of components added to the game object
        /// </summary>
        public ComponentItemCollection Components;
        #endregion

        private GameObjectItem()
        {
        }

        #region Public methods
        /// <summary>
        /// Called by Level.FromFile(filename) on each Item after the deserialization process.
        /// Loads all assets needed by the TextureItem, especially the Texture2D.
        /// You must provide your own implementation. However, you can rely on all public fields being
        /// filled by the level deserialization process.
        /// </summary>
        public override void LoadContent(ContentManager cm, Scene scene, Layer layer)
        {
            var gameObject = new GameObject(Name, Position, scene, layer);
            var compTypes = from type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            where type.IsClass && type.Namespace == "Pontification.Components"
                            select type;
            var addComponentMethod = typeof(GameObject).GetMethod("AddComponent");
            foreach (Type compType in compTypes)
            {
                foreach (PropertyBag compItem in Components)
                {
                    if (compType.Name == compItem.TypeName)
                    {   // Found component to add to game object.
                        // Set generic variable of AddComponent method of the game object.
                        var genericAddComponent = addComponentMethod.MakeGenericMethod(new Type[] { compType });
                        
                        // Invoke method to add component to game object.
                        var newComponent = genericAddComponent.Invoke(gameObject, null) as Component;

                        // Copy values of ComponentItem public properties to Component public properties with the same name.
                        var properties = (from src in compItem.GetKeyValuePairs()
                                          from dst in newComponent.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                          where src.Key == dst.Name
                                          select new { Src = src.Value, Dst = dst }).ToList();

                        properties.ForEach((pair) =>
                        {
                            if (pair.Dst.PropertyType.IsValueType || pair.Dst.PropertyType == typeof(string))
                                pair.Dst.SetValue(newComponent, pair.Src, null);

                            var blob = pair.Src as DataObject;
                            if (blob != null)
                                blob.Assign(cm, newComponent, pair.Dst.Name);
                        });

                        var camera = newComponent as Camera;
                        if (camera != null)
                            camera.UpdateViewport(ScreenManager.Instance.GraphicsDevice.Viewport.Width, ScreenManager.Instance.GraphicsDevice.Viewport.Height);
                    }
                }
            }

            //gameObject.BagIndex = layer.GameObjects.Add(gameObject);
        }
        #endregion
    }

    public class PathItem : Item
    {
        public Vector2[] LocalPoints;
        public Vector2[] WorldPoints;
        public bool IsPolygon;
        public int LineWidth;
        public Color LineColor;

        protected PathItem()
        {
        }
    }

    public partial class TextureItem : Item
    {
        #region Public properties
        /// <summary>
        /// The item's rotation in radians.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The item's scale vector.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The color to tint the item's texture with (use white for no tint).
        /// </summary>
        public Color TintColor;

        /// <summary>
        /// If true, the texture is flipped horizontally when drawn.
        /// </summary>
        public bool FlipHorizontally;

        /// <summary>
        /// If true, the texture is flipped vertically when drawn.
        /// </summary>
        public bool FlipVertically;

        /// <summary>
        /// The path to the texture's filename (including the extension) relative to ContentRootFolder.
        /// </summary>
        public String texture_filename;

        /// <summary>
        /// The texture_filename without extension. For using in Content.Load<Texture2D>().
        /// </summary>
        public String asset_name;

        /// <summary>
        /// The XNA texture to be drawn. Can be loaded either from file (using "texture_filename") 
        /// or via the Content Pipeline (using "asset_name") - then you must ensure that the texture
        /// exists as an asset in your project.
        /// Loading is done in the Item's load() method.
        /// </summary>
        // private Texture2D texture;

        /// <summary>
        /// The item's origin relative to the upper left corner of the texture. Usually the middle of the texture.
        /// Used for placing and rotating the texture when drawn.
        /// </summary>
        public Vector2 Origin;

        public bool isTemplate;
        #endregion

        // Constructor
        protected TextureItem()
        {
        }

        #region Public methods

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);
            var texture = cm.Load<Texture2D>(asset_name);

            prop.SetValue(comp, texture, null);
        }

        /// <summary>
        /// Called by Level.FromFile(filename) on each Item after the deserialization process.
        /// Loads all assets needed by the TextureItem, especially the Texture2D.
        /// You must provide your own implementation. However, you can rely on all public fields being
        /// filled by the level deserialization process.
        /// </summary>
        public override void LoadContent(ContentManager cm, Scene scene, Layer layer)
        {
            var texture = cm.Load<Texture2D>(asset_name);
            var gameObject = new GameObject(Name, Position, scene, layer);

            // Add texture components.
            var texComponent = gameObject.AddComponent<TextureComponent>();
            texComponent.Texture = texture;

            // Add to bag.
            //gameObject.BagIndex = layer.GameObjects.Add(gameObject);
        }
        #endregion
    }

    public partial class PhysicsPath : Item
    {
        public Vector2[] LocalPoints;
        public Vector2[] WorldPoints;
        public bool IsPolygon;
        public int LineWidth;
        public Color LineColor;
        public string PhysicsType;

        public PhysicsPath()
        {
        }

        public override void LoadContent(ContentManager cm, Scene scene, Layer layer)
        {
            var vertices = new List<Vector2>();

            // Get centroid first
            for (int i = 0; i < WorldPoints.Length; i++)
            {
                vertices.Add(ConvertUnits.ToSimUnits(WorldPoints[i]));
            }
            float area = GeometryFunctions.GetSignedArea(vertices);

            if (area < 0)
                vertices.Reverse();

            Vector2 position = GeometryFunctions.GetCentroid(vertices);
            position = position * layer.ScrollSpeed;

            var gameObject = new GameObject(Name, ConvertUnits.ToDisplayUnits(position), scene, layer);

            var physComponent = gameObject.AddComponent<PhysicsComponent>();
            physComponent.Polygon = new List<Vector2>(WorldPoints);
            physComponent.Mass = 10f;
            physComponent.Restitution = 0.5f;
            physComponent.Friction = 0.8f;
            physComponent.IsStatic = PhysicsType != "Dynamic";

            //gameObject.BagIndex = layer.GameObjects.Add(gameObject);
        }
    }

    public partial class PlayerStart : Item
    {
        public override void LoadContent(ContentManager cm, Scene scene, Layer layer)
        {
            var sprite = cm.Load<Texture2D>("CharacterSprites/player_sprite");
            var gameObject = new GameObject(Name, Position * layer.ScrollSpeed, scene, layer);
 
            var charPhysics = gameObject.AddComponent<CharacterPhysicsComponent>();
            charPhysics.StandingJumpCells = new Vector2(3, 2);
            charPhysics.RunningJumpCells = new Vector2(6, 2);
            charPhysics.CollisionShapeCells = new Vector2(1, 3);
            charPhysics.Mass = 5f;  /// 0 Mass should be changed -> That might be the cause for the weird floaty problems that occured -> but need to change jump calculation then!!!!!!!!!

            var charAnim = gameObject.AddComponent<AnimationComponent>();
            charAnim.Sprite = new Animation(sprite, 116, 6);
            charAnim.Sprite.AnimationDictionary.Add("Idle", new AnimationData("Idle", 0.833f, 0, 20, 0, true));
            charAnim.Sprite.AnimationDictionary.Add("Walking", new AnimationData("Walking", 0.34f, 0, 17, 1, true));
            charAnim.Sprite.AnimationDictionary.Add("Running", new AnimationData("Running", 0.51f, 0, 17, 2, true));
            charAnim.Sprite.AnimationDictionary.Add("JumpOff", new AnimationData("JumpOff", 0.34f, 2, 17, 3, false));
            charAnim.Sprite.AnimationDictionary.Add("MidAir", new AnimationData("MidAir", 1, 0, 1, 4, true));
            charAnim.Sprite.AnimationDictionary.Add("Landing", new AnimationData("Landing", 0.28f, 2, 9, 5, false));

            gameObject.AddComponent<PlayerInput>();

            var camera = gameObject.AddComponent<Camera>();
            camera.UpdateViewport(ScreenManager.Instance.GraphicsDevice.Viewport.Width, ScreenManager.Instance.GraphicsDevice.Viewport.Height);

            //gameObject.BagIndex = layer.GameObjects.Add(gameObject);
        }
    }
    
    public class CustomProperty
    {
        #region Public properties
        public string name;
        public object value;
        public Type type;
        public string description;
        #endregion

        // Constructor

        public CustomProperty()
        {
        }


        #region Public methods
        public CustomProperty(string n, object v, Type t, string d)
        {
            name = n;
            value = v;
            type = t;
            description = d;
        }

        public CustomProperty clone()
        {
            CustomProperty result = new CustomProperty(name, value, type, description);
            return result;
        }
        #endregion
    }

    public class SerializableDictionary : Dictionary<String, CustomProperty>, IXmlSerializable
    {
        // Constructor.

        public SerializableDictionary()
            : base()
        {

        }

        #region Public methods
        public SerializableDictionary(SerializableDictionary copyfrom)
            : base(copyfrom)
        {
            string[] keyscopy = new string[Keys.Count];
            Keys.CopyTo(keyscopy, 0);
            foreach (string key in keyscopy)
            {
                this[key] = this[key].clone();
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty) return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                CustomProperty cp = new CustomProperty();
                cp.name = reader.GetAttribute("Name");
                cp.description = reader.GetAttribute("Description");

                string type = reader.GetAttribute("Type");
                if (type == "string") cp.type = typeof(string);
                if (type == "bool") cp.type = typeof(bool);
                if (type == "Vector2") cp.type = typeof(Vector2);
                if (type == "Color") cp.type = typeof(Color);
                if (type == "Item") cp.type = typeof(Item);

                if (cp.type == typeof(Item))
                {
                    cp.value = reader.ReadInnerXml();
                    this.Add(cp.name, cp);
                }
                else
                {
                    reader.ReadStartElement("Property");
                    XmlSerializer valueSerializer = new XmlSerializer(cp.type);
                    object obj = valueSerializer.Deserialize(reader);
                    cp.value = Convert.ChangeType(obj, cp.type);
                    this.Add(cp.name, cp);
                    reader.ReadEndElement();
                }

                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (String key in this.Keys)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", this[key].name);
                if (this[key].type == typeof(string)) writer.WriteAttributeString("Type", "string");
                if (this[key].type == typeof(bool)) writer.WriteAttributeString("Type", "bool");
                if (this[key].type == typeof(Vector2)) writer.WriteAttributeString("Type", "Vector2");
                if (this[key].type == typeof(Color)) writer.WriteAttributeString("Type", "Color");
                if (this[key].type == typeof(Item)) writer.WriteAttributeString("Type", "Item");
                writer.WriteAttributeString("Description", this[key].description);

                if (this[key].type == typeof(Item))
                {
                    Item item = (Item)this[key].value;
                    if (item != null) writer.WriteString(item.Name);
                    else writer.WriteString("$null$");
                }
                else
                {
                    XmlSerializer valueSerializer = new XmlSerializer(this[key].type);
                    valueSerializer.Serialize(writer, this[key].value);
                }
                writer.WriteEndElement();
            }
        }
        #endregion

        /*public void RestoreItemAssociations(Scene scene)
        {
            foreach (CustomProperty cp in Values)
            {
                if (cp.type == typeof(Item)) cp.value = scene.getItemByName((string)cp.value);
            }
        }*/
    }
}
