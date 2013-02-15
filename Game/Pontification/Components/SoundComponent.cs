using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class SoundComponent : Component
    {
        #region Public properties
        public SoundEffects Sounds { get; set; }
        #endregion

        public void Play(string name)
        {
            if (name == "")
                Sounds.Stop();
            else
                Sounds.Play(name);
        }
    }
}
