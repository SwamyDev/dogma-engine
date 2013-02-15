using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLEED2D
{
    public partial class GameObjectItem
    {
        #region Private attributes.
        private Matrix transform;
        private Rectangle boundingrectangle;    //bounding rectangle in world space, for collision broadphase
        private Vector2[] polygon = new Vector2[4];              //selection box: drawn when selected
        private Texture2D _icon;
        private int _selectionWidth = 32;
        private int _selectionHeight = 32;
        #endregion

        #region PropertyGrid properties
        [XmlIgnore()]
        [DisplayName("Rotation"), Category(" General")]
        [Description("The item's rotation in radians.")]
        public float pRotation
        {
            get
            {
                return Rotation;
            }
            set
            {
                Rotation = value;
                OnTransformed();
            }
        }

        [XmlIgnore()]
        [DisplayName("Scale"), Category(" General")]
        [Description("The item's scale vector.")]
        public Vector2 pScale
        {
            get
            {
                return Scale;
            }
            set
            {
                Scale = value;
                OnTransformed();
            }
        }

        [XmlIgnore()]
        [DisplayName("IsActive"), Category(" General")]
        [Description("Wether the game object is active or not")]
        public bool pIsActive
        {
            get
            {
                return IsActive;
            }
            set
            {
                IsActive = value;
            }
        }
        #endregion

        public GameObjectItem(Vector2 position)
            : base()
        {
            Position = position;
            Rotation = 0;
            Scale = Vector2.One;
            Components = new ComponentItemCollection();

            // Load icon for game object.
            _icon = Game1.Instance.Content.Load<Texture2D>("icon_gameobject");

            OnTransformed();
        }

        public List<Item> GetSelectables()
        {
            List<Item> selectables = new List<Item>();
            foreach (var comp in Components)
            {
                selectables.AddRange(comp.GetSelectables());
            }

            return selectables;
        }

        public override Item clone()
        {
            GameObjectItem result = (GameObjectItem)this.MemberwiseClone();
            result.CustomProperties = new SerializableDictionary(CustomProperties);
            result.polygon = (Vector2[])polygon.Clone();
            result.hovering = false;
            return result;
        }

        public override string getNamePrefix()
        {
            return "GameObject_";
        }

        public override void OnTransformed()
        {
            transform =
                Matrix.CreateTranslation(new Vector3(-_selectionWidth / 2, -_selectionHeight / 2, 0.0f)) *
                Matrix.CreateScale(Scale.X, Scale.Y, 1) *
                Matrix.CreateRotationZ(Rotation) *
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


        public override void onMouseButtonDown(Vector2 mouseworldpos)
        {
            hovering = false;
            MainForm.Instance.pictureBox1.Cursor = Cursors.SizeAll;
            base.onMouseButtonDown(mouseworldpos);
        }


        public override bool CanRotate()
        {
            return true;
        }

        public override float getRotation()
        {
            return pRotation;
        }

        public override void setRotation(float rotation)
        {
            pRotation = rotation;
        }


        public override bool CanScale()
        {
            return true;
        }

        public override Vector2 getScale()
        {
            return pScale;
        }

        public override void setScale(Vector2 scale)
        {
            pScale = scale;
        }

        public override void setPosition(Vector2 pos)
        {
            // Update component positons as well.
            foreach (var c in Components)
                foreach (var item in c.GetSelectables())
                    item.setPosition(pos - Position + item.pPosition);
            
            base.setPosition(pos);
        }

        public override bool loadIntoEditor()
        {
            foreach (var c in Components)
                foreach (var item in c.GetSelectables())
                    item.loadIntoEditor();

            return true;
        }

        public override void drawInEditor(SpriteBatch sb)
        {
            if (!Visible) return;

            // Draw components
            foreach (var component in Components)
                foreach (var texture in component.GetTextures())
                    texture.drawInEditor(sb);

            // Draw textures first and then polgons so polygons are always on top.
            foreach (var component in Components)
                foreach (var polygon in component.GetPolygons())
                    polygon.drawInEditor(sb);

            Color c = Color.White;
            if (hovering && Constants.Instance.EnableHighlightOnMouseOver) c = Constants.Instance.ColorHighlight;
            sb.Draw(_icon, Position, null, c, Rotation, new Vector2(_selectionWidth / 2, _selectionHeight / 2), Scale, SpriteEffects.None, 0);

            // Draw sound icons on top of that
            foreach (var component in Components)
                foreach (var sound in component.GetSounds())
                    sound.drawInEditor(sb);
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
    }
}
