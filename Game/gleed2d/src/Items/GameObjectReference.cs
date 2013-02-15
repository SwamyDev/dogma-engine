using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GLEED2D
{
    // 
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GameObjectReference
    {
        private string _reference;

        [TypeConverter(typeof(GameObjectReferenceTypeConverter))]
        public string Reference 
        {
            get
            {
                string s = "";
                if (_reference != null)
                {
                    s = _reference;
                }

                return s;

            }
            set
            {
                _reference = value;
            }
        }
    }

    #region GameObjectReference type converter
    internal class GameObjectReferenceTypeConverter : TypeConverter
    {

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var svc = new StandardValuesCollection(Scene.GameObjectNames);

            return svc;
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
        {
            if (destType == typeof(string) && value is GameObjectReference)
            {
                GameObjectReference goReference = (GameObjectReference)value;
                return goReference.Reference;
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                if (!Scene.GameObjectNames.Contains(value))
                {
                    Scene.GameObjectNames.Add((string)value);
                    Scene.GameObjectNames.Sort();
                }

                return value;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
    #endregion
}
