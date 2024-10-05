using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF2Clone.Exceptions
{
    public class SceneNotLoadedException : Exception
    {
        public SceneNotLoadedException(string message) : base(message)
        {
            
        }
    }
}
