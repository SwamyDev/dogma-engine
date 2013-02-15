using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Pontification
{
    public class Music
    {
        private static Dictionary<string, Song> _songs = new Dictionary<string, Song>();

        public static void Load(ContentManager cm)
        {
			#if WINDOWS || XBOX
            MediaPlayer.IsRepeating = true;

            var song = cm.Load<Song>("Levels/Level00/background-music");
            _songs.Add("background-music", song);
			#endif
        }

        public static void Unload()
        {
			#if WINDOWS || XBOX
            MediaPlayer.Stop();
			#endif
        }

        public static void Play(string name)
        {
			#if WINDOWS || XBOX
            Song song;
            if (_songs.TryGetValue(name, out song))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(song);
            }
			#endif
        }
    }
}
