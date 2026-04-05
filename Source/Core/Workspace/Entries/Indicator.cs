using RandomItemGiverUpdater.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{
    public struct Indicator
    {
        public static Indicator none = new Indicator("", new SolidColorBrush(Colors.Gray));
        public static Indicator modified = new Indicator("#", new SolidColorBrush(Colors.LightBlue));
        public static Indicator deleted = new Indicator("X", new SolidColorBrush(Colors.Red));

        public string text { get; set; }
        public SolidColorBrush color { get; set; }

        private Indicator(string text, SolidColorBrush color)
        {
            this.text = text;
            this.color = color;
        }
    }
}
