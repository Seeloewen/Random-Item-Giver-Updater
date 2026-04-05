using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Core.Util;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{

    public class MainEntry : ObservableObject
    {
        public Item item { get; set; }
        public SolidColorBrush canvasBackColor { get; set; }
        public string index { get; set; }
        public Indicator indicator { get; set; }

        public MainEntry(Item item, int index)
        {
            this.item = item;
            this.index = index.ToString();
            canvasBackColor = SetBackColor();
        }

        public SolidColorBrush SetBackColor() => new SolidColorBrush(int.Parse(index) % 2 == 0
                                                                        ? Color.FromArgb(100, 70, 70, 70)
                                                                        : Color.FromArgb(100, 90, 90, 90));

        public void UpdateIndicator()
        {
            //Update indicator accordingly
            if (item.isDeleted)
            {
                indicator = Indicator.deleted;
            }
            else if (item.IsModified())
            {
                indicator = Indicator.modified;
            }
            else
            {
                indicator = Indicator.none;
            }

            NotifyPropertyChanged(nameof(indicator));
        }
    }
}
