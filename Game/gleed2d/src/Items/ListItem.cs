using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GLEED2D
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ListItem<T> : ICollection<T>, ICustomTypeDescriptor, IXmlSerializable
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
            for (int i = 0; i < _size; i++)
            {
                // For each employee create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance
                ListCollectionPropertyDescriptor<T> pd = new ListCollectionPropertyDescriptor<T>(this, i);
                pds.Add(pd);
            }
            return pds;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
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

    internal class ListCollectionPropertyDescriptor<T> : PropertyDescriptor
    {
        private ListItem<T> collection = null;
        private int index = -1;

        public ListCollectionPropertyDescriptor(ListItem<T> coll, int idx) 
            : base( "#"+idx.ToString(), null )
        {
            this.collection = coll;
            this.index = idx;
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
                return this.collection.GetType();
            }
        }

        public override string DisplayName
        {
            get 
            {
                return index.ToString();
            }
        }

        public override string Description
        {
            get
            {
                return "Contains a list of objects";
            }
        }

        public override object GetValue(object component)
        {
            return this.collection[index];
        }

        public override bool IsReadOnly
        {
            get { return false;  }
        }

        public override string Name
        {
            get { return "#"+index.ToString(); }
        }

        public override Type PropertyType
        {
            get { return typeof(T); }
        }

        public override void ResetValue(object component) {}

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            T newValue = (T)value;
            this.collection[index] = newValue;
        }
    }
}
