using LF2Clone.Base;
using LF2Clone.Misc.Logger;
using Raylib_cs;

namespace LF2Clone.Systems
{
    public sealed class SoundManager : System<SoundManager>
    {
        public SoundManager(ILogger logger) : base(logger)
        {
        }

        private Music _musicPlaying;
        private bool _isMusicPlaying;

        public event PlaySound Play;
        public delegate void PlaySound(object sender, EventArgs e);
    }
}
