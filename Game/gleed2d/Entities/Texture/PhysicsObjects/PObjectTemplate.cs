using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GLEED2D
{
    public class PObjectTemplate : TextureItem
    {
        public float Mass;
        public float Friction;
        public float Restitution;
        public string CollisionShape;
        public string PhysicsType;

        [DisplayName("Mass"), Category(" Physical properties")]
        [XmlIgnore()]
        public float mass
        {
            get
            {
                return Mass;
            }
            set
            {
                Mass = value;
            }
        }

        [DisplayName("Physics Type"), Category(" Physical properties")]
        [XmlIgnore()]
        public string physicsType
        {
            get { return PhysicsType; }
            set { PhysicsType = value; }
        }

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

        [DisplayName("Collision Shape"), Category(" Physical properties")]
        [XmlIgnore()]
        public string collisionShape
        {
            get { return CollisionShape; }
            set { CollisionShape = value; }
        }


        public PObjectTemplate()
            : base()
        {

        }

        /*public override string getNamePrefix()
        {
            if (bodyType)
                return "SO_";
            else
                return "DO_";
        }*/

        public PObjectTemplate(string fullpath, Vector2 position)
            : base(fullpath, position)
        {
            //default settings
            Mass = 10;
            Friction = 2f;
            Restitution = 0.2f;
            CollisionShape = "Rectangle";
            PhysicsType = "Static";
        }


    }
}
