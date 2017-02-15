using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Foundation;
using DispatcherTimer = Windows.UI.Xaml.DispatcherTimer;
using FrameworkElement = Windows.UI.Xaml.FrameworkElement;

namespace Space_Invaders.ViewModel
{
    using View;
    using Model;

    class InvadersViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
        public INotifyPropertyChanged Sprites { get { return _sprites; } }
        public bool GameOver { get { return _model.GameOver; } }
        private readonly ObservableCollection<object> _lives = new ObservableCollection<object>();
        public INotifyCollectionChanged Lives { get { return _lives; } }
        public bool Paused { get; set; }
        private bool _lastPaused = true;
        
        public static double Scale { get; private set; }
        public int Score { get; private set; }
        public Size PlayAreaSize
        {
            set
            {
                Scale = value.Width / 405;
                _model.UpdateAllShipsAndStars();
                RecreateScanLines();
            }
        }

        private readonly InvadersModel _model = new InvadersModel();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private FrameworkElement _playControl = null;
        private bool _playerFlashing = false;
        private readonly Dictionary<Invader, FrameworkElement> _invaders = new Dictionary<Invader, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders = new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<Shot, FrameworkElement> _shots = new Dictionary<Shot, FrameworkElement>();
        private readonly Dictionary<Point, FrameworkElement> _stars = new Dictionary<Point, FrameworkElement>();
        private readonly List<FrameworkElement> _scanLines = new List<FrameworkElement>();

        private DateTime? _leftAction = null;
        private DateTime? _righrAction = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvadersViewModel()
        {
            Scale = 1;

            _model.ShipChanged += ModelShipChangedEventHandler;
            _model.ShotChanged += ModelShotMovedEventHandler;
            _model.StarChanged += ModelStarChangedEventHandler;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerTickEventhandler;

            EndGame();
        }



        public void StartGame()
        {
            Paused = false;
            foreach (var invader in _invaders.Values)
            {
                _sprites.Remove(invader);
            }

            foreach (var shot in _shots.Values)
            {
                _sprites.Remove(shot);
            }

            _model.StartGame();
            OnPropertyChanged("GameOver");
            _timer.Start();
        }

        private void EndGame()
        {
            throw new NotImplementedException();
        }

        private void RecreateScanLines()
        {
            foreach (FrameworkElement scanLines in _scanLines)
            {
                if (_sprites.Contains(scanLines))
                {
                    _sprites.Remove(scanLines);
                }
            }
            _scanLines.Clear();
            for (int y = 0; y < 300; y+= 2)
            {
                FrameworkElement scanLine = InvadersHelper.ScanLineFactory(y, 400, Scale);
                _scanLines.Add(scanLine);
                _sprites.Add(scanLine);
            }
        }

        private void OnPropertyChanged(string v)
        {
            throw new NotImplementedException();
        }

        internal void KeyDown(Windows.System.VirtualKey virtualKey)
        {
            if (virtualKey == Windows.System.VirtualKey.Space)
            {
                _model.PlayerShot();
            }
            if (virtualKey == Windows.System.VirtualKey.Left)
            {
                _leftAction = DateTime.Now;
            }
            if (virtualKey == Windows.System.VirtualKey.Right)
            {
                _righrAction = DateTime.Now;
            }
        }
        
        internal void KeyUp(Windows.System.VirtualKey virtualKey)
        {
            if (virtualKey == Windows.System.VirtualKey.Left)
            {
                _leftAction = null;
            }
            if (virtualKey == Windows.System.VirtualKey.Right)
            {
                _righrAction = null;
            }
        }

        internal void LeftGestureStarded()
        {
            _leftAction = DateTime.Now;
        }
        internal void LeftGestureCompleted()
        {
            _leftAction = null;
        }

        internal void RightGestureStarted()
        {
            _righrAction = DateTime.Now;
        }

        internal void RightGestureCompleted()
        {
            _righrAction = null;
        }

        internal void Tapped()
        {
            _model.PlayerShot();
        }



        private void ModelShipChangedEventHandler(object sender, ShipChangedEventArgs e)
        {
            if (!e.Killed)
            {
                if (e.ShipUpdated is Invader)
                {
                    Invader invader = e.ShipUpdated as Invader;
                    if (!_invaders.ContainsKey(invader))
                    {
                        FrameworkElement invaderControl = InvadersHelper.InvaderControlFactory(invader, Scale);
                        _invaders[invader] = invaderControl;
                        _sprites.Add(invaderControl);
                    }
                    else
                    {
                        FrameworkElement invaderControl = _invaders[invader];
                        InvadersHelper.MoveElement(invaderControl, invader.Location.X, Scale, invader.Location.Y * Scale);
                        InvadersHelper.ResizeElement(invaderControl, invader.Size.Width * Scale, invader.Size.Height * Scale);
                    }
                }
                else if (e.ShipUpdated is Player)
                {
                    if (_playerFlashing)
                    {
                        AnimatedImage playerImage = _playControl as AnimatedImage;
                        _playerFlashing = false;

                    }
                    if (_playControl == null)
                    {

                        FrameworkElement playerControl = InvadersHelper.PlayerControlFactory(player, Scale);
                        _sprites.Add(playerControl);
                    }
                }
            }
        }

        private void ModelShotMovedEventHandler(object sender, ShotMovedEventArgs e)
        {
            if (!e.Dissapeared)
            {
                if (e.)
                {

                }
            }
        }

        private void TimerTickEventhandler(object sender, object e)
        {
            if (_lastPaused != Paused)
            {
                _lastPaused = Paused;
                OnPropertyChanged("Paused");
            }
            if (!Paused)
            {
                if (_leftAction.HasValue && _righrAction.HasValue)// jesli jednoczesnie zrobil 2 akcje wybierz pozniejsze zdarzenie
                {
                    if (DateTime.Compare((DateTime)_leftAction, (DateTime)_righrAction) > 0)//jesli akcja 'w prawo' byla wczesniej idz w lewo
                    {
                        _model.MovePlayer(Direction.Left);
                    }
                    else // jesli pozniej w prawo
                    {
                        _model.MovePlayer(Direction.Right);
                    }
                }
                if (_leftAction.HasValue)//ruch w lewo
                {
                    _model.MovePlayer(Direction.Left);
                }
                if (_righrAction.HasValue)//ruch w prawo
                {
                    _model.MovePlayer(Direction.Right);
                }
            }

            _model.Update(Paused);

            if (Score != _model.Score)//sprawdzamy score
            {
                Score = _model.Score;
                OnPropertyChanged("Score");
            }

            if (_model.Lives >= 0)// aktualizacja _lives gracza
            {
                while (_model.Lives > _lives.Count)
                {
                    _lives.Add(new object());
                }
                while (_model.Lives < _lives.Count)
                {
                    _lives.RemoveAt(0);
                }
            }

            foreach (FrameworkElement control in _shotInvaders.Keys.ToList())//jesli invader zostal zniszczony, to po animacji niszczenia(0.5s) jest usuwany po 0.5s
            {
                if (DateTime.Now - _shotInvaders[control] > TimeSpan.FromSeconds(0.5))
                {
                    _sprites.Remove(control);
                    _shotInvaders.Remove(control);
                }
            }

            if (_model.GameOver)//jesli gra sie skonczyla zatrzymujemy stoper i zglaszamy GameOver w viewModel
            {
                _timer.Stop();
                OnPropertyChanged("GameOver");
            }
        }

    }
}
