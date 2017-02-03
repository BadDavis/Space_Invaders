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
        public const int MaxLives = 4;
        public const int InitialStarCount = 50; // gwiazdy w tle

        private readonly Random _random = new Random();

        public int Score { get; private set; }
        public int Wave { get; private set; } //fala wrogow
        public int Lives { get; private set; }

        public bool GameOver { get; private set; }

        private DateTime? _playerDied = null;
        public bool PlayerDying { get { return _playerDied.HasValue; } } //gdy ginie gracz miga jego statek i reszta stoi

        private Player _player;
        private Invader _invader;

        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShot = new List<Shot>();
        private readonly List<Shot> _invaderShot = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();

        private readonly Dictionary<InvaderType, int> _scoresForInvader = new Dictionary<InvaderType, int>()
        {
            {InvaderType.Bag, 10 },
            {InvaderType.Satellite, 20 },
            {InvaderType.Saucer, 30 },
            {InvaderType.Spaceship, 40 },
            {InvaderType.Star, 50 },
        };

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
            GameOver = false; // poczatek gry

            foreach (Shot shot in _playerShot)
            {
                OnShotChanged(shot, true);
            }
            _playerShot.Clear();

            foreach (Shot shot in _invaderShot)
            {
                OnShotChanged(shot, true);
            }
            _invaderShot.Clear();

            foreach (Invader ship in _invaders)
            {
                OnShipChanged(ship as Ship, true);
            }
            _invaders.Clear();

            foreach (Point star in _stars)
            {
                OnStarChanged(star, true);
            }
            _stars.Clear();

            for (int i = 0; i < InitialStarCount; i++)
            {
                AddStar();
            }

            _player = new Player();
            OnShipChanged(_player, false);

            Wave = 0;
            Score = 0;
            Lives = 4;

            NextWave();
        }

        public void PlayerShot()//gracz strzela
        {
            if (_playerShot.Count() < MaximumPlayerShots)
            {
                _playerShot.Add(new Shot(_player.Location, Direction.Up));
                ShotMoved(_player, false);
            }
        }

        private void InvaderFire()
        {
            if (_invaderShot.Count < 2)
            {
                _invaderShot.Add(new Shot(_invader.Location, Direction.Down));
                ShotChanged(_invader., false);
            }
        }

        private void NextWave()
        {
            Wave++;
            _invaders.Clear();
            InvaderType invader;


            for (int i = 0; i < 66; i++)
            {
                //_invaders.Add()  tworzenie nowej fali
            }

            _invaderDirection = Direction.Left;

            for (int invaderRow = 0; invaderRow < 5; invaderRow++)
            {
                switch (invaderRow)
                {
                    case 0:
                        invader = InvaderType.Bag;
                        break;

                    case 1:
                        invader = InvaderType.Satellite;
                        break;

                    case 2:
                        invader = InvaderType.Saucer;
                        break;

                    case 3:
                        invader = InvaderType.Spaceship;
                        break;

                    case 4:
                        invader = InvaderType.Star;
                        break;
                    default:
                        invader = InvaderType.Star;
                        break;
                }

                for (int column = 0; column < 11; column++)
                {
                    Point location = new Point(column * Invader.InvaderSize.Width * 1.4,
                        invaderRow * Invader.InvaderSize.Height * 1.4);
                    _invaders.Add(new Invader(invader, location, _scoresForInvader[invader]));
                    OnShipChanged(_invaders[_invaders.Count - 1], false);
                }
            }
        }


        public void MovePlayer(Direction direction)// ruch gracza
        {
            if (PlayerDying == null)//jesli umarł to nic się nie dzieje
            {
                return;
            }
            else
            {
                _player.Move(direction);
                ShipChanged(_player, false);//zdarzenie zmiany polozenia statku
            }
        }

        private void MoveInvader()
        {

        }

        public void Twinkle()//dodawanie i usuwanie gwiazd
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

        public void Update(bool paused)
        {
            if (!paused)//do zmiany
            {
                if (_invaders.Count == 0)
                {
                    NextWave();

                    if (!PlayerDying)
                    {
                        MoveInvader();
                        MoveShots();
                        InvaderFire();
                        CheckForInvaderCollisions();
                        CheckForMotherShipCollisions();
                        CheckForPlayerCollisions();
                    }
                }
                if (PlayerDying && TimeSpan.FromSeconds(2.5) < DateTime.Now - _playerDied)
                {
                    _playerDied = null;
                    OnShipChanged(_player, false);
                }
            }
            Twinkle();

        }

        private void CheckForPlayerCollisions()
        {
            throw new NotImplementedException();
        }

        private void CheckForInvaderCollisions()
        {
            throw new NotImplementedException();
        }

        private void MoveShots()
        {
            throw new NotImplementedException();
        }


        //eventy
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
