using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Forms = System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using CustomUITypeEditors;
using Microsoft.Xna.Framework;

namespace GLEED2D
{
    public partial class Scene
    {
        [XmlIgnore()]
        public static readonly List<string> GameObjectNames = new List<string>();
        [XmlIgnore()]
        public string selectedlayers;
        [XmlIgnore()]
        public string selecteditems;

        public class EditorVars
        {
            public int NextItemNumber;
            public string ContentRootFolder;
            public Vector2 CameraPosition;
            public string Version;
        }

        [XmlIgnore()]
        [Category(" General")]
        [Description("When the level is saved, each texture is saved with a path relative to this folder."
                     + "You should set this to the \"Content.RootDirectory\" of your game project.")]
        [EditorAttribute(typeof(FolderUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public String ContentRootFolder
        {
            get
            {
                return EditorRelated.ContentRootFolder;
            }
            set
            {
                EditorRelated.ContentRootFolder = value;
            }
        }

        EditorVars editorrelated = new EditorVars();
        [Browsable(false)]
        public EditorVars EditorRelated
        {
            get
            {
                return editorrelated;
            }
            set
            {
                editorrelated = value;
            }
        }

        [XmlIgnore()]
        public Forms.TreeNode treenode;


        public string getNextItemNumber()
        {
            return (++EditorRelated.NextItemNumber).ToString("0000");
        }


        public void export(string filename)
        {
            foreach (Layer l in Layers)
            {
                foreach (Item i in l.Items)
                {
                    if (i is TextureItem)
                    {
                        TextureItem ti = (TextureItem)i;
                        ti.texture_filename = RelativePath(ContentRootFolder, ti.texture_fullpath);
                        ti.asset_name = ti.texture_filename.Substring(0, ti.texture_filename.LastIndexOf('.'));
                    }
                }
            }


            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;

            XmlSerializer serializer = new XmlSerializer(typeof(Scene));
            serializer.Serialize(writer, this);

            writer.Close();

        }



        public static string RelativePath(string relativeTo, string pathToTranslate)
        {
            string[] absoluteDirectories = relativeTo.Split('\\');
            string[] relativeDirectories = pathToTranslate.Split('\\');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                // throw new ArgumentException("Paths do not have a common base");
                return pathToTranslate;

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0) relativePath.Append("..\\");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "\\");

            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }








    }








}
