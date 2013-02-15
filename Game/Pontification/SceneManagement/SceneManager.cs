using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Pontification.ScreenManagement;

namespace Pontification.SceneManagement
{
    public class SceneManager
    {
        private static readonly SceneManager _instance;     
        public static SceneManager Instance { get { return _instance; } }

        #region Private attributes
        private List<Scene> _loadedScenes = new List<Scene>();
        private SceneGraph _sceneGraph = new SceneGraph();
        private static readonly string _configFileEnding = "ini";
        private static readonly int _preloadRange = 0;  // Specifies how far the scene manager reaches into the scene graph to preload scenes.
        #endregion

        #region Public properties
        public Scene FocusScene { get; private set; }
        #endregion

        // Constructors.
        static SceneManager()
        {
            _instance = new SceneManager();
        }

        private SceneManager()
        {
            readFromConfigFile();

            _sceneGraph.Reset();
        }

        #region Public methods
        public Scene GetLoadedScene(string name)
        {
            return _loadedScenes.Find((scene) => { return scene.SceneName == name; });
        }

        public void GoToScene(string name)
        {
            _sceneGraph.MoveTo(name);
            UpdateScenes();
        }

        public void UpdateScenes()
        {
            // Load net around new focus
            addSceneNet(_sceneGraph.Focus, 0);
            trimSceneNet(_sceneGraph.Focus, 0);
            _sceneGraph.ResetVisits();

            // Set focus to active scene.
            FocusScene = GetLoadedScene(_sceneGraph.Focus.SceneName);
            ScreenManager.Instance.SetFocusOnScreen(FocusScene.CurrentScreen);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Recursive function, adds the scene net around the the first node passed to the function.
        /// The net has the the depth specified in the private attribute _preloadRange.
        /// </summary>
        /// <param name="node">SceneNode which should be used to create and add the new scene</param>
        /// <param name="depth">Current depth in the scene graph around the first node passed to the function</param>
        /// <param name="parent">Optional parent node</param>
        private void addSceneNet(SceneNode node, int depth, SceneNode parent = null)
        {
            loadScene(node);
            
            node.IsVisited = true;

            if (depth < _preloadRange)
            {
                node.NeighbourNodes.ForEach((neighbour) => 
                {
                    if (!neighbour.IsVisited)
                        addSceneNet(neighbour, depth + 1, node);
                });
            }
        }

        /// <summary>
        /// Trims the scene net around the node passed first to the function (with no parent node).
        /// Looks up all nodes exceeding _preloadRange and removes them.
        /// </summary>
        /// <param name="node">Current node to remove</param>
        /// <param name="depth">Current depth</param>
        /// <param name="parent">Parent node</param>
        private void trimSceneNet(SceneNode node, int depth, SceneNode parent = null)
        {
            if (depth <= _preloadRange)
            {
                node.NeighbourNodes.ForEach((neighbour) =>
                {
                    trimSceneNet(neighbour, depth + 1, node);
                });
            }
            else
            {
                // We reached trim depth -> remove scene.
                if (!node.IsVisited)
                {
                    unloadScene(node);
                    node.IsVisited = true; 
                    node.NeighbourNodes.ForEach((neighbour) =>
                    {
                        trimSceneNet(neighbour, depth + 1, node);
                    });
                }
            }
        }

        /// <summary>
        /// Loads the scene from the xml file, with the node's SceneName file name. If the scene is already
        /// loaded and part of _loadedScenes it is not loaded from the file. Loads the scene into the screen
        /// manager as well. If the scene is already present in the screen manager it does nothing.
        /// </summary>
        /// <param name="node">SceneNode used to load the scene</param>
        /// <param name="addToScreenManager">If true creates a new screen and add it to the screen manager</param>
        private void loadScene(SceneNode node)
        {
            Scene scene = _loadedScenes.Find((s) => { return s.SceneName == node.SceneName; });
            if (scene == null)
            {
                // Load from file then.
                scene = Scene.LoadFromFile(node.SceneName);
            }

            if (scene.CurrentScreen == null)
            {
                // Add to screen manager if we want to update the objects within the scene also in the preload.
                ScreenManager.Instance.AddScreen(new GameScreen(scene, node.TransitionOnDuration, node.TransitionOffDuration, node.IsPopup, node.IsTile));
            }

            // Add to scene list
            _loadedScenes.Add(scene);
        }

        /// <summary>
        /// Removes scene from the _loadedScenes list if present and also exits the screen associated with the scene.
        /// </summary>
        /// <param name="node">ScreenNode which specifies the scene tro remove</param>
        private void unloadScene(SceneNode node)
        {
            Console.WriteLine(string.Format("Unload scene: {0}", node.SceneName));
            Scene scene = _loadedScenes.Find((s) => { return s.SceneName == node.SceneName; });

            if (scene != null)
            {
                _loadedScenes.Remove(scene);
                // RemoveScreen calls the unload methods and does all the clean up.
                ScreenManager.Instance.RemoveScreen(scene.CurrentScreen);
            }
        }
        #endregion

        private void readFromConfigFile()
        {
            using (StreamReader sr = new StreamReader(string.Format("Config/SceneSettings.{0}", _configFileEnding)))
            {
                // Read file line by line
                while (!sr.EndOfStream)
                {
                    // Read in scene graph
                    //
                    string line = sr.ReadLine().Replace(" ",string.Empty);

                    // Split node name from neightbours
                    string[] primSplit = line.Split(':');
                    string name = primSplit[0];
                    string[] neightbours = primSplit[1].Split(',');

                    // Add node to graph
                    _sceneGraph.Add(name, neightbours);
                }
                sr.Close();
            }
        }

        #region Scene graph implementation
        private class SceneGraph
        {
            private List<SceneNode> _nodes = new List<SceneNode>();
            private SceneNode _root;

            public SceneNode Focus; // Current focus of the SceneGraph.

            public SceneGraph()
            {
            }

            /// <summary>
            /// Adds a new SceneNode to the scene graph with the given name and neighbours. Makes sure that all scene names
            /// appearing in the scene graph nodes are unique.
            /// </summary>
            /// <param name="name">Scene name of the SceneNode</param>
            /// <param name="neighbours">Neighbours of the SceneNode</param>
            public void Add(string name, string[] neighbours)
            {
                // Make sure we don't add nodes with the same scene name. If node with the same scene name already exists.
                // just use that reference.
                SceneNode node = _nodes.Find((sn) => { return sn.SceneName == name; });

                if (node == null)
                {
                    // If not yet in the graph add a new node.
                    node = new SceneNode(name);
                }

                // First element added will be the root.
                if (_nodes.Count <= 0)
                {
                    _root = node;
                }
                _nodes.Add(node);
                AddNeighbours(node, neighbours);
            }

            /// <summary>
            /// Moves the focus of the graph to the SceneNode with the specified SceneName, if the SceneNode
            /// can be reached from the current SceneNode in focus.
            /// </summary>
            /// <param name="name">Name of the scene we want to move</param>
            /// <returns>Returns false if SceneNode with specified scene name can't be reached from current Focus or Focus has never been set (call SceneGraph.Reset first)</returns>
            public bool MoveTo(string name)
            {
                if (Focus == null)  // We are not initialized yet.
                    return false;

                SceneNode moveNode = Focus.NeighbourNodes.Find((n) => { return n.SceneName == name; });

                if (moveNode == null)
                    return false;

                Focus = moveNode;
                return true;
            }

            // Get the SceneNode with the specified name.
            public SceneNode GetNode(string name)
            {
                return _nodes.Find((n) => { return n.SceneName == name; });
            }

            // Resets SceneGraphs focus back to the root.
            public void Reset()
            {
                Focus = _root;
                ResetVisits();
            }

            public void ResetVisits()
            {
                _nodes.ForEach((n) => { n.IsVisited = false; });
            }

            // Add neighbours and make sure that we just add the reference if the node already exists in the graph.
            private void AddNeighbours(SceneNode node, string[] neighbours)
            {
                for (int i = 0; i < neighbours.Length; i++)
                {
                    string neighbourName = neighbours[i];
                    SceneNode neighbour = GetNode(neighbourName);
                    if (neighbour == null)
                    {
                        // Create new one
                        neighbour = new SceneNode(neighbourName);
                        _nodes.Add(neighbour);
                    }
                    node.AddNeighbour(neighbour);
                }
            }
        }
        private class SceneNode
        {
            public List<SceneNode> NeighbourNodes = new List<SceneNode>();
            public string SceneName { get; private set; }

            public TimeSpan TransitionOnDuration { get; private set; }
            public TimeSpan TransitionOffDuration { get; private set; }
            public bool IsPopup { get; private set; }
            public bool IsTile { get; private set; }

            public bool IsVisited { get; set; }

            public SceneNode(string sceneName)
            {
                SceneName = sceneName;
            }

            // Adds neighbour to the list, and adds nothing if the neighbour is already in the list
            public void AddNeighbour(SceneNode node)
            {
                if (NeighbourNodes.Contains(node))
                    return;
                NeighbourNodes.Add(node);
            }

            public override string ToString()
            {
                return string.Format("Scene name: {0}; Neighbours: {1};", SceneName, NeighbourNodes.Count);
            }
        }
        #endregion
    }    
}
