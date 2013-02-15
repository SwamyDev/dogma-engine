using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Pontification
{
    public struct SoundData
    {
        public string Name;
        public string AssetName;
        public float Volume;
        public float Pitch;
        public float Pan;
        public int Priority;
        public bool IsLooping;

        public SoundData(string name, string assetName, float volume, float pitch, float pan, int priority, bool isLooping)
        {
            Name = name;
            AssetName = assetName;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
            Priority = priority;
            IsLooping = isLooping;
        }
    }

    public class SoundEffects
    {
        #region Private attributes
        private Dictionary<string, SoundEffectInstance> _soundEffects = new Dictionary<string, SoundEffectInstance>();
        private SoundData _currentSoundData;
        private SoundEffectInstance _currentSound;
        #endregion

        #region Public properties
        public Dictionary<string, SoundData> SoundDictionary { get; set; }
        public SoundData CurrentSound { get { return _currentSoundData; } }
        #endregion

        public SoundEffects()
        {
            SoundDictionary = new Dictionary<string, SoundData>();
        }

        public void LoadSoundEffects(ContentManager cm)
        {
            foreach (var pair in SoundDictionary)
            {
                var soundEffect = cm.Load<SoundEffect>(pair.Value.AssetName);
                SoundEffectInstance sound = soundEffect.CreateInstance();
                sound.Volume = pair.Value.Volume;
                sound.Pitch = pair.Value.Pitch;
                sound.Pan = pair.Value.Pan;
                sound.IsLooped = pair.Value.IsLooping;

                _soundEffects.Add(pair.Key, sound);
            }
        }

        public void Play(string name)
        {
            SoundData data;

            Console.WriteLine(string.Format("Try to play sound {0}.", name));

            if (SoundDictionary.TryGetValue(name, out data))
            {
                SoundEffectInstance sound = _soundEffects[data.Name];
                if ((_currentSoundData.Priority <= data.Priority || _currentSound.State != SoundState.Playing) && sound.State != SoundState.Playing)
                {
                    if (_currentSound != null)
                        _currentSound.Stop();

                    _currentSoundData = data;
                    _currentSound = sound;
                    sound.Play();
                }
            }
        }

        public void Pause()
        {
            SoundEffectInstance sound;
            if (_soundEffects.TryGetValue(_currentSoundData.Name, out sound))
            {
                if (sound != null)
                    sound.Pause();
            }
        }

        public void Stop()
        {
            SoundEffectInstance sound;
            if (_currentSoundData.Name != null && _soundEffects.TryGetValue(_currentSoundData.Name, out sound))
            {
                if (sound != null)
                    sound.Stop();
            }
        }
    }
}
