namespace LF2Clone.Events
{
    public sealed class SFXVolumeEventArgs : EventArgs
    {
        public Guid _soundResourceId;
        public float _volumeNormalized;
    }
}
