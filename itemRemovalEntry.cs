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
    public class itemRemovalEntry
    {
        //Attributes
        public List<lootTable> lootTableCheckList = new List<lootTable>();
        public List<lootTable> lootTables = new List<lootTable>();
        public string lootTableWhiteList = "";
        public bool allLootTablesChecked = true;
        public string itemName;
        public string itemNBT;
        public string fullItemName { get; set; }
        public string lootTablesString { get; set; }


        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //Reference to main window
        public MainWindow wndMain = (MainWindow)Application.Current.MainWindow;

        //-- Constructor --//

        public itemRemovalEntry(string name, string nbt)
        {
            //Set attributes
            itemName = name;
            itemNBT = nbt;
            if(string.IsNullOrEmpty(itemNBT))
            {
                fullItemName = itemName;
            }
            else
            {
                fullItemName = string.Format("{0} (NBT: {1})", name, nbt);
            }
        }

        public void UpdateLootTables(lootTable newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables.Add(newLootTable);
            if (string.IsNullOrEmpty(lootTablesString))
            {
                lootTablesString = string.Format("{0}", newLootTable.fullLootTablePath.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }
            else
            {
                lootTablesString = string.Format("{0}, {1}", lootTablesString, newLootTable.fullLootTablePath.Replace(wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }

            //Set loot table checklist
            lootTableCheckList.Clear();
            foreach (lootTable lootTable in lootTables)
            {
                lootTableCheckList.Add(new lootTable(lootTable.lootTableName, lootTable.lootTableType, lootTable.lootTablePath));            }
        }
    }
}
