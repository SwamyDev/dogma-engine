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
    public class nwSpawner : TextureItem
    {

        public nwSpawner(string fullpath, Vector2 position)
            : base(fullpath, position)
        {
        }
        public nwSpawner()
            : base()
        {
        }
    }

        public class PlayerStart : nwSpawner
        {
            public PlayerStart(string fullpath, Vector2 position)
            : base(fullpath, position)
            {
                fullpath = "Assets/player.png";
            }
            public override string getNamePrefix()
            {
                return "PlayerStart_";
            }

            public PlayerStart()
                : base()
            {

            }
        }

        public class GuardStart : nwSpawner
        {
            public int MoveSpeed;
            [DisplayName("Move Speed"), Category(" Physical properties")]
            [XmlIgnore()]
            public int movespeed
            {
                get
                {
                    return MoveSpeed;
                }
                set
                {
                    MoveSpeed = value;
                }
            }

            public GuardStart(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                fullpath = "Assets/guard.png";
                this.movespeed = 1;
            }
            public override string getNamePrefix()
            {
                return "GuardStart_";
            }

            public GuardStart()
                : base()
            {
            }
        }

        public class PriestStart : nwSpawner
        {
            public int MoveSpeed;
            [DisplayName("Move Speed"), Category(" Physical properties")]
            [XmlIgnore()]
            public int movespeed
            {
                get
                {
                    return MoveSpeed;
                }
                set
                {
                    MoveSpeed = value;
                }
            }

            public PriestStart(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                fullpath = "Assets/priest.png";
                this.movespeed = 1;
            }
            public override string getNamePrefix()
            {
                return "PriestStart_";
            }

            public PriestStart()
                : base()
            {
            }
        }

        public class DoorItem : nwSpawner
        {
            public string openTexture;
            public string closedTexture;
            public Boolean isOpen;
            public int openDirection;
            public float mass;
            public float friction;
            public float restitution;
            public string CollisionShape;
            public string PhysicsType;

            [DisplayName("Open Texture"), Category(" Door properties")]
            [XmlIgnore()]
            public String OpenTexture
            {
                get
                {
                    return openTexture;
                }
                set
                {
                    openTexture = value;
                }
            }

            [DisplayName("Closed Texture"), Category(" Door properties")]
            [XmlIgnore()]
            public String ClosedTexture
            {
                get
                {
                    return closedTexture;
                }
                set
                {
                    closedTexture = value;
                }
            }

            [DisplayName("Door Opened"), Category(" Door properties")]
            [XmlIgnore()]
            public Boolean IsOpen
            {
                get
                {
                    return isOpen;
                }
                set
                {
                    isOpen = value;
                }
            }

            [DisplayName("Open Direction"), Category(" Door properties")]
            [XmlIgnore()]
            public int OpenDirection
            {
                get
                {
                    return openDirection;
                }
                set
                {
                    openDirection = value;
                }
            }

            [DisplayName("Mass"), Category(" Door properties")]
            [XmlIgnore()]
            public float Mass
            {
                get
                {
                    return mass;
                }
                set
                {
                    mass = value;
                }
            }
            [DisplayName("Friction"), Category(" Door properties")]
            [XmlIgnore()]
            public float Friction
            {
                get
                {
                    return friction;
                }
                set
                {
                    friction = value;
                }
            }
            [DisplayName("Restitution"), Category(" Door properties")]
            [XmlIgnore()]
            public float Restitution
            {
                get
                {
                    return restitution;
                }
                set
                {
                    restitution = value;
                }
            }
            [DisplayName("CollisionShape"), Category(" Door properties")]
            [XmlIgnore()]
            public string collisionShape
            {
                get
                {
                    return CollisionShape;
                }
                set
                {
                    CollisionShape = value;
                }
            }
            [DisplayName("PhysicsType"), Category(" Door properties")]
            [XmlIgnore()]
            public string physicsType
            {
                get
                {
                    return PhysicsType;
                }
                set
                {
                    PhysicsType = value;
                }
            }


            public DoorItem(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                this.openTexture = "blah/open";
                this.closedTexture = "blah/closed";
                this.isOpen = true;
                this.openDirection = -1;
                this.mass = 4;
                this.restitution = 0;
                this.PhysicsType = "Dynamic";
                this.friction = 0;
                this.CollisionShape = "Rectangle";
            }
            public override string getNamePrefix()
            {
                return "DoorItem_";
            }

            public DoorItem()
                : base()
            {

            }
        }

        public class FuseBoxItem : nwSpawner
        {
            public string texturePath;
            public string lightItems;

            [DisplayName("Texture Path"), Category(" FuseBox Item")]
            [XmlIgnore()]
            public string TexturePath
            {
                get
                {
                    return texturePath;
                }
                set
                {
                    texturePath = value;
                }
            }

            [DisplayName("Light Items"), Category(" FuseBox Item")]
            [XmlIgnore()]
            public string LightItems
            {
                get
                {
                    return lightItems;
                }
                set
                {
                    lightItems = value;
                }
            }

            public FuseBoxItem(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                this.texturePath = "blah/blocker";
                this.lightItems = "light1,light2,light3";
            }
            public override string getNamePrefix()
            {
                return "FuseBoxItem_";
            }

            public FuseBoxItem()
                : base()
            {

            }
        }

        public class LightEmitterItem : nwSpawner
        {
            public string LightTexture;

            [DisplayName("Light Item"), Category("Light Emitter Item")]
            [XmlIgnore()]
            public string LightItem
            {
                get
                {
                    return LightTexture;
                }
                set
                {
                    LightTexture = value;
                }
            }

            public LightEmitterItem(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                this.LightTexture = "blah/blocker";
            }
            public override string getNamePrefix()
            {
                return "LightEmitterItem_";
            }

            public LightEmitterItem()
                : base()
            {

            }
        }


        public class BlockerItem : nwSpawner
        {
            public string texturePath;
            public float mass;
            public float friction;
            public float restitution;
            public string CollisionShape;
            public string PhysicsType;

            [DisplayName("Texture Path"), Category(" Blocker properties")]
            [XmlIgnore()]
            public String TexturePath
            {
                get
                {
                    return texturePath;
                }
                set
                {
                    texturePath = value;
                }
            }

            //Mass, Friction, Restitution, CollisionShape, PhysicsType

            [DisplayName("Mass"), Category(" Blocker properties")]
            [XmlIgnore()]
            public float Mass
            {
                get
                {
                    return mass;
                }
                set
                {
                    mass = value;
                }
            }
            [DisplayName("Friction"), Category(" Blocker properties")]
            [XmlIgnore()]
            public float Friction
            {
                get
                {
                    return friction;
                }
                set
                {
                    friction = value;
                }
            }
            [DisplayName("Restitution"), Category(" Blocker properties")]
            [XmlIgnore()]
            public float Restitution
            {
                get
                {
                    return restitution;
                }
                set
                {
                    restitution = value;
                }
            }
            [DisplayName("CollisionShape"), Category(" Blocker properties")]
            [XmlIgnore()]
            public string collisionShape
            {
                get
                {
                    return CollisionShape;
                }
                set
                {
                    CollisionShape = value;
                }
            }
            [DisplayName("PhysicsType"), Category(" Blocker properties")]
            [XmlIgnore()]
            public string physicsType
            {
                get
                {
                    return PhysicsType;
                }
                set
                {
                    PhysicsType = value;
                }
            }



            public BlockerItem(string fullpath, Vector2 position)
                : base(fullpath, position)
            {
                this.texturePath = "blah/blocker";
                this.mass = 4;
                this.restitution = 0;
                this.PhysicsType = "Dynamic";
                this.friction = 0;
                this.CollisionShape = "Rectangle";
            }
            public override string getNamePrefix()
            {
                return "BlockerItem_";
            }

            public BlockerItem()
                : base()
            {

            }
        }

}
