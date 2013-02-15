using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pontification.Components;

namespace Pontification.SceneManagement
{
    public class PropertyBag : DataObject, IXmlSerializable
    {
        private Dictionary<string, object> _storage = new Dictionary<string, object>();

        public string TypeName { get; set; }
        public bool IsComponent { get; set; }

        public PropertyBag()
        {
        }

        #region Public methods
        public IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs()
        {
            return _storage;
        }
        public void SetProperty(string name, object value)
        {
            if (_storage.ContainsKey(name))
                _storage[name] = value;
            else
                _storage.Add(name, value);
        }

        public void RemoveProperty(string propertyName)
        {
            if (_storage.ContainsKey(propertyName))
            {
                var value = _storage[propertyName];
                _storage.Remove(propertyName);
            }
        }
        #endregion

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);
            var keyType = prop.PropertyType.GetGenericArguments()[0];
            var valueType = prop.PropertyType.GetGenericArguments()[1];
            var openType = typeof(Dictionary<,>);
            var dictType = openType.MakeGenericType(keyType, valueType);
            var dict = Activator.CreateInstance(dictType);
            var addMethod = dictType.GetMethod("Add");


            foreach (var pair in _storage)
            {
                if (keyType == typeof(string))
                {
                    addMethod.Invoke(dict, new object[] { pair.Key, pair.Value });
                }
                else if (keyType.IsEnum)
                {
                    var key = Enum.Parse(keyType, pair.Key);
                    addMethod.Invoke(dict, new object[] { key, pair.Value });
                }
            }

            prop.SetValue(comp, dict, null);
        }

        #region IXmlSerializable methods
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            // Read until we reach last element
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                var binderName = reader.GetAttribute("Binder");
                var typeName = reader.GetAttribute("Type");

                Type type = null;
                if (typeName == "bool") type = typeof(bool);
                else if (typeName == "int") type = typeof(int);
                else if (typeName == "float") type = typeof(float);
                else if (typeName == "string") type = typeof(string);
                else if (typeName == "Vector2") type = typeof(Vector2);
                else if (typeName == "Color") type = typeof(Color);
                else type = Type.GetType(string.Format("Pontification.SceneManagement.{0}", typeName));

                if (type == null)
                    type = Type.GetType(string.Format("Pontification.Components.{0}", typeName));
                if (type == null)
                    type = Type.GetType(string.Format("Pontification.{0}", typeName));

                if (type != null)
                {
                    reader.ReadStartElement("Property");
                    var serializer = new XmlSerializer(type);
                    var value = serializer.Deserialize(reader);
                    SetProperty(binderName, value);

                    reader.ReadEndElement();
                }

                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("TypeName", TypeName);
            writer.WriteAttributeString("IsComponent", IsComponent.ToString());
            foreach (KeyValuePair<string, object> pair in _storage)
            {
                var serializer = new XmlSerializer(pair.Value.GetType());

                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Binder", pair.Key);

                // Write type information
                if (pair.Value.GetType() == typeof(bool))
                    writer.WriteAttributeString("Type", "bool");
                else if (pair.Value.GetType() == typeof(int))
                    writer.WriteAttributeString("Type", "int");
                else if (pair.Value.GetType() == typeof(float))
                    writer.WriteAttributeString("Type", "float");
                else if (pair.Value.GetType() == typeof(string))
                    writer.WriteAttributeString("Type", "string");
                else if (pair.Value.GetType() == typeof(Vector2))
                    writer.WriteAttributeString("Type", "Vector2");
                else if (pair.Value.GetType() == typeof(Color))
                    writer.WriteAttributeString("Type", "Color");
                else
                    writer.WriteAttributeString("Type", pair.Value.GetType().ToString());

                serializer.Serialize(writer, pair.Value);
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
