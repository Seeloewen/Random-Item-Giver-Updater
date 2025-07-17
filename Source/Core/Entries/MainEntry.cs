using RandomItemGiverUpdater.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RandomItemGiverUpdater
{
    public class MainEntry : Item
    {
        public SolidColorBrush canvasBackColor { get; set; }
        public string index { get; set; }
        public bool isDeleted = false;

        public MainEntry(string itemBody, int index) : base(itemBody)
        {
            this.index = index.ToString();
            canvasBackColor = SetBackColor();
        }

        public SolidColorBrush SetBackColor() => new SolidColorBrush(int.Parse(index) % 2 == 0
                                                                        ? Color.FromArgb(100, 70, 70, 70)
                                                                        : Color.FromArgb(100, 90, 90, 90));

        public void SetIndicatorState(Button button, ModificationState state)
        {
            if (button != null)
            {
                //Get the parent canvas and textblock
                Canvas canvas = SeeloewenLib.Tools.FindVisualParent<Canvas>(button);
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;

                //Update indicator accordingly
                switch(state)
                {
                    case ModificationState.Deleted:
                        textblock.Visibility = Visibility.Visible;
                        textblock.Text = "X";
                        textblock.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case ModificationState.Edited:
                        textblock.Visibility = Visibility.Visible;
                        textblock.Text = "#";
                        textblock.Foreground = new SolidColorBrush(Colors.LightBlue);
                        break;
                    default:
                        textblock.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        public ModificationState GetModificationState()
        {
            if (isDeleted)
            {
                return ModificationState.Deleted;
            }
            else if (IsModified())
            {
                return ModificationState.Edited;
            }
            else
            {
                return ModificationState.Unchanged;
            }
        }
    }
}
