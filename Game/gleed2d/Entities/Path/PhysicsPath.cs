using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GLEED2D
{
    public class PhysicsPath : PathItem
    {
        public float Mass;
        public float Friction;
        public float Restitution;
        public string PhysicsType;

        //Physics
        [DisplayName("Friction"), Category(" Physical properties")]
        [XmlIgnore()]
        public float friction
        {
            get { return Friction; }
            set { Friction = value; }
        }

        [DisplayName("Restitution"), Category(" Physical properties")]
        [XmlIgnore()]
        public float restitution
        {
            get { return Restitution; }
            set { Restitution = value; }
        }

        [DisplayName("Mass"), Category(" Physical properties")]
        [XmlIgnore()]
        public float mass
        {
            get { return Mass; }
            set { Mass = value; }
        }

        [DisplayName("Physics Type"), Category(" Physical properties")]
        [XmlIgnore()]
        public string physicstype
        {
            get { return PhysicsType; }
            set { PhysicsType = value; }
        }

        public PhysicsPath(Vector2[] points)
            : base()
        {
            Position = points[0];
            WorldPoints = points;
            LocalPoints = (Vector2[])points.Clone();
            for (int i = 0; i < LocalPoints.Length; i++) LocalPoints[i] -= Position;
            LineWidth = Constants.Instance.DefaultPathItemLineWidth;
            LineColor = Constants.Instance.ColorPrimitives;
            Mass = 10;
            Friction = 2f;
            Restitution = 0.2f;
            PhysicsType = "Static";
            IsPolygon = true;

            // Add to physics path list.
            Editor.Instance.PhysicObjects.Add(this);
        }

        public override string getNamePrefix()
        {
            return "PhysicsPath_";
        }

        public PhysicsPath()
            : base()
        {
            // Add to physics path list.
            Editor.Instance.PhysicObjects.Add(this);
        }

        public void SetExtent()
        {
            float minX = float.PositiveInfinity; float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity; float maxY = float.NegativeInfinity;
            foreach (Vector2 point in WorldPoints)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;
                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            Vector2 topLeft = Editor.Instance.PhysicsExtentTopLeft;
            Vector2 bottomRight = Editor.Instance.PhysicsExtentBottomRight;

            if (minX < topLeft.X)
            {
                topLeft.X = minX;
            }
            if (minY < topLeft.Y)
            {
                topLeft.Y = minY;
            }
            if (maxX > bottomRight.X)
            {
                bottomRight.X = maxX;
            }
            if (maxY > bottomRight.Y)
            {
                bottomRight.Y = maxY;
            }

            Editor.Instance.PhysicsExtentTopLeft = topLeft;
            Editor.Instance.PhysicsExtentBottomRight = bottomRight;
        }
    }
}
