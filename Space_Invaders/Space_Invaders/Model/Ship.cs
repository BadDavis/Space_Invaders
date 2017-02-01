using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    abstract class Ship
    {
        //położenie i wielkosc gracza/przeciwnika
        public Point Location { get; protected set; }
        public Size Size { get; private set; }
        public Rect Area { get { return new Rect(Location, Size); } }

        public Ship(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public abstract void Move(Direction direction);
    }
}
