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
        IEnumerable<SFX> _musicValues;

        // music overlay, may discard later
        private Music _musicPlaying;
        private bool _isMusicPlaying;

        public SoundManager(ILogger logger) : base(logger)
        {
            _soundsDict = new Dictionary<Guid, SFX>();
        }

        public void SetMusicPlaying()
        {

        }

        public void AddSFX(Guid sfxId, SFX res)
        {
            _soundsDict.TryAdd(sfxId, res);
            if (res._type == SFX.SoundType.Music)
            {
                _musicValues = _soundsDict.Values.Where(x => x._type == SFX.SoundType.Music);
            }
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

        public override void Update()
        {
            base.Update();
            // maybe for few seconds?
            foreach (var sound in _soundsDict.Values.Where(x => x._type == SFX.SoundType.Music))
            {
                Raylib.UpdateMusicStream((Music)sound._value);
            }
        }

        public override void Destroy()
        {
            if(_musicValues.Count() > 0)
            {
                foreach (var sound in _musicValues)
                {
                    Raylib.UnloadAudioStream(((Music)sound._value).Stream);
                }
            }

            base.Destroy();
        }
    }
}
