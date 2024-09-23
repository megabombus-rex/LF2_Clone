using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF2Clone.Events
{
    public sealed class SFXVolumeEventArgs : EventArgs
    {
        public Guid _soundResourceId;
        public float _volume;
    }
}
