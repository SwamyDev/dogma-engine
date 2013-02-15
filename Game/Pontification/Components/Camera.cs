using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.ScreenManagement;

namespace Pontification.Components
{
    public class Camera : Component
    {
        #region Private attributes
        private static Matrix[] _views;
        private static Vector2[] _parallaxes;
        private static Vector2 _viewport;                //width and height of the viewport
        private static Segment _leftBorder;
        private static Segment _topBorder;
        private static Segment _rightBorder;
        private static Segment _bottomBorder;

        private static Segment[] _segments;
        private static float _camPolyMinX;
        private static float _camPolyMaxX; 
        private static float _camPolyMinY;
        private static float _camPolyMaxY;

        private static Vector2 _position;
        private static float _rotation;
        private static float _scale;

        private static Vector2 _oldPosition;
        private static int _viewCount;
        private static bool _initialized;
        #endregion

        #region Public properties
        public static Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                updatematrix();
            }
        }

        public static float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                updatematrix();
            }
        }

        public static float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                updatematrix();
            }
        }

        public static Matrix NormalView { get; set; }
        public static bool IsFrozen { get; private set; }

        public List<Vector2> CameraPolygon { get; set; }
        #endregion

        public Camera()
        {
            if (_initialized)
                throw new NotSupportedException("You can only have one Camera Component in one game");

            _initialized = true;
            _position = Vector2.Zero;
            _rotation = 0;
            _scale = 1.0f;
            updatematrix();
        }

        #region Public methods
        public override void Start()
        {
            _views = new Matrix[GameObject.Scene.Layers.Count];
            _parallaxes = new Vector2[GameObject.Scene.Layers.Count];

            _camPolyMinX = CameraPolygon.Max((vertex) => { return vertex.X; });
            _camPolyMaxX = CameraPolygon.Min((vertex) => { return vertex.X; });

            _camPolyMinY = CameraPolygon.Max((vertex) => { return vertex.Y; });
            _camPolyMaxY = CameraPolygon.Min((vertex) => { return vertex.Y; });

            _segments = new Segment[CameraPolygon.Count];
            for (int i = 0; i < _segments.Length; i++)
            {
                int nextIdx = (i + 1) % _segments.Length;

                _segments[i] = new Segment(CameraPolygon[i], CameraPolygon[nextIdx]);
            }
        }

        /// <summary>
        /// Get's the view matrix with the given parallax
        /// </summary>
        /// <param name="parallax">parallax</param>
        /// <returns>View matrix</returns>
        public static Matrix GetView(Vector2 parallax)
        {
            int viewIdx = getParallaxIdx(parallax);

            if (viewIdx >= 0 && viewIdx < _viewCount)
                return GetView(viewIdx);

            return Matrix.Identity;
        }

        /// <summary>
        /// Returns the view matrix with the given index.
        /// </summary>
        /// <param name="index">View index</param>
        /// <returns>View Matrix</returns>
        public static Matrix GetView(int index)
        {
            return _views[index];
        }

        /// <summary>
        /// Adds a new view with the specified parallax scrolling
        /// </summary>
        /// <param name="parallax">parallax scrolling</param>
        /// <returns>View index</returns>
        public static int AddView(Vector2 parallax)
        {
            if (!_parallaxes.Any((v) => { return v.X == parallax.X && v.Y == parallax.Y; }))
            {
                // Add new parallax view
                _parallaxes[_viewCount] = parallax;
                _views[_viewCount] = Matrix.Identity;

                return _viewCount++;
            }
            else
            {
                return getParallaxIdx(parallax);
            }
        }

        public override void Update(GameTime gameTime)
        {
            updatePosition();
        }

        public void UpdateViewport(float width, float height)
        {
            _viewport.X = width;
            _viewport.Y = height;
            updatematrix();
        }

        public void UnFreeze()
        {
            IsFrozen = false;
        }

        /*
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, GameTime gameTime)
        {
            foreach (var segment in _segments)
            {
                Primitives.Instance.DrawLine(sb, segment.P1, segment.P2, Color.Red, 2);
            }
            Primitives.Instance.DrawLine(sb, _topBorder.P1, _topBorder.P2, Color.Green, 2);
            Primitives.Instance.DrawLine(sb, _bottomBorder.P1, _bottomBorder.P2, Color.Green, 2);
            Primitives.Instance.DrawLine(sb, _leftBorder.P1, _leftBorder.P2, Color.Green, 2);
            Primitives.Instance.DrawLine(sb, _rightBorder.P1, _rightBorder.P2, Color.Green, 2);
        }*/
        #endregion

        #region Private method
        private static void updatematrix()
        {
            bool isNormalViewSet = false;
            for (int i = 0; i < _viewCount; i++)
            {
                _views[i] = Matrix.CreateTranslation(-_position.X * _parallaxes[i].X, -_position.Y * _parallaxes[i].Y, 0.0f) *
                     Matrix.CreateRotationZ(_rotation) *
                     Matrix.CreateScale(_scale) *
                     Matrix.CreateTranslation(_viewport.X / 2, _viewport.Y / 2, 0.0f);

                if (!isNormalViewSet && _parallaxes[i].X == 1 && _parallaxes[i].Y == 1)
                {
                    isNormalViewSet = true;
                    NormalView = _views[i];
                }
            }
        }

        private static int getParallaxIdx(Vector2 parallax)
        {
            for (int i = 0; i < _viewCount; i++)
            {
                if (_parallaxes[i].X == parallax.X && _parallaxes[i].Y == parallax.Y)
                    return i;
            }

            return -1;
        }

        private void updatePosition()
        {
            if (IsFrozen)
                return;

            var newPos = GameObject.Position;
            var diff = newPos - _oldPosition;

            if (diff != Vector2.Zero)    // Only update Matrix if neccessary.
            {
                // Snap to camera polygon.
                Vector2 collisionPoint = Vector2.Zero;

                updateViewportBorders(newPos);

                // Test vertical borders
                if (intersectWithCameraPolygon(_leftBorder, out collisionPoint))
                {
                    /*if (Math.Abs(collisionPoint.Y - newPos.Y) <= 20)
                        IsFrozen = true;*/

                    if (collisionPoint.Y >= newPos.Y)   // Collision happens at the bottom border.
                    {
                        newPos.Y -= _leftBorder.P1.Y - collisionPoint.Y + 1;
                        updateViewportBorders(newPos);
                    }
                    else
                    {
                        newPos.Y -= _leftBorder.P2.Y - collisionPoint.Y;
                        updateViewportBorders(newPos);
                    }
                }
                else if (intersectWithCameraPolygon(_rightBorder, out collisionPoint))
                {
                    /*if (Math.Abs(collisionPoint.Y - newPos.Y) <= 20)
                        IsFrozen = true;*/

                    if (collisionPoint.Y >= newPos.Y)   // Collision happens at the bottom border.
                    {
                        newPos.Y -= _rightBorder.P1.Y - collisionPoint.Y + 1;
                        updateViewportBorders(newPos);
                    }
                    else
                    {
                        newPos.Y -= _leftBorder.P2.Y - collisionPoint.Y;
                        updateViewportBorders(newPos);
                    }
                }

                // Test horizontal borders
                if (intersectWithCameraPolygon(_topBorder, out collisionPoint))
                {
                    /*if (Math.Abs(collisionPoint.X - newPos.X) <= 20)
                        IsFrozen = true;*/

                    if (collisionPoint.X <= newPos.X)   // Collision happens on the left
                    {
                        newPos.X -= _topBorder.P1.X - collisionPoint.X;
                        updateViewportBorders(newPos);
                    }
                    else
                    {
                        newPos.X -= _topBorder.P2.X - collisionPoint.X;
                        updateViewportBorders(newPos);
                    }
                }
                else if (intersectWithCameraPolygon(_bottomBorder, out collisionPoint))
                {
                    /*if (Math.Abs(collisionPoint.X - newPos.X) <= 20)
                        IsFrozen = true;*/

                    if (collisionPoint.X <= newPos.X)   // Collision happens on the left
                    {
                        newPos.X -= _bottomBorder.P1.X - collisionPoint.X;
                        updateViewportBorders(newPos);
                    }
                    else
                    {
                        newPos.X -= _bottomBorder.P2.X - collisionPoint.X;
                        updateViewportBorders(newPos);
                    }
                }

                Position = newPos;
            }

            _oldPosition = Position;
        }

        private void updateViewportBorders(Vector2 pos)
        {
            Vector2 topLeft = pos - _viewport / 2;
            Vector2 topRight = topLeft + Vector2.UnitX * _viewport.X;
            Vector2 bottomRight = topRight + Vector2.UnitY * _viewport.Y;
            Vector2 bottomLeft = bottomRight - Vector2.UnitX * _viewport.X;

            if (_leftBorder == null)
                _leftBorder = new Segment(bottomLeft, topLeft);
            else
                _leftBorder.SetSegment(bottomLeft, topLeft);

            if (_topBorder == null)
                _topBorder = new Segment(topLeft, topRight);
            else
                _topBorder.SetSegment(topLeft, topRight);

            if (_rightBorder == null)
                _rightBorder = new Segment(bottomRight, topRight);
            else
                _rightBorder.SetSegment(bottomRight, topRight);

            if (_bottomBorder == null)
                _bottomBorder = new Segment(bottomLeft, bottomRight);
            else
                _bottomBorder.SetSegment(bottomLeft, bottomRight);
        }

        private bool intersectWithCameraPolygon(Segment line, out Vector2 collisionPoint) 
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < _segments.Length; i++)
            {
                Segment edge = _segments[i];

                float A1 = line.A; float B1 = line.B; float C1 = line.C;
                float A2 = edge.A; float B2 = edge.B; float C2 = edge.C;

                // Calculate line intersection
                float det = A1 * B2 - A2 * B1;

                if (det == 0f)   // Lines are parallel
                    continue; 
                
                float sx = (B2 * C1 - B1 * C2) / det;
                float sy = (A1 * C2 - A2 * C1) / det;

                // Test if point is on line segments - we only need to test one projection of each segment.
                if (Math.Abs(Vector2.Dot(line.Direction, new Vector2(1, 0))) > 0.3f)
                {
                    if (sx < Math.Min(line.P1.X, line.P2.X) || sx > Math.Max(line.P1.X, line.P2.X))
                        continue;
                }
                else
                {
                    if (sy < Math.Min(line.P1.Y, line.P2.Y) || sy > Math.Max(line.P1.Y, line.P2.Y))
                        continue;
                }
                if (Math.Abs(Vector2.Dot(edge.Direction, new Vector2(1, 0))) > 0.3f)
                {
                    if (sx < Math.Min(edge.P1.X, edge.P2.X) || sx > Math.Max(edge.P1.X, edge.P2.X))
                        continue;
                }
                else
                {
                    if (sy < Math.Min(edge.P1.Y, edge.P2.Y) || sy > Math.Max(edge.P1.Y, edge.P2.Y))
                        continue;
                }

                // Collision detected.
                collisionPoint =  new Vector2(sx, sy);
                return true;
            }

            return false; 
        }
        #endregion

        #region Segment private class
        private class Segment
        {
            public Vector2 P1 { get; private set; }
            public Vector2 P2 { get; private set; }
            public Vector2 Direction { get; private set; }
            public float A { get; private set; }
            public float B { get; private set; }
            public float C { get; private set; }

            public Segment(Vector2 p1, Vector2 p2)
            {
                P1 = p1;
                P2 = p2;

                Direction = Vector2.Normalize(P2 - P1);

                A = p2.Y - p1.Y;
                B = p1.X - p2.X;
                C = A * p1.X + B * p1.Y;
            }

            public void SetSegment(Vector2 p1, Vector2 p2)
            {
                P1 = p1;
                P2 = p2;

                Direction = Vector2.Normalize(P2 - P1);

                A = p2.Y - p1.Y;
                B = p1.X - p2.X;
                C = A * p1.X + B * p1.Y;
            }
        }
        #endregion
    }
}
