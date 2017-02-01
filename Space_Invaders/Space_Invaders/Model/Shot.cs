using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    class Shot
    {

        //predkosc pocisku
        public const double ShotPixelsPerSeconds = 95;
        public Point Location { get; private set; }
        public static Size ShotSize = new Size(2, 10);

        private Direction _direction;
        public Direction Direction { get; private set; }

        private DateTime _lastMoved;

        public Shot(Point location, Direction direction) 
        {
            Location = location;
            Direction = Direction;
            _direction = direction;
            _lastMoved = DateTime.Now;
        }

        public void Move()
        {
            TimeSpan timeSincelastMoved = DateTime.Now - _lastMoved;
            double distance = timeSincelastMoved.Milliseconds * ShotPixelsPerSeconds / 1000;

            if (Direction == Direction.Up) distance *= -1;

            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}
