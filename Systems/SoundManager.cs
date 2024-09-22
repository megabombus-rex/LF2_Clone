using LF2Clone.Base;
using LF2Clone.Components;
using LF2Clone.Events;
using LF2Clone.Misc.Logger;
using LF2Clone.Resources;
using Raylib_cs;

namespace LF2Clone.Systems
{
    public sealed class SoundManager : System<SoundManager>
    {
        // dictionary of sounds loaded by resource manager (not implemented yet)
        Dictionary<Guid, SFX> _soundsDict;
        public SoundManager(ILogger logger) : base(logger)
        {
            _soundsDict = new Dictionary<Guid, SFX>();
        }

        public void AddSFX(Guid sfxId, SFX res)
        {
            _soundsDict.TryAdd(sfxId, res);
        }

        public void Play(object sender, PlaySoundEventArgs e)
        {
            var sfxPlayer = sender as SFXSoundPlayer;
            _logger.LogInfo(string.Format("SFXPlayer calling is {0}. \n Sound id: {1}", sfxPlayer?._id, e._soundResourceId));

            var sound = _soundsDict[e._soundResourceId];

            switch (sound._type)
            {
                case SFX.SoundType.Sound:
                    Raylib.PlaySound((Sound)sound._value);       break;
                case SFX.SoundType.Music:
                    Raylib.PlayMusicStream((Music)sound._value); break;
            }
        }

        // music overlay, may discard later
        private Music _musicPlaying;
        private bool _isMusicPlaying;
    }
}
