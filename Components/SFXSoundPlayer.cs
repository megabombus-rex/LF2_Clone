using LF2Clone.Base;
using LF2Clone.Events;
using LF2Clone.Resources;

namespace LF2Clone.Components
{
    public class SFXSoundPlayer : Component
    {
        // update with the rest of components
        public SFXSoundPlayer(int id, string name, Guid? soundId)
        {
            _id = id;
            _name = name;
            _resourceId = soundId;
        }

        /// <summary>
        /// Sound manager givees the Id here based on the resource manager's data.
        /// </summary>
        /// <param name="soundId"> Id of given sound/music resource. </param>
        public void SetSFX(Guid? soundId)
        {
            _resourceId = soundId;
        }

        Guid? _resourceId;

        public void PlayCurrentSound()
        {
            if(_resourceId == null)
            {
                return;
            }
            Play.Invoke(this, new PlaySoundEventArgs() { _soundResourceId = _resourceId.Value });
        }

        public delegate void PlaySound(object sender, PlaySoundEventArgs e);
        public event PlaySound Play;
    }
}
