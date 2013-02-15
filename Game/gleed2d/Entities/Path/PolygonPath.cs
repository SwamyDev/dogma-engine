using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace GLEED2D
{
    [TypeConverter(typeof(PolygonPathExpandableCollectionConverter))]
    public class PolygonPath : PathItem
    {
        // Factory functions
        public static PolygonPath CreateEmpty(GameObjectItem go)
        {
            var polygon = new PolygonPath(go);
            polygon.gameObject = go;
            polygon.layer = go.layer;

            return polygon;
        }

        public PolygonPath(GameObjectItem gameObject)
        {
            Position = gameObject.Position;
            LineWidth = Constants.Instance.DefaultPathItemLineWidth;
            LineColor = Constants.Instance.ColorPrimitives;

            IsPolygon = true;
        }

        public PolygonPath()
        {
            IsPolygon = true;
        }

        public override string getNamePrefix()
        {
            return "PolygonPath_";
        }

        public void SetPolygon(Vector2[] points)
        {
            WorldPoints = points;
            LocalPoints = (Vector2[])points.Clone();
            for (int i = 0; i < LocalPoints.Length; i++) LocalPoints[i] -= Position;
        }
    }

    #region PolygonPath expandable collection converter class
    internal class PolygonPathExpandableCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                         System.Globalization.CultureInfo culture,
                         object value, Type destType)
        {
            if (destType == typeof(string) && value is PolygonPath)
            {
                PolygonPath polygonPath = (PolygonPath)value;
                return string.Format("Vertices: {0}", polygonPath.WorldPoints == null ? 0 : polygonPath.WorldPoints.Length);
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
    #endregion
}
