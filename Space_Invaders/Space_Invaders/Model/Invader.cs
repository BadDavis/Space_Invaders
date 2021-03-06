﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    class Invader : Ship
    {
        //public Point Location { get; private set; }
        public static Size InvaderSize = new Size(15, 25);
        public InvaderType InvaderType { get; private set; }
        public int Score { get; set; }


        public const double HorizontalPixelsPerMove = 5;
        public const double VerticalPixelsPerMove = 15;

        public Invader(InvaderType invaderType, Point location, int score)
            : base(location, InvaderSize)
        {
            InvaderType = invaderType;
            Score = score;
        }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    Location = new Point(Location.X - HorizontalPixelsPerMove, Location.Y);
                    break;

                case Direction.Right:
                    Location = new Point(Location.X + HorizontalPixelsPerMove, Location.Y);
                    break;

                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + VerticalPixelsPerMove);
                    break;

                default:
                    break;
            }
        }
    }
}
