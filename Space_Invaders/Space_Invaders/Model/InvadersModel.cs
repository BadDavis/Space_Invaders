using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Invaders.Model
{
    //thegame
    class InvadersModel
    {

        public readonly static Size PlayAreaSize = new Size(400, 300);
        public const int MaximumPlayerShots = 3;
        public const int InitialStarCount = 50; // gwiazdy w tle

        private readonly Random _random = new Random();

        public int Score { get; private set; }
        public int Wave { get; private set; } //fala wrogow
        public int Lives { get; private set; }

        public bool GameOver { get; private set; }

        private DateTime? _playerDied = null;
        public bool PlayerDying { get { return _playerDied.HasValue; } } //gdy ginie gracz miga jego statek i reszta stoi

        private Player _player;

        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShot = new List<Shot>();
        private readonly List<Shot> _invaderShot = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();



        private Direction _invaderDirection = Direction.Left;
        private bool _justMovedDown = false;

        private DateTime _lastUpdated = DateTime.MinValue;

        public InvadersModel() {
            EndGame();
        }

        private void EndGame()
        {
            GameOver = true;
        }

        public void StartGame()
        {
            GameOver = false;
            ShipChanged();
            _invaders.Clear();
            ShotMoved();
            _playerShot.Clear();
            _invaderShot.Clear();

            _stars.Clear();
            StarChangedEventArgs();
            _stars.Add(new Point(1,8));
            ShipChanged("Player");
            Lives = 2;
            Wave = 0;
            _invaders.Add(new Invader(InvaderType.Bag, 10));
        }

        public void FireShot()
        {
            if (_playerShot.Count() < MaximumPlayerShots)
            {
                _playerShot.Add(new Shot(_player.Location, Direction.Up));
                ShotMoved("_playerShot");
            }
        }

        public void MovePlayer(Direction direction)
        {
            if (PlayerDying == null)
            {
                return;
            }
            else
            {
                _player.Move(direction);
                ShipChanged(_player, false);
            }
        }

        public void Twinkle()
        {
            Point starToRemove = _stars.ToList()[_random.Next(_stars.Count)];
            if (_random.Next(2) == 1 && _stars.Count < 1.5M * _stars.Count() && _stars.Count() > .15M * _stars.Count())
            {
                _stars.Add(new Point(_random.Next(400), _random.Next(300)));
                //StarChanged(starToRemove, false);
            }
            else
            {
                if (_stars.Count > _stars.Count * .15M)
                {
                    _stars.Remove(starToRemove);
                    StarChanged(starToRemove, true);
                }
            }
        }



        public event EventHandler<StarChangedEventArgs> StarChanged;
        private void OnStarChanged(Point starThatChanged, bool dissapeard)
        {
            EventHandler<StarChangedEventArgs> starChanged = StarChanged;
            if (starChanged != null)
            {
                starChanged(this, new StarChangedEventArgs(starThatChanged, dissapeard));
            }
        }

        public event EventHandler<ShipChangedEventArgs> ShipChanged;
        private void OnShipChanged(Ship shipThatChanged, bool killed)
        {
            EventHandler<ShipChangedEventArgs> shipChanged = ShipChanged;
            if (shipChanged != null)
            {
                shipChanged(this, new ShipChangedEventArgs(shipThatChanged, killed));
            }
        }

        public event EventHandler<ShotMovedEventArgs> ShotChanged;
        private void OnShotChanged(Shot shotThatMoved, bool dissapeard)
        {
            EventHandler<ShotMovedEventArgs> shotChanged = ShotChanged;
            if (shotChanged != null)
            {
                shotChanged(this, new ShotMovedEventArgs(shotThatMoved, dissapeard));
            }
        }
    }
}
