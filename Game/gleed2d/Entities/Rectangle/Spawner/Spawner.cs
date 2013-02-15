/*
 *  Sets spawnable objects
 */
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
    public class Spawner : RectangleItem
    {
        public Spawner(Rectangle rect)
            : base(rect)
        {

        }
        public Spawner()
            : base()
        {

        }
    }


    public class NPCStart : Spawner
    {
        public string NPCController;
        public int MoveSpeed;

        [DisplayName("NPC Type"), Category(" Physical properties")]
        [XmlIgnore()]
        public String npcController
        {
            get
            {
                return NPCController;
            }
            set
            {
                NPCController = value;
            }
        }



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

        public NPCStart(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Green;
            this.FillColor.A = 155;
            this.NPCController = "Type";
            this.MoveSpeed = 1;
        }
        public override string getNamePrefix()
        {
            return "NPCStart_";
        }

        public NPCStart()
            : base()
        {

        }
    }

    //Door Object
    

    //BlockerItem

    

    //Fuse Box Item

    

    //Level End

    public class LevelTransition : Spawner
    {
        public string nextLevel;

        [DisplayName("Next Level"), Category(" Level Transition")]
        [XmlIgnore()]
        public string NextLevel
        {
            get
            {
                return nextLevel;
            }
            set
            {
                nextLevel = value;
            }
        }

        public LevelTransition(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Gray;
            this.FillColor.A = 155;
            this.nextLevel = "Next/Level";
        }
        public override string getNamePrefix()
        {
            return "LevelTransition_";
        }

        public LevelTransition()
            : base()
        {

        }
    }

    // Turret Item
    public class TurretItem : Spawner
    {
        public string texturePath;
        public float mass;
        public float damage;

        [DisplayName("Texture Path"), Category(" Turret properties")]
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

        [DisplayName("Mass"), Category(" Turret properties")]
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

        [DisplayName("Damage"), Category(" Turret properties")]
        [XmlIgnore()]
        public float Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }



        public TurretItem(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Chocolate;
            this.FillColor.A = 155;
            this.texturePath = "blah/turret";
            this.mass = 4;
            this.damage = 10;
        }
        public override string getNamePrefix()
        {
            return "TurretItem_";
        }

        public TurretItem()
            : base()
        {

        }
    }


    // Holy Ground
    public class HolyGround : Spawner
    {

        public HolyGround(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Chocolate;
            this.FillColor.A = 155;
        }
        public override string getNamePrefix()
        {
            return "HolyGround_";
        }

        public HolyGround()
            : base()
        {

        }
    }

    // Water
    public class Water : Spawner
    {

        public Water(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Blue;
            this.FillColor.A = 155;
        }
        public override string getNamePrefix()
        {
            return "Water_";
        }

        public Water()
            : base()
        {

        }
    }

    // Smoke
    public class HolySmoke : Spawner
    {

        public HolySmoke(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Gray;
            this.FillColor.A = 155;
        }
        public override string getNamePrefix()
        {
            return "HolySmoke_";
        }

        public HolySmoke()
            : base()
        {

        }
    }

    // Trap
    public class Trap : Spawner
    {

        public Trap(Rectangle rect)
            : base(rect)
        {
            this.FillColor = Color.Gray;
            this.FillColor.A = 155;
        }
        public override string getNamePrefix()
        {
            return "Trap_";
        }

        public Trap()
            : base()
        {

        }
    }



}
