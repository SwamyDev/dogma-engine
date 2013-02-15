using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PontAnimator
{
    public class Settings
    {
        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();

                return _instance;
            }
        }

        [Category("Settings"), Description("The content path used to load the animation stripes from.")]
        public string ContentPath { get; set; }

        public Settings()
        {
        }

        public void SetContentPath(string path)
        {
            ContentPath = path;
            FolderSettings.Instance.Import(path + "\\" + MainForm.SettingsFileName);
        }

        //XML import and export
        public void Export(string filename)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\" + filename;

            FileStream stream = File.Open(path, FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            
            serializer.Serialize(stream, this);
            stream.Close();
        }
        public void Import(string filename)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\" + filename;
            
            if (!File.Exists(path)) 
                return;

            FileStream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            _instance = (Settings)serializer.Deserialize(stream);
            stream.Close();
        }
    }

    public class FolderSettings
    {
        private static FolderSettings _instance;
        public static FolderSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FolderSettings();

                return _instance;
            }
        }

        // Set up xml properties
        [Category("Settings"), Description("Name of the animation used to produce names for final stripes according to naming convention")]
        public SerializableDictionary<string, string> StripeAnimationName { get; set; }

        [Category("Settings"), Description("Frame count of specified stripes.")]
        public SerializableDictionary<string, int> StripeFrames { get; set; }

        [Category("Settings"), Description("Frames per second of specified stripes.")]
        public SerializableDictionary<string, int> StripeFramesPerSecond { get; set; }

        [Category("Settings"), Description("Column of this sprite in the actual sheet.")]
        public SerializableDictionary<string, int> StripeColumn { get; set; }

        public FolderSettings()
        {
            // Set default values.
            StripeAnimationName = new SerializableDictionary<string, string>();
            StripeFrames = new SerializableDictionary<string, int>();
            StripeFramesPerSecond = new SerializableDictionary<string, int>();
            StripeColumn = new SerializableDictionary<string, int>();
        }

        // Set's the animation name and adds a new field to the dictionary if necessary.
        public void SetAnimationName(string name, string stripeName)
        {
            if (StripeAnimationName.ContainsKey(stripeName))
            {
                StripeAnimationName[stripeName] = name;
            }
            else
            {
                StripeAnimationName.Add(stripeName, name);
            }
        }

        // Set's the frames and adds a new field to the dictionary if necessary.
        public void SetFrames(int frames, string stripeName)
        {
            if (StripeFrames.ContainsKey(stripeName))
            {
                StripeFrames[stripeName] = frames;
            }
            else
            {
                StripeFrames.Add(stripeName, frames);
            }
        }

        // Set's the frames per second and adds a new field to the dictionary if necessary.
        public void SetFramesPerSecond(int fps, string stripeName)
        {
            if (StripeFramesPerSecond.ContainsKey(stripeName))
            {
                StripeFramesPerSecond[stripeName] = fps;
            }
            else
            {
                StripeFramesPerSecond.Add(stripeName, fps);
            }
        }

        public void SetColumn(int column, string stripeName)
        {
            if (StripeColumn.ContainsKey(stripeName))
            {
                StripeColumn[stripeName] = column;
            }
            else
            {
                StripeColumn.Add(stripeName, column);
            }
        }

        public string GetAnimationName(string stripeName)
        {
            if (StripeAnimationName.ContainsKey(stripeName))
            {
                return StripeAnimationName[stripeName];
            }

            return string.Empty;
        }

        public int GetFrames(string stripeName)
        {
            if (StripeFrames.ContainsKey(stripeName))
            {
                return StripeFrames[stripeName];
            }

            return 0;
        }

        public int GetFramesPerSecond(string stripeName)
        {
            if (StripeFramesPerSecond.ContainsKey(stripeName))
            {
                return StripeFramesPerSecond[stripeName];
            }

            return 0;
        }

        public int GetColumn(string stripeName)
        {
            if (StripeColumn.ContainsKey(stripeName))
            {
                return StripeColumn[stripeName];
            }

            return -1;
        }

        public bool StripeConfigured(string stripeName)
        {
            return StripeAnimationName.ContainsKey(stripeName) && StripeFrames.ContainsKey(stripeName) && StripeFramesPerSecond.ContainsKey(stripeName);
        }

        public bool ColumnsDistributed()
        {
            // First we need all stripes assigned with columns.
            if (MainForm.Instance.cobAnimations.Items.Count != StripeColumn.Count)
                return false;

            // Make sure there are no double column values.
            foreach (var A in StripeColumn)
            {
                int appearance = 0;
                foreach (var B in StripeColumn)
                {
                    if (A.Value == B.Value)
                        appearance++;

                    // Check if value appears more than once
                    if (appearance > 1)
                        return false;
                }
            }

            return true;
        }

        //XML import and export
        public void Export(string path)
        {
            FileStream stream = File.Open(path, FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(FolderSettings));

            serializer.Serialize(stream, this);
            stream.Close();
        }
        public void Import(string path)
        {
            if (!File.Exists(path))
                return;

            FileStream stream = File.Open(path, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(FolderSettings));

            _instance = (FolderSettings)serializer.Deserialize(stream);
            stream.Close();
        }
    }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();

                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
