using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Pontification.SceneManagement
{
    public class ListItem<T> : DataObject, ICollection<T>, IXmlSerializable
    {
        private T[] _data;
        private int _size;

        public ListItem()
            : this(16)
        {
        }
        public ListItem(int capacity)
        {
            _data = new T[capacity];
        }

        public T this[int index]
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

        public override void Assign(Microsoft.Xna.Framework.Content.ContentManager cm, Components.Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);
            var propType = prop.PropertyType;
            T[] arguments = new T[Count];

            for (int i = 0; i < arguments.Length; i++)
                arguments[i] = _data[i];

            var list = Activator.CreateInstance(propType, new object[] { arguments });

            prop.SetValue(comp, list, null);
        }

        #region ICollection methods.
        public void Add(T item)
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
                _data[i] = default(T);

            _size = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _data.Length; i++)
                if (_data[i].Equals(item))
                    return true;

            return false;
        }
        public void CopyTo(T[] array, int arrayIndex)
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

        public bool Remove(T item)
        {
            bool found = false;
            for (var i = 0; i < _size; i++)
            {
                if (item.Equals(_data[i]))
                    found = true;

                if (found)
                {
                    if (i + 1 < _size)
                        _data[i] = _data[i + 1];
                    else
                        _data[i] = default(T);
                }
            }

            if (found) _size--;

            return found;
        }

        public IEnumerator<T> GetEnumerator()
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

        #region Private methods
        private void Grow()
        {
            var newCapacity = (_data.Length * 3) / 2 + 1;
            Grow(newCapacity);
        }

        private void Grow(int newCapacity)
        {
            var oldData = _data;
            _data = new T[newCapacity];
            Array.Copy(oldData, 0, _data, 0, oldData.Length);
        }
        #endregion

        #region IXmlSerializable methods
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer valueSerializer = new XmlSerializer(typeof(T));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            // Read until we reach last element
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                Add((T)valueSerializer.Deserialize(reader));
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer valueSerializer = new XmlSerializer(typeof(T));

            for (int i = 0; i < Count; i++)
            {
                valueSerializer.Serialize(writer, _data[i]);
            }
        }
        #endregion
    }
}
