using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Pontification
{
    public class Bag<T> : IEnumerable<T>, IXmlSerializable where T : class
    {
        #region Private attributes
        private T[] _data;
        private int _size;
        #endregion

        #region Public methods
        public Bag()
        {
            _data = new T[16];
        }

        public Bag(int capacity)
        {
            _data = new T[capacity];
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public int Count
        {
            get { return _size; }
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

        public T Remove(int index)
        {
            var o = _data[index];
            _data[index] = _data[--_size];
            _data[_size] = null;

            return o;
        }

        public T RemoveLast()
        {
            if (_size > 0)
            {
                var o = _data[--_size];
                _data[_size] = null;

                return o;
            }

            return default(T);
        }

        public bool Remove(T o)
        {
            for (var i = 0; i < _size; i++)
            {
                object o1 = _data[i];

                if (!o.Equals(o1))
                    continue;

                _data[i] = _data[--_size];
                _data[_size] = null;
                return true;
            }

            return false;
        }

        public bool Contains(T o)
        {
            for (var i = 0; _size > i; i++)
            {
                if (o.Equals(_data[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEmpty()
        {
            var bReturn = true;

            for (var i = 0; i < _data.Length; i++)
            {
                if (_data[i] != null)
                {
                    bReturn = false;
                    break;
                }
            }

            return bReturn;
        }

        public bool RemoveAll(Bag<T> bag)
        {
            var modified = false;

            var bagSize = bag.Count;
            for (var i = 0; i < bagSize; i++)
            {
                object o1 = bag[i];

                for (var j = 0; j < _size; j++)
                {
                    object o2 = _data[j];

                    if (o1 != o2)
                        continue;

                    Remove(j);
                    j--;
                    modified = true;
                    break;
                }
            }

            return modified;
        }

        /*public bool IsEmpty()
        {
            return _size == 0;
        }*/

        public int Add(T o)
        {
            // is size greater than capacity increase capacity
            if (_size == _data.Length)
            {
                Grow();
            }

            _data[_size++] = o;

            return _size - 1;
        }

        public void Clear()
        {
            for (var i = 0; i < _size; i++)
            {
                _data[i] = null;
            }

            _size = 0;
        }

        public void AddAll(Bag<T> items)
        {
            for (int i = 0, j = items.Count; j > i; i++)
            {
                Add(items[i]);
            }
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (item == _data[i])
                    return i;
            }

            return -1;
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _size; i++)
            {
                action(_data[i]);
            }
        }

        public void ForEachWith(Action<T> action, Predicate<T> predicate)
        {
            for (int i = 0; i < _size; i++)
            {
                if (predicate(_data[i]))
                {
                    action(_data[i]);
                }
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
            _data = new T[newCapacity];
            Array.Copy(oldData, 0, _data, 0, oldData.Length);
        }
        #endregion

        #region IEnumerator implementation
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

        #region IXmlSerializable implementation
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