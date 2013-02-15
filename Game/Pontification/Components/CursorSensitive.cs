using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pontification.SceneManagement;

namespace Pontification.Components
{
    public class CursorSensitive : Component
    {
        private Segment[] _segments;
        private Segment _intersectionLine;
        private Vector2 _oldPosition;
        private AABB _boundingBox;

        public List<Vector2> Polygon { get; set; }
        public CursorState CursorEffect { get; set; }

        public CursorSensitive()
        {
            Polygon = new List<Vector2>();
        }

        public override void Start()
        {
            _intersectionLine = new Segment(Vector2.Zero, Vector2.One);
            _segments = new Segment[Polygon.Count];

            float minX = float.PositiveInfinity; float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity; float maxY = float.NegativeInfinity;
            for (int i = 0; i < Polygon.Count; i++)
            {
                var vertex = Polygon[i];
                if (vertex.X < minX)
                    minX = vertex.X;
                if (vertex.X > maxX)
                    maxX = vertex.X;
                if (vertex.Y < minY)
                    minY = vertex.Y;
                if (vertex.Y > maxY)
                    maxY = vertex.Y;

                int nextIdx = (i + 1) % Polygon.Count;
                _segments[i] = new Segment(Polygon[i], Polygon[nextIdx]);
            }

            var ul = new Vector2(minX, minY);
            var lr = new Vector2(maxX, maxY);
            Vector2 boxCenter = ul + (lr - ul) * 0.5f;

            _boundingBox = new AABB();
            _boundingBox.Offset = boxCenter - GameObject.Position;
            _boundingBox.Position = GameObject.Position + _boundingBox.Offset;
            _boundingBox.XHalfWidth = new Vector2((maxX - minX) / 2, 0);
            _boundingBox.YHalfWidth = new Vector2(0, (maxY - minY) / 2);

            _oldPosition = GameObject.Position;

            SceneInfo.CursorSensitives.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            var position = GameObject.Position;

            if (position != _oldPosition)
            {
               updatePolygon();
            }
            _oldPosition = position;
        }

        #region Private methods.
        /// <summary>
        /// Updates the polyogn with the current position
        /// </summary>
        private void updatePolygon()
        {
            var diff = GameObject.Position - _oldPosition;

            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].SetSegment(_segments[i].P1 + diff, _segments[i].P2 + diff);
            }

            _boundingBox.Position = GameObject.Position;
        }

        /// <summary>
        /// Checks if cursor hovers over something interesting.
        /// </summary>
        public bool CheckHovering(Vector2 position)
        {
            // Check bounding box first.
            if (position.X < (_boundingBox.Position.X - _boundingBox.XHalfWidth.X))
                return false;
            if (position.X > (_boundingBox.Position.X + _boundingBox.XHalfWidth.X))
                return false;
            if (position.Y < (_boundingBox.Position.Y - _boundingBox.YHalfWidth.Y))
                return false;
            if (position.Y > (_boundingBox.Position.Y + _boundingBox.YHalfWidth.Y))
                return false;

            var startPos = position - Vector2.UnitX * 5000;

            _intersectionLine.SetSegment(startPos, position);
            var intersections = intersectWithPolygon(_intersectionLine);

            if ((intersections & 1) == 1)
            {
                return true;
            }

            return false;
        }

        public void DeactivateSensitivity()
        {
            IsActive = false;
            Cursor.State = CursorState.CS_IDLE;
        }

        private int intersectWithPolygon(Segment line) 
        {
            var intersections = 0;
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
                intersections++;
            }

            return intersections; 
        }
        #endregion

        protected override void disposing()
        {
            SceneInfo.CursorSensitives.Remove(this);
        }

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
