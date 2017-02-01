using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    class Invader : Ship
    {
        public Point Location { get; private set; }
        public Size InvaderSize { get; private set; }
        InvaderType InvaderType;
        public int Score { get; set; }


        public Invader(Point location, Size size) : base(location, size)
        {
            Location = location;
            InvaderSize = size;
        }

        public Invader(InvaderType type, int score)
        {
            InvaderType = type;
            Score += score;
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    break;

                case Direction.Right:
                    break;

                case Direction.Up:
                    break;

                case Direction.Down:
                    break;

                default:
                    break;
            }
        }
    }
}
