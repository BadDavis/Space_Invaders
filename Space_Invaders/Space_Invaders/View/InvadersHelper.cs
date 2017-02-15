using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Invaders.Model;
using Windows.UI.Xaml;

namespace Space_Invaders.View
{
    static class InvadersHelper
    {
        internal static FrameworkElement ScanLineFactory(int y, int v, double scale)
        {
            throw new NotImplementedException();
        }

        internal static FrameworkElement InvaderControlFactory(Invader invader, double scale)
        {
            throw new NotImplementedException();
        }

        internal static FrameworkElement PlayerControlFactory(Player player, double scale)
        {
            throw new NotImplementedException();
        }

        internal static void MoveElement(FrameworkElement invaderControl, double x, double scale, double v)
        {
            throw new NotImplementedException();
        }

        internal static void ResizeElement(FrameworkElement invaderControl, double v1, double v2)
        {
            throw new NotImplementedException();
        }
    }
}
