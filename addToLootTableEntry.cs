using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Random_Item_Giver_Updater
{
    public class addToLootTableEntry
    {
        //Attributes
        public List<lootTable> lootTableCheckList = new List<lootTable>();
        public string lootTableWhiteList = "";
        public bool allLootTablesChecked = true;
        public string itemName { get; set; }
        public int index;

        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //Reference to main window
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //-- Constructor --//

        public addToLootTableEntry(string name, int index)
        {
            //Set attributes
            itemName = name;
            this.index = index;

            //Set loot table checklist
            foreach (lootTable lootTable in wndMain.lootTableList)
            {
                lootTableCheckList.Add(new lootTable(lootTable.lootTableName, lootTable.lootTableType, lootTable.lootTablePath));
            }

            //Set backcolor
            canvasBackColor = SetBackColor();
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            if (index % 2 == 0)
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
