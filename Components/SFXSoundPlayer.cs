using LF2Clone.Base;
using LF2Clone.Events;
using LF2Clone.Resources;

namespace LF2Clone.Components
{
    public class SFXSoundPlayer : Component
    {
        Guid? _resourceId;
        bool _isPlaying;

        public bool IsPlaying { get { return _isPlaying; } }

        // update with the rest of components
        public SFXSoundPlayer(int id, string name, Guid? soundId)
        {
            _id = id;
            _name = name;
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
            Play.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = true;
        }

        public void ChangeCurrentSoundStatus()
        {
            if (!_resourceId.HasValue) 
            {
                return;
            }
            PauseResume.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = !_isPlaying;
        }

        public void StopCurrentSound()
        {
            if (!_resourceId.HasValue)
            {
                return;
            }
            Stop.Invoke(this, new SFXEventArgs() { _soundResourceId = _resourceId.Value });
            _isPlaying = false;
        }

        public delegate void PlaySound(object sender, SFXEventArgs e);
        public event PlaySound Play;

        public delegate void StopSound(object sender, SFXEventArgs e);
        public event StopSound Stop;

        public delegate void ChangeStatusSound(object sender, SFXEventArgs e);
        public event ChangeStatusSound PauseResume;
    }
}
