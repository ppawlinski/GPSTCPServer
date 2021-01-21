using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace GPSTCPClient
{
    public static class Tools
    {
        public static ToolTip CreateTootip(string content)
        {
            ToolTip tt = new ToolTip();
            tt.Background = new SolidColorBrush(Color.FromRgb(0x42, 0x42, 0x42));
            tt.Foreground = new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xf3));
            tt.Content = content;
            return tt;
        }

        public static PackIcon CreateIcon(PackIconKind kind)
        {
            PackIcon pi = new PackIcon();
            pi.Kind = kind;
            return pi;
        }
    }
}
