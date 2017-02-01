using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders.Model
{
    class ShotMovedEventArgs : EventArgs
    {
        public Shot Shot { get; private set; }
        public bool Dissapeared { get; private set; }

        public ShotMovedEventArgs(Shot shot, bool dissapeared)
        {
            Shot = shot;
            Dissapeared = dissapeared;
        }
    }
}
