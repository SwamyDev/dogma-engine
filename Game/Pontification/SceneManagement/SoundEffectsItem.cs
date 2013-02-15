using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Pontification.Components;

namespace Pontification.SceneManagement
{
    #region SoundDataItem data class
    public class SoundDataItem
    {
        public string Name { get; set; }
        public string AssetName { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public float Pan { get; set; }
        public int Priority { get; set; }
        public bool IsLooping { get; set; }

        public SoundDataItem()
        {
        }
    }
    #endregion

    public class SoundEffectsItem : Item
    {
        public ListItem<SoundDataItem> SoundData { get; set; }

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);
            var sound = new SoundEffects();

            foreach (var sdi in SoundData)
            {
                var soundData = new SoundData(sdi.Name, sdi.AssetName, sdi.Volume, sdi.Pitch, sdi.Pan, sdi.Priority, sdi.IsLooping);
                sound.SoundDictionary.Add(sdi.Name, soundData);
            }
            sound.LoadSoundEffects(cm);

            prop.SetValue(comp, sound, null);
        }
    }
}
