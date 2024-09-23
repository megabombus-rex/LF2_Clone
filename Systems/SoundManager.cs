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
        private Music? _musicPlayed;
        private bool _isLooped;
        private bool _isMusicPlaying;

        public SoundManager(ILogger logger) : base(logger)
        {
            _soundsDict = new Dictionary<Guid, SFX>();
            _musicValues = Enumerable.Empty<SFX>();
            _isMusicPlaying = false;
        }

        public void SetMusicPlayed(Music music)
        {
            _musicPlayed = music;
        }

        public void AddSFX(SFX res)
        {
            _soundsDict.TryAdd(res._id, res);
            if (res._type == SFX.SoundType.Music)
            {
                _musicValues = _soundsDict.Values.Where(x => x._type == SFX.SoundType.Music);
            }
        }

        public void AddSFXBundle(IEnumerable<SFX> resources)
        {
            foreach (var res in resources)
            {
                _soundsDict.TryAdd(res._id, res);
            }

            if (resources.Any(x => x._type == SFX.SoundType.Music))
            {
                _musicValues = _soundsDict.Values.Where(x => x._type == SFX.SoundType.Music);
            }
        }

        public void Play(object sender, SFXEventArgs e)
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

        public void Stop(object sender, SFXEventArgs e)
        {
            var sfxPlayer = sender as SFXSoundPlayer;
            _logger.LogInfo(string.Format("SFXPlayer calling is {0}. \n Sound id: {1}", sfxPlayer?._id, e._soundResourceId));

            var sound = _soundsDict[e._soundResourceId];

            switch (sound._type)
            {
                case SFX.SoundType.Sound:
                    Raylib.StopSound((Sound)sound._value); break;
                case SFX.SoundType.Music:
                    Raylib.StopMusicStream((Music)sound._value); break;
            }
        }

        // make one event, SFXEventArgs + operationType (Play/Pause/Resume/Stop)
        public void ChangeStatus(object sender, SFXEventArgs e)
        {
            var sfxPlayer = sender as SFXSoundPlayer;
            _logger.LogInfo(string.Format("SFXPlayer calling is {0}. \n Sound id: {1}", sfxPlayer?._id, e._soundResourceId));

            var sound = _soundsDict[e._soundResourceId];

            switch (sound._type)
            {
                case SFX.SoundType.Sound:
                    if (sfxPlayer!.IsPlaying)
                    {
                        Raylib.PauseSound((Sound)sound._value);
                        break;
                    }
                    Raylib.ResumeSound((Sound)sound._value);
                    break;
                case SFX.SoundType.Music:
                    if (sfxPlayer!.IsPlaying)
                    {
                        Raylib.PauseMusicStream((Music)sound._value);
                        break;
                    }
                    Raylib.ResumeMusicStream((Music)sound._value);
                    break;
            }
        }

        public void ChangeVolume(object sender, SFXVolumeEventArgs e)
        {
            var sfxPlayer = sender as SFXSoundPlayer;

            var sound = _soundsDict[e._soundResourceId];

            var volume = e._volumeNormalized > 1.0f ? 1.0f : e._volumeNormalized;

            switch (sound._type)
            {
                case SFX.SoundType.Sound:
                    Raylib.SetAudioStreamVolume(((Sound)sound._value).Stream, volume);
                    break;
                case SFX.SoundType.Music:
                    Raylib.SetAudioStreamVolume(((Music)sound._value).Stream, volume);
                    break;
            }

        }

        public override void Update()
        {
            base.Update();
            foreach (var sound in _musicValues)
            {
                Raylib.UpdateMusicStream((Music)sound._value);
            }
        }

        public override void Destroy()
        {
            if(_soundsDict.Values.Any())
            {
                foreach (var sound in _soundsDict.Values)
                {
                    switch (sound._type)
                    {
                        case SFX.SoundType.Music: Raylib.UnloadAudioStream(((Music)sound._value).Stream); break;
                        case SFX.SoundType.Sound: Raylib.UnloadSound(((Sound)sound._value)); break;
                    }
                    _logger.LogInfo($"Sound: {sound._type} removed.");
                }
            }

            Raylib.CloseAudioDevice();
            base.Destroy();
        }
    }
}
