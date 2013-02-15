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
    public class IlluminationArea : PathItem
    {
        public string LightEmitterID;

        [DisplayName("LightEmitterID"), Category("Light Emitter ID")]
        [XmlIgnore()]
        public string lightEmitterID
        {
            get
            {
                return LightEmitterID;
            }
            set
            {
                LightEmitterID = value;
            }
        }

        public IlluminationArea(Vector2[] points)
            : base()
        {
            Position = points[0];
            WorldPoints = points;
            LocalPoints = (Vector2[])points.Clone();
            for (int i = 0; i < LocalPoints.Length; i++) LocalPoints[i] -= Position;
            LineWidth = Constants.Instance.DefaultPathItemLineWidth;
            LineColor = Color.Azure;
            this.LightEmitterID = "None";
            IsPolygon = true;
        }

        public override string getNamePrefix()
        {
            return "IlluminationArea_";
        }

        public IlluminationArea()
            : base()
        {
        }
    }
}
