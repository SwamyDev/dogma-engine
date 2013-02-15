using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLEED2D
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertyBag : ICustomTypeDescriptor, IXmlSerializable
    {
        private Dictionary<string, object> _storage = new Dictionary<string, object>();
        private List<PolygonPath> _polygons = new List<PolygonPath>();
        private List<TextureItem> _textures = new List<TextureItem>();
        private List<SoundEffectsItem> _sounds = new List<SoundEffectsItem>();

        public string TypeName { get; set; }
        public bool IsComponent { get; set; }

        public PropertyBag()
        {
        }

        public PropertyBag(string typeName, bool isComponent)
        {
            TypeName = typeName;
            IsComponent = isComponent;
        }

        #region Public methods
        public void SetProperty(string name, object value)
        {
            if (_storage.ContainsKey(name))
                _storage[name] = value;
            else
                _storage.Add(name, value);

            // Add to draw lists.
            AddToDrawable(value);
        }

        public List<Item> GetSelectables()
        {
            List<Item> drawables = new List<Item>(_textures);
            drawables.AddRange(_polygons);
            drawables.AddRange(_sounds);

            return drawables;
        }

        public List<PolygonPath> GetPolygons()
        {
            return _polygons;
        }

        public List<TextureItem> GetTextures()
        {
            return _textures;
        }

        public List<SoundEffectsItem> GetSounds()
        {
            return _sounds;
        }

        public void RemoveProperty(string propertyName)
        {
            if (_storage.ContainsKey(propertyName))
            {
                var value = _storage[propertyName];
                RemoveFromDrawable(value);
                _storage.Remove(propertyName);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw stuff if we have drawable stuff.
            foreach (var polygon in _polygons)
                polygon.drawInEditor(sb);
        }
        #endregion

        #region ICustomTypeDescriptor methods
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            foreach (string key in _storage.Keys)
            {
                // For each employee create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance
                ComponentItemPropertyDescriptor pd = new ComponentItemPropertyDescriptor(_storage, key);
                pds.Add(pd);
            }
            return pds;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

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
                else type = Type.GetType(string.Format("GLEED2D.{0}", typeName));

                if (type == null)
                    type = Type.GetType(string.Format("Pontification.Components.{0}, Pontification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeName));
                if (type == null)
                    type = Type.GetType(string.Format("Pontification.{0}, Pontification, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeName));

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
                else if (pair.Value.GetType().Name == typeof(ListItem<>).Name)
                    writer.WriteAttributeString("Type", pair.Value.GetType().ToString().Replace(string.Format("{0}.", pair.Value.GetType().Namespace), ""));
                else
                    writer.WriteAttributeString("Type", pair.Value.GetType().Name);

                serializer.Serialize(writer, pair.Value);
                writer.WriteEndElement();
            }
        }
        #endregion

        private void AddToDrawable(object o)
        {
            var physics = o as PolygonPath;
            var texture = o as TextureItem;
            var sound = o as SoundEffectsItem;
            if (physics != null)
            {
                _polygons.Add(physics);
            }
            else if (texture != null)
            {
                _textures.Add(texture);
            }
            else if (sound != null)
            {
                _sounds.Add(sound);
            }
        }

        private void RemoveFromDrawable(object o)
        {
            var physics = o as PolygonPath;
            var texture = o as TextureItem;

            if (physics != null)
            {
                _polygons.Remove(physics);
            }
            else if (texture != null)
            {
                _textures.Remove(texture);
            }
        }
    }

    #region Property descriptor
    public class ComponentItemPropertyDescriptor : PropertyDescriptor
    {
        private Dictionary<string, object> _collection;
        private string _key;

        public ComponentItemPropertyDescriptor(Dictionary<string, object> collection, string key)
            : base("#" + key, null)
        {
            _collection = collection;
            _key = key;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return _collection.GetType();
            }
        }

        public override string DisplayName
        {
            get
            {
                return _key;
            }
        }

        public override string Description
        {
            get
            {
                return "This is the ComponentItem description";
            }
        }

        public override object GetValue(object component)
        {
            return _collection[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "#" + _key; }
        }

        public override Type PropertyType
        {
            get { return _collection[_key].GetType(); }
        }

        public override void ResetValue(object component) { }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            _collection[_key] = value;
        }
    }
    #endregion
}
