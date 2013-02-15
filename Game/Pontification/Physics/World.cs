using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Components;

namespace Pontification.Physics
{
    public class World
    {
        public Vector2 Gravity;

        public Vector2 Origin = new Vector2(-10, -50);

        private const int _gridWidth = 200;
        private const int _gridHeight = 240;

        private Bag<PhysicsObject>[,] _grid = new Bag<PhysicsObject>[_gridWidth, _gridHeight];
        private float _cellSize = 3f;

        private Vector2 _offset;

        private Bag<StaticObject> _staticObjects = new Bag<StaticObject>();
        private Bag<DynamicObject> _dynamicObjects = new Bag<DynamicObject>();
        private Bag<Sensor> _sensors = new Bag<Sensor>();

        private List<Line> _drawLines = new List<Line>();
        private List<Vector2> _drawPoints = new List<Vector2>();

        public World(Vector2 gravity)
        {
            Gravity = gravity;

            /*_whiteTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _whiteTexture.SetData(new[] { Color.White });

            _redTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _redTexture.SetData(new[] { new Color(255, 0, 0, 255) });

            VertexTexture = new Texture2D(game.GraphicsDevice, 10, 10);

            Color[] data = new Color[100];
            for (int i = 0; i < 100; i++)
            {
                data[i] = Color.Green;
            }
            VertexTexture.SetData(data);

            AABBTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            AABBTexture.SetData(new[] { Color.Silver });

            LineTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            LineTexture.SetData(new[] { Color.Yellow });

            NormTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            NormTexture.SetData(new[] { Color.Blue });*/

            _offset = new Vector2(ConvertUnits.ToSimUnits(0), ConvertUnits.ToSimUnits(0)) + Origin;
        }

        public delegate bool RayCastCallback(PhysicsObject physicsObject, Vector2 point, Vector2 normal);

        public void RayCast(RayCastCallback callback, Vector2 start, Vector2 end, bool ignoreSensors = true, bool ignoreProjectile = true)
        {
            // Use Bresenham's line algorithm to check cells along the ray          IMPROVE THAT AS IT'S NOT 100% RELIABLE BRESENHAM CELLS DON'T CATCH ALL POSSIBLE COLLIDERS NEEDS MORE THAN ONE CELL THICK       !!! !!! !!!
            //

            Line line = new Line(start, end);
            _drawLines.Add(line);

            // Get starting end and positions.
            Vector2 pos1 = start - Origin;
            Vector2 pos2 = end - Origin;
            int x1 = (int)(pos1.X / _cellSize); int x2 = (int)(pos2.X / _cellSize);
            int y1 = (int)(pos1.Y / _cellSize); int y2 = (int)(pos2.Y / _cellSize);

            int deltaX = Math.Abs(x1 - x2);
            int deltaY = Math.Abs(y1 - y2);

            // Set signum values.
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;

            int error = deltaX - deltaY;

            bool bFirst = true;
            while (true)
            {
                bool bLast = (x1 == x2 && y1 == y2);

                // Stop if we found the collision.
                if (CheckLineCollisions(x1, y1, line, callback, bFirst, bLast, ignoreSensors, ignoreProjectile))
                    break;

                // If we reached last cell break out of loop.
                if (bLast)
                    break;
                if (bFirst)
                    bFirst = false;

                // Double error to avoid using floats.
                int doubleError = error * 2;

                if (doubleError > -deltaY)
                {
                    // Check cells inbetween to make sure.
                    int y1b = y1 + signY;
                    if (checkLineInCell(x1, y1b, line))
                    {
                        if (CheckLineCollisions(x1, y1b, line, callback, bFirst, bLast, ignoreSensors, ignoreProjectile))
                            break;
                    }

                    // Increment in x direction.
                    error -= deltaY;
                    x1 += signX;
                }
                if (doubleError < deltaX)
                {
                    // Check cells inbetween to make sure.
                    int x1b = x1 + signX;
                    if (checkLineInCell(x1b, y1, line))
                    {
                        if (CheckLineCollisions(x1 + signX, y1, line, callback, bFirst, bLast, ignoreSensors, ignoreProjectile))
                            break;
                    }

                    // Increment in y direction.
                    error += deltaX;
                    y1 += signY;
                }
            }
        }

