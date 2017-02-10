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


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
