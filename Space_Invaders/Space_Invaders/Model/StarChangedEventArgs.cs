using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    class StarChangedEventArgs : EventArgs
    {
        public Point Point { get; private set; }
        public bool Dissapeared { get; private set; }

        public StarChangedEventArgs(Point point, bool dissapeared)
        {
            Point = point;
            Dissapeared = dissapeared;
        }
    }
}
