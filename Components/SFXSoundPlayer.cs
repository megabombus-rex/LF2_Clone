using LF2Clone.Base;
using LF2Clone.Events;

namespace LF2Clone.Components
{
    public class SFXSoundPlayer : Component
    {
        Guid? _resourceId;
        bool _isPlaying;

        public bool IsPlaying { get { return _isPlaying; } }

        // update with the rest of components
        public SFXSoundPlayer(int id, Guid? soundId)
        {
            _id = id;
            _resourceId = soundId;
            _isPlaying = false;
        }

        /// <summary>
        /// Sound manager givees the Id here based on the resource manager's data.
        /// </summary>
        /// <param name="soundId"> Id of given sound/music resource. </param>
        public void SetSFX(Guid? soundId)
        {
            _resourceId = soundId;
        }


        public void PlayCurrentSound()
        {
            if(!_resourceId.HasValue)
            {
                return;
            }
            SoundPlayed.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = true;
        }

        public void ChangeCurrentSoundStatus()
        {
            if (!_resourceId.HasValue) 
            {
                return;
            }
            SoundPausedOrResumed.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = !_isPlaying;
        }

        public void StopCurrentSound()
        {
            if (!_resourceId.HasValue)
            {
                return;
            }
            SoundStopped.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = false;
        }

        public void ChangeCurrentVolume(float volume)
        {
            if (!_resourceId.HasValue)
            {
                return;
            }
            VolumeChanged.Invoke(this, new SFXVolumeEventArgs() { _soundResourceId = _resourceId.Value, _volumeNormalized = volume });
        }

        public delegate void PlaySound(object sender, SFXEventArgs e);
        public event PlaySound SoundPlayed;

        public delegate void StopSound(object sender, SFXEventArgs e);
        public event StopSound SoundStopped;

        public delegate void ChangeStatusSound(object sender, SFXEventArgs e);
        public event ChangeStatusSound SoundPausedOrResumed;

        public delegate void ChangeVolume(object sender, SFXVolumeEventArgs e);
        public event ChangeVolume VolumeChanged;
    }
}
