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
        public readonly static Size PlayerSize = new Size(25, 15);
        public static double Speed = 10;
        public Player() : base(new Point(PlayerSize.Width / 2, 0), PlayerSize)
        {
            Location = new Point(Location.X, InvadersModel.PlayAreaSize.Height - PlayerSize.Height * 3);
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    if (Location.X < InvadersModel.PlayAreaSize.Width - PlayerSize.Width * 1.5)
                    {
                        Location = new Point(Location.X - Speed, Location.Y);
                    }
                    break;

                case  Direction.Left:
                    if (Location.X > PlayerSize.Width / 2)
                    {
                        Location = new Point(Location.X - Speed, Location.Y);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
