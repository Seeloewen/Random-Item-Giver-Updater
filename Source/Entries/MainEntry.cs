using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RandomItemGiverUpdater
{
    public class MainEntry : Item
    {
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        public SolidColorBrush canvasBackColor { get; set; }
        public string index { get; set; }

        public bool isDeleted = false;
        private string originalBody;

        public MainEntry(string itemBody, int index) : base(itemBody)
        {
            this.index = index.ToString();
            canvasBackColor = SetBackColor();
            originalBody = GetItemBody();
        }

        public SolidColorBrush SetBackColor() => new SolidColorBrush(int.Parse(index) % 2 == 0
                                                                        ? Color.FromArgb(100, 70, 70, 70)
                                                                        : Color.FromArgb(100, 90, 90, 90));

        public void SetIndicatorState(Button button, ModificationState state)
        {
            if (button != null)
            {
                //Get the parent canvas and textblock
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;

                if (state == ModificationState.Deleted)
                {
                    //Item deleted, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "X";
                    textblock.Foreground = new SolidColorBrush(Colors.Red);
                }

                else if (state == ModificationState.Edited)
                {
                    //Item modified, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "#";
                    textblock.Foreground = new SolidColorBrush(Colors.LightBlue);
                }
                else
                {
                    //No changes, hide indicator
                    textblock.Visibility = Visibility.Hidden;
                }
            }
        }

        public ModificationState GetModificationState()
        {
            if (isDeleted)
            {
                return ModificationState.Deleted;
            }
            else if (originalBody != GetItemBody())
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
