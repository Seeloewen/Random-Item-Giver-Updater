using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using SeeloewenLib;

namespace Random_Item_Giver_Updater
{
    public class itemEntry
    {
        //Item attributes
        public string itemName { get; set; }
        public string itemNBT { get; set; }
        public string itemIndex { get; set; }
        public string newName;
        public string newNBT;
        public bool isModified = false;
        public bool isDeleted = false;

        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //Important references
        SeeloewenLibTools SeeloewenLibTools = new SeeloewenLibTools();

        //-- Constructor --//

        public itemEntry(string name, string nbt, int index)
        {

            //Initialize variables
            itemName = name.TrimEnd('\r', '\n');
            itemNBT = nbt.TrimEnd('\r', '\n'); ;
            itemIndex = (index + 1).ToString();
            newName = itemName;
            newNBT = itemNBT;

            //Set the backcolor
            canvasBackColor = SetBackColor();
        }

        //-- Custom Methods --//

        public void SetIndicatorState(object sender)
        {
            if (sender is Button button)
            {
                //Get the parent canvas
                Canvas canvas = SeeloewenLibTools.FindVisualParent<Canvas>(button);

                //Get the necessary controls
                TextBlock textblock = canvas.FindName("tblIndicator") as TextBlock;
                if (isModified == true && isDeleted == true)
                {
                    //Item deleted, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "X";
                    textblock.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (isModified == true && isDeleted == false)
                {
                    //Item modified, show indicator
                    textblock.Visibility = Visibility.Visible;
                    textblock.Text = "#";
                    textblock.Foreground = new SolidColorBrush(Colors.LightBlue);

                }
                else if (isModified == false && isDeleted == false)
                {
                    //No changes, hide indicator
                    textblock.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Invalid state provided, hide indicator
                    textblock.Visibility = Visibility.Hidden;
                }
            }
          
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            if (Convert.ToInt32(itemIndex) % 2 == 0)
            {
                return new SolidColorBrush(Color.FromArgb(100, 70, 70, 70));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(100, 90, 90, 90));
            }
        }
    }
}
