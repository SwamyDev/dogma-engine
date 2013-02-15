using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pontification;

namespace GLEED2D
{
    #region Animation data class
    public class AnimationDataItem
    {
        [DisplayName("Name"), Category(" General")]
        [Description("This is the name of the animation")]
        public string Name { get; set; }

        [DisplayName("Duration"), Category(" General")]
        [Description("This is the duration of the animation")]
        public float Duration { get; set; }

        [DisplayName("Priority"), Category(" General")]
        [Description("The priority which is used to override an animation when play is called")]
        public int Priority { get; set; }

        [DisplayName("Frames"), Category(" General")]
        [Description("The frame count of this animation")]
        public int Frames { get; set; }

        [DisplayName("Column"), Category(" General")]
        [Description("The column of the animation within the sprite")]
        public int Column { get; set; }

        [DisplayName("IsLooping"), Category(" General")]
        [Description("Wheather the animation should loop or not")]
        public bool IsLooping { get; set; }

        public AnimationDataItem()
            : this("None", 1, 0, 1, 1, false)
        {
        }

        public AnimationDataItem(string name, float duration, int priority, int frames, int column, bool isLooping)
        {
            this.Name = name;
            this.Duration = duration;
            this.Priority = priority;
            this.Frames = frames;
            this.Column = column;
            this.IsLooping = isLooping;
        }

        #region Animation expandable collection converter class
        internal class AnimationDataExpandableCollectionConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
            {
                if (destType == typeof(string) && value is AnimationDataItem)
                {
                    AnimationDataItem animData = (AnimationDataItem)value;
                    return animData.Name;
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
        #endregion
    }
    #endregion

    public class AnimationItem : TextureItem
    {
        // Factory function
        public static AnimationItem CreateEmpty(GameObjectItem go)
        {
            var animation = new AnimationItem();
            animation.gameObject = go;
            animation.layer = go.layer;
            animation.AnimationData = new List<AnimationDataItem>();
            animation.Position = go.Position;
            animation.TintColor = Microsoft.Xna.Framework.Color.White;
            animation.Scale = Microsoft.Xna.Framework.Vector2.One;
            animation.FrameWidth = 116;
            animation.Columns = 1;

            return animation;
        }

        private Animation _sprite;

        public List<AnimationDataItem> AnimationData { get; set; }
        public int FrameWidth { get; set; }
        public int Columns { get; set; }

        public void UpdateAnimation()
        {
            if (texture != null)
            {
                _sprite = new Animation(texture, FrameWidth, Columns);
                OnTransformed();
            }
            foreach (var animData in AnimationData)
            {
                _sprite.AnimationDictionary.Add(animData.Name, new AnimationData(animData.Name, animData.Duration, animData.Priority, animData.Frames, animData.Column, animData.IsLooping));
            }
        }

        public void PlayAnimation(string name)
        {
            _sprite.PlayAnimation(name);
        }

        public override bool loadIntoEditor()
        {
            if (base.loadIntoEditor())
            {
                UpdateAnimation();
                return true;
            }

            return false;
        }

        public override void OnTransformed()
        {
            transform =
                Matrix.CreateTranslation(new Vector3(-Origin.X, -Origin.Y, 0.0f)) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            // 0 values are invalis so catch them.
            if (FrameWidth == 0)
                FrameWidth = 116;
            if (Columns == 0)
                Columns = 1;

            Vector2 leftTop = new Vector2(0, 0);
            Vector2 rightTop = new Vector2(FrameWidth, 0);
            Vector2 leftBottom = new Vector2(0, texture.Height / Columns);
            Vector2 rightBottom = new Vector2(FrameWidth, texture.Height / Columns);

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

        public override void draw(SpriteBatch sb)
        {
            if (!Visible) return;
            SpriteEffects effects = SpriteEffects.None;
            if (FlipHorizontally) effects |= SpriteEffects.FlipHorizontally;
            if (FlipVertically) effects |= SpriteEffects.FlipVertically;
            _sprite.Draw(Editor.DeltaTime, sb, Position, effects);
        }

        public override void drawInEditor(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (!Visible) return;

            SpriteEffects se = SpriteEffects.None;

            if (pFlipHorizontally) 
                se |= SpriteEffects.FlipHorizontally;
            if (pFlipVertically) 
                se |= SpriteEffects.FlipVertically;

            Color c = TintColor;
            if (hovering && Constants.Instance.EnableHighlightOnMouseOver) c = Constants.Instance.ColorHighlight;
            _sprite.Draw(Editor.DeltaTime, sb, Position, se);
        }
    }
}
