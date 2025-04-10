using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Random_Item_Giver_Updater
{
    public class addItemEntry
    {
        //Attributes
        public string itemName { get; set; }
        public int itemIndex { get; set; }
        public string itemNBT { get; set; }
        public string itemPrefix { get; set; }

        public string itemStackComponent;

        public List<lootTable> lootTableCheckList = new List<lootTable>();
        public string lootTableWhiteList = "";
        public bool allLootTablesChecked = true;


        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //-- Constructor --//

        public addItemEntry(string prefix, string name, int index)
        {
            //Set variables
            itemName = name.TrimEnd('\r', '\n');
            itemIndex = index;
            itemPrefix = prefix;

            //Set loot table checklist
            foreach (lootTable lootTable in RIGU.wndMain.lootTableList)
            {
                lootTableCheckList.Add(new lootTable(lootTable.lootTableName, lootTable.lootTableType, lootTable.lootTablePath));
            }

            //Set the backcolor
            canvasBackColor = SetBackColor();
        }

        public bool HasLegacyNBT()
        {
            return !string.IsNullOrEmpty(itemNBT);
        }
        public bool HasItemStackComponent()
        {
            return !string.IsNullOrEmpty(itemStackComponent);
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
