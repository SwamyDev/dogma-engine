using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Pontification.SceneManagement
{
    public class ComponentItemCollection : ICollection<PropertyBag>, IXmlSerializable
    {
        #region Private attributes
        private PropertyBag[] _data;
        private int _size;
        #endregion

        public ComponentItemCollection()
            : this(16)
        {
        }

        public ComponentItemCollection(int capacity)
        {
            _data = new PropertyBag[capacity];
        }

        public PropertyBag this[int index]
        {
            get { return _data[index]; }
            set
            {
                if (index >= _data.Length)
                {
                    Grow(index * 2);
                    _size = index + 1;
                }
                else if (index >= _size)
                {
                    _size = index + 1;
                }
                _data[index] = value;
            }
        }

        #region ICollection methods.
        public void Add(PropertyBag item)
        {
            if (_size == _data.Length)
            {
                Grow();
            }

            _data[_size++] = item;
        }

        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i] = null;

            _size = 0;
        }

        public bool Contains(PropertyBag item)
        {
            for (int i = 0; i < _data.Length; i++)
                if (_data[i] == item)
                    return true;

            return false;
        }

        public void CopyTo(PropertyBag[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _size; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PropertyBag item)
        {
            for (var i = 0; i < _size; i++)
            {
                if (item != _data[i])
                    continue;

                _data[i] = _data[--_size];
                _data[_size] = null;
                return true;
            }

            return false;
        }

        public IEnumerator<PropertyBag> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _data[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IXmlSerializable methods
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer valueSerializer = new XmlSerializer(typeof(PropertyBag));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            // Read until we reach last element
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                var compType = reader.GetAttribute("TypeName");
                var isComponent = bool.Parse(reader.GetAttribute("IsComponent"));
                var comp = (PropertyBag)valueSerializer.Deserialize(reader);
                comp.TypeName = compType;
                comp.IsComponent = isComponent;
                Add(comp);
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer valueSerializer = new XmlSerializer(typeof(PropertyBag));

            for (int i = 0; i < Count; i++)
            {
                valueSerializer.Serialize(writer, _data[i]);
            }
        }
        #endregion

        #region Private methods
        private void Grow()
        {
            var newCapacity = (_data.Length * 3) / 2 + 1;
            Grow(newCapacity);
        }

        private void Grow(int newCapacity)
        {
            var oldData = _data;
            _data = new PropertyBag[newCapacity];
            Array.Copy(oldData, 0, _data, 0, oldData.Length);
        }
        #endregion
    }
}
