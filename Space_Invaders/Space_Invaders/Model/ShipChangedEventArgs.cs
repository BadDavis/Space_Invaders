using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders.Model
{
    class ShipChangedEventArgs : EventArgs
    {
        public Ship ShipUpdated { get; private set; }
        public bool Killed { get; private set; }

        public ShipChangedEventArgs(Ship ship, bool killed)
        {
            ShipUpdated = ship;
            Killed = killed;
        }
    }
}
