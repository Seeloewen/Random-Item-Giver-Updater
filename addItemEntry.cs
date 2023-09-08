using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Random_Item_Giver_Updater
{
    public class addItemEntry
    {
        //Attributes
        public string itemName { get; set; }
        public int itemIndex { get; set; }
        public string itemNBT { get; set; }
        public string itemPrefix { get; set; }
        public addToLootTableEntry lootTableEntry;

        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //-- Constructor --//

        public addItemEntry(string prefix, string name, int index)
        {
            //Set variables
            itemName = name.TrimEnd('\r', '\n');
            itemIndex = index;
            itemPrefix = prefix;

            //Set the backcolor
            canvasBackColor = SetBackColor();
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
