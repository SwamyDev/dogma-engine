using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Pontification;
using Microsoft.Xna.Framework.Graphics;

namespace GLEED2D
{
    #region Sound data class
    [TypeConverter(typeof(SoundDataItemExpandableCollectionConverter))]
    public class SoundDataItem
    {
        [DisplayName("Name"), Category(" General")]
        [Description("This is the name of the sound")]
        public string Name { get; set; }

        [DisplayName("Asset Name"), Category(" General")]
        [Description("This is the asset name of the sound")]
        public string AssetName { get; set; }

        [DisplayName("Volume"), Category(" General")]
        [Description("This is the volume of the sound")]
        public float Volume { get; set; }

        [DisplayName("Pitch"), Category(" General")]
        [Description("This is the pitch of the sound")]
        public float Pitch { get; set; }

        [DisplayName("Pan"), Category(" General")]
        [Description("This is the pan of the sound")]
        public float Pan { get; set; }

        [DisplayName("Priority"), Category(" General")]
        [Description("This is the priority of the sound")]
        public int Priority { get; set; }

        [DisplayName("IsLooping"), Category(" General")]
        [Description("Wheather the sound should loop or not")]
        public bool IsLooping { get; set; }

        public SoundDataItem()
            : this("", "", 1, 0, 0, 0, false)
        {
        }

        public SoundDataItem(string name, string assetName, float volume, float pitch, float pan, int priority, bool isLooping)
        {
            Name = name;
            AssetName = assetName;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            Priority = priority;
            IsLooping = isLooping;
        }

        #region SoundDataItem expandable collection converter class
        internal class SoundDataItemExpandableCollectionConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
            {
                if (destType == typeof(string) && value is SoundDataItem)
                {
                    SoundDataItem soundData = (SoundDataItem)value;
                    return soundData.Name;
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
        #endregion
    }
    #endregion

    [TypeConverter(typeof(SoundEffectsItemExpandableCollectionConverter))]
    public class SoundEffectsItem : Item
    {
        // Factory function
        public static SoundEffectsItem CreateEmpty(GameObjectItem go)
        {
            var sound = new SoundEffectsItem();
            sound.SoundData = new ListItem<SoundDataItem>();
            sound.gameObject = go;
            sound.layer = go.layer;
            sound.Position = go.Position;

            return sound;
        }

        #region Private attributes.
        private Matrix transform;
        private Rectangle boundingrectangle;    //bounding rectangle in world space, for collision broadphase
        private Vector2[] polygon = new Vector2[4];              //selection box: drawn when selected
        private Texture2D _icon;
        private int _selectionWidth = 32;
        private int _selectionHeight = 32;
        #endregion

        public ListItem<SoundDataItem> SoundData { get; set; }

        public SoundEffectsItem()
        {
            _icon = Game1.Instance.Content.Load<Texture2D>("sound-icon");
            Visible = true;
        }

        public override Item clone()
        {
            SoundEffectsItem result = (SoundEffectsItem)this.MemberwiseClone();
            result.SoundData = new ListItem<SoundDataItem>();
            foreach (SoundDataItem data in SoundData)
                result.SoundData.Add(data);

            result.polygon = (Vector2[])polygon.Clone();
            result.hovering = false;
            return result;
        }

        public override string getNamePrefix()
        {
            return "SoundEffects_";
        }

        public override void OnTransformed()
        {
            transform =
                Matrix.CreateTranslation(new Vector3(-_selectionWidth / 2, -_selectionHeight / 2, 0.0f)) *
                Matrix.CreateScale(1, 1, 1) *
                Matrix.CreateRotationZ(0) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            Vector2 leftTop = new Vector2(0, 0);
            Vector2 rightTop = new Vector2(_selectionWidth, 0);
            Vector2 leftBottom = new Vector2(0, _selectionHeight);
            Vector2 rightBottom = new Vector2(_selectionWidth, _selectionHeight);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            polygon[0] = leftTop;
            polygon[1] = rightTop;
            polygon[3] = leftBottom;
            polygon[2] = rightBottom;

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return as a rectangle
            boundingrectangle = new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public override void drawInEditor(SpriteBatch sb)
        {
            if (!Visible) return;

            Color c = Color.White;
            if (hovering && Constants.Instance.EnableHighlightOnMouseOver) c = Constants.Instance.ColorHighlight;
            sb.Draw(_icon, Position, null, c, 0, new Vector2(_icon.Width / 2, _icon.Height / 2), 1f, SpriteEffects.None, 0);
        }

        public override void drawSelectionFrame(SpriteBatch sb, Matrix matrix, Color color)
        {
            Vector2[] poly = new Vector2[4];
            Vector2.Transform(polygon, ref matrix, poly);

            Primitives.Instance.drawPolygon(sb, poly, color, 2);
            foreach (Vector2 p in poly)
            {
                Primitives.Instance.drawCircleFilled(sb, p, 4, color);
            }
            Vector2 origin = Vector2.Transform(pPosition, matrix);
            Primitives.Instance.drawBoxFilled(sb, origin.X - 5, origin.Y - 5, 10, 10, color);
        }

        public override bool contains(Vector2 worldpos)
        {
            if (boundingrectangle.Contains((int)worldpos.X, (int)worldpos.Y))
            {
                return true;
            }
            return false;
        }

        #region SoundEffectsItem expandable collection converter class
        internal class SoundEffectsItemExpandableCollectionConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
            {
                if (destType == typeof(string) && value is SoundEffectsItem)
                {
                    return "Sound effects";
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
        #endregion
    }
}
