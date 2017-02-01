using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    class Player : Ship
    {
        public static Size PlayerSize = new Size(25, 15);
        public static double Speed = 10;
        public Player(Point location, Size size) : base(location, size)
        {
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    break;

                case  Direction.Left:
                    break;
                default:
                    break;
            }
        }
    }
}
