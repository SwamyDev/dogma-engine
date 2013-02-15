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
using System.Windows.Forms;

namespace GLEED2D
{
   public class AIPath : PathItem
    {
       public string NPCID;
       public string[] Nodes;

       [DisplayName("NPCID"), Category(" NPC properties")]
       [XmlIgnore()]
       public string npcid
       {
           get
           {
               return NPCID;
           }
           set
           {
               NPCID = value;
           }
       }

       [DisplayName("Nodes"), Category(" NPC properties")]
       [XmlIgnore()]
       public string[] nodes
       {
           get
           {
               List<string> list = new List<string>();
               for (int i = 0; i < WorldPoints.Length; i++)
               {
                   string test = WorldPoints[i].ToString();
                   string test2 = test.Replace("}", " TYPE:Waypoint}");
                   list.Add(test2);
               }
               Nodes = list.ToArray();
               return Nodes;
           }
           set
           {
               
               Nodes = value;
              
           }
       }

       public AIPath(Vector2[] points)
            : base()
        {
            Position = points[0];
            WorldPoints = points;
            LocalPoints = (Vector2[])points.Clone();
            for (int i = 0; i < LocalPoints.Length; i++) LocalPoints[i] -= Position;
            LineWidth = Constants.Instance.DefaultPathItemLineWidth;
            LineColor = Color.Red;
            this.NPCID = "None";
            IsPolygon = false;
       }

       public override string getNamePrefix()
       {
           return "AIPath_";
       }

       public AIPath()
            : base()
        {
        }
    }
}