        private bool CheckLineCollisions(int col, int row, Line line, RayCastCallback callback, bool isFirst, bool isLast, bool ignoreSensor, bool ignoreProjectile)
        {
            Bag<PhysicsObject> possibleColliders = _grid[col, row];
            Vector2 collisionPoint = line.P2;
            Vector2 collisionNormal = Vector2.Zero;
            PhysicsObject collidingObject = null;
            float distance = float.PositiveInfinity;

            if (possibleColliders == null)
                return false;

            /// I actually don't now why I put this in - it may causes bugs in future but for the moment it seems to work when commented.
            /*if (possibleColliders.Count > 0)
            {   
                // Cut out segment within the current cell.
                //

                // Create a sensor within the cell to collide with
                float xCell = (col + 0.5f) * _cellSize + Origin.X;
                float yCell = (row + 0.5f) * _cellSize + Origin.Y;

                var cellCollider = new Sensor(this, new Vector2(xCell, yCell), new Vector2(_cellSize, _cellSize));

                var newStart = Vector2.Zero;
                var newEnd = Vector2.Zero;
                var newNorm = Vector2.Zero;
                var mirrorLine = new Line(line.P2, line.P1);

                if (!CollideLinePoly(line, (Polygon)cellCollider.CollisionShape, out newStart, out newNorm))
                {
                    newStart = line.P1;
                }
                if (!CollideLinePoly(mirrorLine, (Polygon)cellCollider.CollisionShape, out newEnd, out newNorm))
                {
                    newEnd = line.P2;
                }

                line = new Line(newStart, newEnd);

                cellCollider.Dispense(); // Remove from world
            }*/
            
            // Test each object to collide with line.
            possibleColliders.ForEachWith((collider) =>
            {
                if (ignoreSensor)
                {
                    if (collider.IsSensor)
                        return;
                }

                if (ignoreProjectile)
                {
                    if (collider.IsProjectile)
                        return;
                }

                if (collider.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                {
                    var poly = collider.CollisionShape as Polygon;
                    if (isFirst)
                    {
                        if (CollidePointPoly(line.P1, poly))
                            return;
                    }

                    Vector2 curCollisionPoint, curCollisionNormal;
                    if (CollideLinePoly(line, poly, out curCollisionPoint, out curCollisionNormal))
                    {
                        // Check for closest point
                        float curDistance = (curCollisionPoint - line.P1).Length();
                        if (curDistance < distance)
                        {
                            distance = curDistance;
                            collidingObject = collider;
                            collisionPoint = curCollisionPoint;
                            collisionNormal = curCollisionNormal;
                        }
                    }
                }
            }, (collider) => { return collider.IsActive; });

            if (collidingObject != null)
            {
                callback(collidingObject, collisionPoint, collisionNormal);
                _drawPoints.Add(collisionPoint);
                return true;
            }

            return false;
        }

        public Bag<PhysicsObject> TestPointAll(Vector2 point)
        {
            Vector2 pos = point - Origin;
            var col = (int)(pos.X / _cellSize);
            var row = (int)(pos.Y / _cellSize);

            Bag<PhysicsObject> possibleObjects = _grid[col, row];
            Bag<PhysicsObject> collidingObjects = new Bag<PhysicsObject>();

            if (possibleObjects == null)
                return collidingObjects;

            possibleObjects.ForEachWith((collider) =>
            {
                if (collider.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                {
                    var poly = collider.CollisionShape as Polygon;

                    if (CollidePointPoly(point, poly))
                        collidingObjects.Add(collider);
                }
            }, (collider) => { return collider.IsActive; });

            return collidingObjects;
        }

        /**
         * Call this method to invoke a collision test for the given sensor. 
         * It immediatly fires the OnCollision event.
         */
        public void TestSensor(Sensor sensor)
        {
            CheckCollisions(sensor, 0f);
        }

        public void Update(float deltaTime)
        {
            // Update all dynamic objects.
            _dynamicObjects.ForEachWith((collider) =>
            {
                // Update dynamic objects.
                collider.Update(deltaTime);

                #region Update cells
                // Don't update when sleeping.
                if (!collider.bSleeps)
                {
                    // Remove object from grid.
                    collider.ContainingCells.ForEach((cell) => { _grid[cell.Column, cell.Row].Remove(collider); }); /// MAKE THAT FASTER WITH A DIFFERENT LIST ITEM WITH FAST ACCESS INSTEAD OF A LIST FOR CONTAINING CELLS
                    collider.ContainingCells.Clear();
                    if (collider.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                    {
                        var poly = collider.CollisionShape as Polygon;
                        for (int i = 0; i < poly.Edges.Length; i++)
                            AddEdgeToGrid(poly.Edges[i], collider);
                    }
                }
                #endregion

                if (collider.MotionState == DynamicObject.MotionStates.MS_LANDED)
                {
                    // Dip down to test for ground.
                    collider.Position += collider.StandOnSurface * 0.01f;
                }
                collider.MotionState = DynamicObject.MotionStates.MS_MIDAIR;
                CheckCollisions(collider, deltaTime);
            }, (collider) => { return collider.IsActive; });
        }

        private void CheckCollisions(PhysicsObject physicsObject, float deltaTime)
        {
            // Check collision with all objects within our cells

            Bag<PhysicsObject> collidedObjects = new Bag<PhysicsObject>();

            physicsObject.ContainingCells.ForEach((cell) => 
            { 
                Bag<PhysicsObject> possibleColliders = _grid[cell.Column, cell.Row];
                possibleColliders.ForEachWith((collider) => 
                {
                    /// IMPLEMENT BETTER MOUDLAR VERSION TO FIGURE OUT IF OBJECTS SHOULD COLLIDE. MAYBE SOMETHING LIKE BITWISE COLLISION GROUPS ETC...
                    if (physicsObject is CharacterObject && collider is CharacterObject)
                        return;

                    #region Depricated collision filters
                    /// IMPLEMENT A BETTER MODULAR VERSION TO FIGURE OUT COLLISIONS. MAYBE USE BITWISE COLLISION GROUPS ETC...
                    /*if (physicsObject.Owner != null && physicsObject.Owner.Type == Actor.ActorType.Spirit)
                    {
                        // Spirits don't collide with objects
                        if (curCollider.Owner != null && curCollider.Owner.Type == Actor.ActorType.Object)
                            continue;
                    }
                    if (curCollider.Owner != null && curCollider.Owner.Type == Actor.ActorType.Spirit)
                    {
                        // Spirits don't collide with objects
                        if (physicsObject.Owner != null && physicsObject.Owner.Type == Actor.ActorType.Object)
                            continue;
                    }*/
                    #endregion

                    // Don't collide with ourselfs.
                    if (collider == physicsObject)
                        return;

                    // Has this object already been checked
                    if (collider.IsCollisionChecked)
                        return;

                    #region Collide with AABB shape
                    // Coarse collision.
                    AABB A = physicsObject.BoundingBox;
                    AABB B = collider.BoundingBox;

                    float xDist = Math.Abs(A.Position.X - B.Position.X);
                    float yDist = Math.Abs(A.Position.Y - B.Position.Y);

                    // Project half widths.
                    float xProj = A.XHalfWidth.X + B.XHalfWidth.X;
                    float yProj = A.YHalfWidth.Y + B.YHalfWidth.Y;

                    // Early exit.
                    if (xProj < xDist)
                        return;
                    if (yProj < yDist)
                        return;
                    #endregion

                    // If we should test AABB only we are done now.
                    if (physicsObject.TestAABBOnly && collider.TestAABBOnly)
                    {
                        #region Test AABB only
                        // Calculate projection vector.
                        Vector2 projectionVector;
                        Vector2 diff = B.Position - A.Position;
                        float xDiff = ((xProj - xDist) * 1.0f) + ConvertUnits.ToSimUnits(1);
                        float yDiff = ((yProj - yDist) * 1.0f) + ConvertUnits.ToSimUnits(1);

                        if (xDiff < yDiff)
                            projectionVector = new Vector2(xDiff * Math.Sign(diff.X), 0);
                        else
                            projectionVector = new Vector2(0, yDiff * Math.Sign(diff.Y));

                        // Are we dealing with sensors
                        if (!physicsObject.IsSensor && !collider.IsSensor)
                        {
                            // Get colliding edge (O(4))
                            Edge collidingEdge = null;
                            if (collider.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                            {
                                var poly = collider.CollisionShape as Polygon;
                                var projDir = Vector2.Normalize(projectionVector);

                                for (int i = 0; i < poly.Edges.Length; i++)
                                {
                                    var edge = poly.Edges[i];
                                    if (Vector2.Dot(projDir, edge.Normal) > 0.9f)
                                        collidingEdge = edge;
                                }
                            }
                            physicsObject.Collided(collider, collidingEdge, projectionVector, deltaTime);
                        }
                        if (physicsObject.OnCollision != null)
                            physicsObject.OnCollision(physicsObject, collider, projectionVector);
                        if (collider.OnCollision != null)
                            collider.OnCollision(collider, physicsObject, Vector2.Zero);

                        collider.IsCollisionChecked = true;
                        collidedObjects.Add(collider);
                        #endregion
                    }
                    else
                    {
                        #region Test Polygon
                        if (physicsObject.CollisionShape.Type == Shape.ShapeType.SH_POLYGON &&
                            collider.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                        {
                            if (CollidePolyPoly(physicsObject.CollisionShape as Polygon, collider.CollisionShape as Polygon, deltaTime))
                                collidedObjects.Add(collider);  // Flag object as checked and add to collided list.
                        }
                        #endregion
                    }
                }, (collider) => { return collider.IsActive; });
            });

            // Uncheck collision flags for next object
            collidedObjects.ForEach((collider) => { collider.IsCollisionChecked = false; });
        }

        private bool CollidePointPoly(Vector2 point, Polygon polygon)
        {
            bool bInside = true;
            for (int j = 0; j < polygon.Edges.Length; j++)
            {
                Edge edge = polygon.Edges[j];
                Vector2 dir = edge.P1 - point;

                if (Vector2.Dot(edge.Normal, dir) < 0)
                {
                    // Poitn lies outside.
                    bInside = false;
                    break;
                }
            }

            return bInside;
        }

        /**
         * Test collision of a line and a polygon
         */
        private bool CollideLinePoly(Line line, Polygon poly, out Vector2 collisionPoint, out Vector2 collisionNormal)
        {
            collisionPoint = Vector2.Zero;
            collisionNormal = Vector2.Zero;

            for (int i = 0; i < poly.Edges.Length; i++)
            {
                Edge edge = poly.Edges[i];

                // We don't need to consider edges which face away.
                // We also ignore when the ray exactly lies on the edge
                if (Vector2.Dot(line.Direction, edge.Normal) >= 0)
                    continue;

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
                collisionPoint = new Vector2(sx, sy);
                collisionNormal = edge.Normal;

                return true;
            }

            return false;
        }

        /**
         * Test collision of two polygons
         */
        private bool CollidePolyPoly(Polygon A, Polygon B, float deltaTime)
        {
            Edge collidingEdge = null;
            Vector2 diff = B.Owner.Position - A.Owner.Position;
            Vector2 projDirection = Vector2.Zero;
            float minOverlap = float.PositiveInfinity;
            int edgeCount = A.Edges.Length + B.Edges.Length;

            // Test all possible axes of both polygons
            for (int i = 0; i < edgeCount; i++)
            {
                Edge edge;
                if (i < A.Edges.Length)
                {
                    // Check edges of polygon A
                    edge = A.Edges[i];
                }
                else
                {
                    // Check edges of polygon B
                    edge = B.Edges[i - A.Edges.Length];
                }

                Vector2 axis = edge.Normal;

                /*var test = Vector2.Dot(Vector2.Normalize(diff), axis);
                if (Vector2.Dot(diff, axis) > 0)    //Edge is facing away and can't seperate
                    continue;*/

                // Project polygon on axis
                //                

                float minA, maxA;
                ProjectPolygon(A, axis, out minA, out maxA);
                float minB, maxB;
                ProjectPolygon(B, axis, out minB, out maxB);

                float overlap = IntersectInterval(minA, maxA, minB, maxB);
                if (overlap < 0)    // No overlap - no interesection of polygons
                    return false;

                if (overlap < minOverlap)
                {
                    // Get smalles overlap for projection vector
                    minOverlap = overlap;
                    projDirection = axis;
                    collidingEdge = edge;
                }
            }

            // Loop finished without a seperation.
            //

            // Calculate projection vector.
            if (Vector2.Dot(diff, projDirection) < 0) 
                projDirection = -projDirection;

            Vector2 projectionVector = projDirection * minOverlap;

            // Objects collides.
            if (!A.Owner.IsSensor && !B.Owner.IsSensor)
            {
                A.Owner.Collided(B.Owner, collidingEdge, projectionVector, deltaTime);
            }
            if (A.Owner.OnCollision != null)
                A.Owner.OnCollision(A.Owner, B.Owner, projectionVector);
            if (B.Owner.OnCollision != null)
                B.Owner.OnCollision(B.Owner, A.Owner, Vector2.Zero);
            return true;
        }

        /**
         * Projects the polygon onto the given axis and out puts
         * the interval
         */
        private void ProjectPolygon(Polygon polygon, Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;
            Edge edge;
            float projection;

            for (int i = 0; i < polygon.Edges.Length; i++)
            {
                edge = polygon.Edges[i];

                // Project point on axis
                projection = Vector2.Dot(edge.P1, axis);

                if (projection < min)
                {
                    min = projection;
                }
                if (projection > max)
                {
                    max = projection;
                }
            }
        }

        /**
         * Calculates the intersection value of two intervals
         * Returns a positive value when interesecting
         */
        private float IntersectInterval(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return maxA - minB;
            }
            else
            {
                return maxB - minA;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            var curColumn = _offset.X;
            var curRow = _offset.Y;

            //Camera playerCamera = GameInfo.CurrentLevel.PlayerOwner.PlayerCamera;

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.GetView(Vector2.One));

            /*
            #region Draw physics grid
            var curClmPixel = (int)ConvertUnits.ToDisplayUnits(curColumn);
            var curRowPixel = (int)ConvertUnits.ToDisplayUnits(curRow);
            var curCSzPixel = (int)ConvertUnits.ToDisplayUnits(_cellSize);
            for (int i = 0; i < _gridHeight; i++)
            {
                for (int j = 0; j < _gridWidth; j++)
                {
                    if (_grid[j,i] != null && _grid[j,i].Count != 0)
                    {
                        float alpha = MathHelper.Clamp(0.2f * _grid[j, i].Count, 0.1f, 0.8f);
                        Primitives.Instance.DrawBox(sb, new Rectangle(curClmPixel, curRowPixel, curCSzPixel, curCSzPixel), new Color(1f, 0f, 0f, alpha));
                    }

                    Primitives.Instance.DrawLine(sb, new Vector2(curClmPixel, curRowPixel), new Vector2(curClmPixel + curCSzPixel, curRowPixel), Color.White, 1);
                    Primitives.Instance.DrawLine(sb, new Vector2(curClmPixel, curRowPixel), new Vector2(curClmPixel, curRowPixel + curCSzPixel), Color.White, 1);
                    curClmPixel += (int)curCSzPixel;
                }
                curClmPixel = (int)ConvertUnits.ToDisplayUnits(_offset.X);
                curRowPixel += (int)curCSzPixel;
            }
            #endregion
            */

            // Draw physic objects debugs.
            _staticObjects.ForEach((po) => { po.Draw(sb); });
            _dynamicObjects.ForEach((po) => { po.Draw(sb); });
            _sensors.ForEach((po) => { po.Draw(sb); });
            _drawLines.ForEach((line) => { line.Draw(sb); });
            _drawPoints.ForEach((point) => { Primitives.Instance.DrawPoint(sb, ConvertUnits.ToDisplayUnits(point), Color.Green, 4); });

            // Cleanup
            _drawLines.Clear();
            _drawPoints.Clear();

            sb.End();
        }

        public void AddToWorld(PhysicsObject newObject)
        {
            if (newObject.IsSensor == true)
            {
                if (newObject.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                {
                    var polygon = newObject.CollisionShape as Polygon;

                    for (int i = 0; i < polygon.Edges.Length; i++)
                        AddEdgeToGrid(polygon.Edges[i], newObject);
                }
                newObject.WorldIndex = _sensors.Add(newObject as Sensor);
            }
            else if (newObject.IsStatic == false)
            {
                if (newObject.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                {
                    var polygon = newObject.CollisionShape as Polygon;

                    for (int i = 0; i < polygon.Edges.Length; i++)
                        AddEdgeToGrid(polygon.Edges[i], newObject);
                }
                newObject.WorldIndex = _dynamicObjects.Add(newObject as DynamicObject);
            }
            else
            {
                if (newObject.CollisionShape != null && newObject.CollisionShape.Type == Shape.ShapeType.SH_POLYGON)
                {
                    // Assign cells to vertices.
                    var polygon = newObject.CollisionShape as Polygon;

                    for (int i = 0; i < polygon.Edges.Length; i++)
                        AddEdgeToGrid(polygon.Edges[i], newObject);
                }
                newObject.WorldIndex = _staticObjects.Add(newObject as StaticObject);
            }
        }

        public void RemoveFromWorld(PhysicsObject physicsObject)
        {
            // Remove from cells.
            List<CellValues> cells = physicsObject.ContainingCells;

            cells.ForEach((cell) => { _grid[cell.Column, cell.Row].Remove(physicsObject); });

            // Remove from bags.
            if (physicsObject.IsSensor)
            {
                _sensors.Remove(physicsObject.WorldIndex);
                if (_sensors.Count != physicsObject.WorldIndex)
                    _sensors[physicsObject.WorldIndex].WorldIndex = physicsObject.WorldIndex;
            }
            else if (physicsObject.IsStatic)
            {
                _staticObjects.Remove(physicsObject.WorldIndex);
                if (_staticObjects.Count != physicsObject.WorldIndex)
                    _staticObjects[physicsObject.WorldIndex].WorldIndex = physicsObject.WorldIndex;
            }
            else
            {
                _dynamicObjects.Remove(physicsObject.WorldIndex);
                if (_dynamicObjects.Count != physicsObject.WorldIndex)
                    _dynamicObjects[physicsObject.WorldIndex].WorldIndex = physicsObject.WorldIndex;
            }
        }

        /**
         * Adds an edge to the grid using Bresenham's line algorithm
         * Adds the added cells to the physic object's cell list
         */
        private void AddEdgeToGrid(Edge edge, PhysicsObject physicsObject)
        {
            // Get starting end and positions.
            Vector2 pos1 = edge.P1 - Origin;
            Vector2 pos2 = edge.P2 - Origin;
            int x1 = (int)(pos1.X / _cellSize); int x2 = (int)(pos2.X / _cellSize);
            int y1 = (int)(pos1.Y / _cellSize); int y2 = (int)(pos2.Y / _cellSize);

            int deltaX = Math.Abs(x1 - x2);
            int deltaY = Math.Abs(y1 - y2);

            // Set signum values.
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;

            int error = deltaX - deltaY;

            while (true)
            {
                InsertIntoGrid(x1, y1, physicsObject);
                
                // If we reached last cell break out of loop.
                if (x1 == x2 && y1 == y2)
                    break;
                // Double error to avoid using floats
                int doubleError = error * 2;

                if (doubleError > -deltaY)
                {
                    // Add cell to edge to avoid loose connections
                    int y1b = y1 + signY;                    
                    if (checkLineInCell(x1, y1b, new Line(edge.P1, edge.P2)))
                        InsertIntoGrid(x1, y1b, physicsObject);

                    // Increment in x direction
                    error -= deltaY;
                    x1 += signX;
                }
                if (doubleError < deltaX)
                {
                    // Add cell to edge to avoid loose connections
                    int x1b = x1 + signX;
                    if (checkLineInCell(x1b, y1, new Line(edge.P1, edge.P2)))
                        InsertIntoGrid(x1b, y1, physicsObject);

                    // Increment in y direction
                    error += deltaX;
                    y1 += signY;
                }
            }
        }

        /**
         * Inserts object into cell and adds it to the physics object's cell list if it isn't already in there.
         */
        private void InsertIntoGrid(int col, int row, PhysicsObject physicObject)
        {
            if (_grid[col, row] == null)
                _grid[col, row] = new Bag<PhysicsObject>();

            if (!_grid[col, row].Contains(physicObject))
            {
                _grid[col, row].Add(physicObject);

                var cell = new CellValues();
                cell.Column = col;
                cell.Row = row;
                cell.Index = _grid[col, row].Count - 1;

                physicObject.ContainingCells.Add(cell);
            }
        }

        private bool checkLineInCell(int col, int row, Line line)
        {
            Vector2 collPoint, collNorm;
            Vector2[] vertices = new Vector2[4];
            vertices[0] = new Vector2(col * _cellSize, row * _cellSize) + Origin;
            vertices[1] = vertices[0] + new Vector2(_cellSize, 0);
            vertices[2] = vertices[1] + new Vector2(0, _cellSize);
            vertices[3] = vertices[0] + new Vector2(0, _cellSize);
            Polygon poly = new Polygon(vertices, null); /// THAT POLYGON ALWAYS LOOKS THE SAME !!!! FIX ME!!!

            return CollideLinePoly(line, poly, out collPoint, out collNorm);
        }
    }
}
