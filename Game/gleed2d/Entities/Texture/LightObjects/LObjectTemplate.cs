/*
 *  Light Object Texture Item Template
 */

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
    public class LObjectTemplate : TextureItem
    {
        public Boolean lightOn;
        public Boolean Hideable;

        [DisplayName("Light On"), Category(" Light properties")]
        [XmlIgnore()]
        public Boolean lighton
        {
            get
            {
                return lightOn;
            }
            set
            {
                lightOn = value;
            }
        }

        [DisplayName("Hideable"), Category(" Light properties")]
        [XmlIgnore()]
        public Boolean hideable
        {
            get
            {
                return Hideable;
            }
            set
            {
                Hideable = value;
            }
        }

        public LObjectTemplate()
            : base()
        {

        }

         public LObjectTemplate(string fullpath, Vector2 position)
            : base(fullpath, position)
        {
            //default settings
            lightOn = true;
            Hideable = false;
        }

    }
}
