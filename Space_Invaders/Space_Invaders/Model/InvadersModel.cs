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

            if (GameOver || PlayerDying || _lastUpdated == DateTime.MinValue)
            {
                return;
            }
            if (_playerShot.Count() < MaximumPlayerShots)
            {
                Shot playerShot = new Shot(new Point(_player.Location.X + (_player.Size.Width / 2) - 1, _player.Location.Y),
                    Direction.Up);
                _playerShot.Add(playerShot);
                OnShotChanged(playerShot, false);
            }
        }

        private void InvaderFire()
        {
            if (_invaderShot.Count() > Wave + 1 || _random.Next(10) < 10 - Wave) //losowy obiekt strzela
            {
                return;
            }

            var invaderColumn = from invader in _invaders
                                group invader by invader.Location.X
                                into invaderGroup
                                orderby invaderGroup.Key descending
                                select invaderGroup;

            var randomGroup = invaderColumn.ElementAt(_random.Next(invaderColumn.Count()));
            var shoot = randomGroup.Last();

            Point shotLocation = new Point(shoot.Area.X + (shoot.Size.Width / 2) - 1, shoot.Area.Bottom);
            Shot shootingInvader = new Shot(shotLocation, Direction.Down);
            _invaderShot.Add(shootingInvader);

            OnShotChanged(shootingInvader, false);
        }

        private void NextWave()
        {
            Wave++;
            _invaders.Clear();
            InvaderType invader;

            _invaderDirection = Direction.Left;

            for (int invaderRow = 0; invaderRow < 6; invaderRow++)
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
            if (_playerDied.HasValue)//jesli umarł to nic się nie dzieje
            {
                return;
            }
            else
            {
                _player.Move(direction);
                OnShipChanged(_player, false);//zdarzenie zmiany polozenia statku
            }
        }

        private void MoveInvader()
        {
            TimeSpan lastMoveTime = DateTime.Now - _lastUpdated;
            double timeBetweenMove = Math.Max(7 - Wave, 1) * (2 * _invaders.Count());

            if (lastMoveTime >= TimeSpan.FromMilliseconds(timeBetweenMove))
            {
                _lastUpdated = DateTime.Now;

                if (_invaderDirection == Direction.Right)
                {
                    var invadersReachedRight = from invader in _invaders
                                               where invader.Area.Right > PlayAreaSize.Width - Invader.HorizontalPixelsPerMove * 2
                                               select invader;

                    if (invadersReachedRight.Count() > 0)
                    {
                        _invaderDirection = Direction.Down;
                        foreach (Invader invader in _invaders)
                        {
                            invader.Move(_invaderDirection);
                            OnShipChanged(invader, false);
                        }

                        _justMovedDown = true;
                        _invaderDirection = Direction.Left;
                    }
                    else
                    {
                        _justMovedDown = false;
                        foreach (Invader invader in _invaders)
                        {
                            invader.Move(_invaderDirection);
                            OnShipChanged(invader, false);
                        }
                    }
                }
                else
                {
                    if (_invaderDirection == Direction.Left)
                    {
                        var invaderReachedLeft = from invader in _invaders
                                                 where invader.Area.Left > Invader.HorizontalPixelsPerMove * 2
                                                 select invader;

                        if (invaderReachedLeft.Count() > 0)
                        {
                            _invaderDirection = Direction.Down;
                            foreach (Invader invader in _invaders)
                            {
                                invader.Move(_invaderDirection);
                                OnShipChanged(invader, false);
                            }

                            _justMovedDown = true;
                            _invaderDirection = Direction.Right;
                        }
                        else
                        {
                            _justMovedDown = false;
                            foreach (Invader invader in _invaders)
                            {
                                invader.Move(_invaderDirection);
                                OnShipChanged(invader, false);
                            }
                        }
                    }
                }

                ///cos pozniej
            }
        }

        public void Twinkle()//dodawanie i usuwanie gwiazd
        {
            Point starToRemove = _stars.ToList()[_random.Next(_stars.Count)]; // ///////////////////moze nie dzialac
            if (_random.Next(2) == 1 && _stars.Count < 1.5M * _stars.Count() && _stars.Count() > .15M * _stars.Count())
            {
                _stars.Add(new Point(_random.Next(400), _random.Next(300)));
                OnStarChanged(starToRemove, false);
            }
            else
            {
                if (_stars.Count > _stars.Count * .15M)
                {
                    _stars.Remove(starToRemove);
                    OnStarChanged(starToRemove, true);
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
            List<Shot> invaderShots = _invaderShot.ToList();

            foreach (Shot shot in invaderShots)
            {
                Rect shotRect = new Rect(shot.Location.X, shot.Location.Y, Shot.ShotSize.Width, Shot.ShotSize.Height);

                if (RectsOverLap(_player.Area, shotRect))
                {
                    if (Lives == 0)
                    {
                        EndGame();
                    }
                    else
                    {
                        _invaderShot.Remove(shot);
                        OnShotChanged(shot, true);
                        _playerDied = DateTime.Now;
                        OnShipChanged(_player, true);
                        RemoveShots();
                        Lives--;
                    }
                }
            }
            if (invadersReachBottom.Count() > 0)
            {
                EndGame();
            }
        }

        private void RemoveShots()
        {
            List<Shot> invaderShot = _invaderShot.ToList();
            List<Shot> playershot = _playerShot.ToList();

            foreach (Shot shots in invaderShot)
            {
                OnShotChanged(shots, true);
            }

            foreach (Shot shots in playershot)
            {
                OnShotChanged(shots, true);
            }

            _invaderShot.Clear();
            _playerShot.Clear();
        }

        private static bool RectsOverlap(Rect r1, Rect r2)
        {
            r1.Intersect(r2);
            if (r1.Width > 0 || r2.Height > 0)
            {
                return true;
            }
            return false;
        }

        private void CheckForInvaderCollisions()
        {
            List<Shot> playersShot = _playerShot.ToList();
            List<Invader> invaders = _invaders.ToList();

            foreach (Shot shot in playersShot)
            {
                Rect shotRect = new Rect(shot.Location.X, shot.Location.Y, Shot.ShotSize.Width,
                                         Shot.ShotSize.Height);

                var invaderHit = from invader in invaders
                                 where RectsOverlap(invader.Area, shotRect)
                                 select invader;

                foreach (Invader deathInvader in invaderHit)
                {
                    _invaders.Remove(deathInvader);
                    OnShipChanged(deathInvader, true);
                    _playerShot.Remove(shot);
                    OnShotChanged(shot, true);
                    Score += deathInvader.Score;
                }
            }
        }

        private void MoveShots()
        {
            throw new NotImplementedException();
        }






        ////////////////////eventy
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
