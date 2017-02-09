using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Invaders.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePlayAreaSize(new Size(e.NewSize.Width, e.NewSize.Height - 160));
        }

        private void pageRoot_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {//zglasza przesuniece palca po ekranie, e.Delta informuje o jaki dystans przesunieto palec w ktorym zgloszonoe zdarzenie
            if (e.Delta.Translation.X < -1)
            {
                viewModel.LeftGestureStarted();
            }
            else
            {
                if (e.Delta.Translation.Y > 1)
                {
                    viewModel.RightGestureStarded();
                }
            }
        }

        private void pageRoot_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {//zglasza oderwanie palca od ekranu
            viewModel.LeftGestureCompleted();
            viewModel.RightGestureComplete();
        }

        private void pageRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            viewModel.Tapped();
        }

        private void playArea_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayAreaSize(playArea.RenderSize);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += KeyDownHandler;
            Window.Current.CoreWindow.KeyUp += KeyUpHandler;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= KeyDownHandler;
            Window.Current.CoreWindow.KeyUp -= KeyUpHandler;
            base.OnNavigatedFrom(e);
        }

        private void KeyUpHandler(CoreWindow sender, KeyEventArgs e)
        {
            viewModel.KeyUp(e.VirtualKey);
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            viewModel.KeyDown(e.VirtualKey);
        }

        private void UpdatePlayAreaSize(Size renderSize)
        {
            double targetWidth;
            double targetHeight;
            if (renderSize.Width > renderSize.Height)
            {
                targetWidth = renderSize.Height * 4 / 3;
                targetHeight = renderSize.Height;
                double leftRightMargin = (renderSize.Width - targetWidth) / 2;
                playArea.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
            }
            else
            {
                targetHeight = renderSize.Width * 3 / 4;
                targetWidth = renderSize.Width;
                double topBottomMargin = (renderSize.Height - targetHeight) / 2;
                playArea.Margin = new Thickness(0, topBottomMargin, 0, topBottomMargin);
            }
            playArea.Width = targetWidth;
            playArea.Height = targetHeight;
            viewModel.PlayAreaSize = playArea.RenderSize;
        }
    }
}
