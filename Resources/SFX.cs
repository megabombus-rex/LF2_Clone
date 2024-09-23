namespace LF2Clone.Resources
{
    public class SFX : Resource
    {
        public SoundType _type;
        public float _durationInSeconds;
        public object _value; // sound or music
        public float _volumeNormalized;
        
        public enum SoundType
        {
            Sound,
            Music
        }
    }
}
