using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pontification.Components;

namespace Pontification.SceneManagement
{
    #region Animation data class
    public class AnimationDataItem
    {
        public string Name { get; set; }
        public float Duration { get; set; }
        public int Priority { get; set; }
        public int Frames { get; set; }
        public int Column { get; set; }
        public bool IsLooping { get; set; }

        public AnimationDataItem()
        {
        }
    }
    #endregion

    public class AnimationItem : TextureItem
    {
        public List<AnimationDataItem> AnimationData { get; set; }
        public int FrameWidth { get; set; }
        public int Columns { get; set; }

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);
            var texture = cm.Load<Texture2D>(asset_name);
            var anim = new Animation(texture, FrameWidth, Columns);

            AnimationData.ForEach((adi) => 
            {
                var animData = new AnimationData(adi.Name, adi.Duration, adi.Priority, adi.Frames, adi.Column, adi.IsLooping);
                anim.AnimationDictionary.Add(adi.Name, animData);
            });

            prop.SetValue(comp, anim, null);
        }
    }
}
