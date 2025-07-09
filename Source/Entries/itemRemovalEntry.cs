using System.Collections.Generic;
using System.Windows.Media;

namespace RandomItemGiverUpdater
{
    public class itemRemovalEntry
    {
        public List<lootTable> lootTableCheckList = new List<lootTable>();
        public List<lootTable> lootTables = new List<lootTable>();
        public string lootTableWhiteList = "";
        public bool allLootTablesChecked = true;
        public string itemName { get; set; }
        public string lootTablesString { get; set; }


        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //-- Constructor --//

        public itemRemovalEntry(string name)
        {
            //Set attributes
            itemName = name;
        }


        public void UpdateLootTables(lootTable newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables.Add(newLootTable);
            if (string.IsNullOrEmpty(lootTablesString))
            {
                lootTablesString = string.Format("{0}", newLootTable.fullLootTablePath.Replace(RIGU.wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }
            else
            {
                lootTablesString = string.Format("{0}, {1}", lootTablesString, newLootTable.fullLootTablePath.Replace(RIGU.wndMain.currentDatapack, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }

            //Set loot table checklist
            lootTableCheckList.Clear();
            foreach (lootTable lootTable in lootTables)
            {
                //Check if the loot table is already on the list
                bool isAdded = false;
                foreach (lootTable lootTableCheck in lootTableCheckList)
                {
                    if (lootTableCheck.fullLootTablePath == lootTable.fullLootTablePath)
                    {
                        isAdded = true;
                    }
                }

                if (isAdded == false)
                {
                    lootTableCheckList.Add(new lootTable(lootTable.lootTableName, lootTable.lootTableType, lootTable.lootTablePath));
                    lootTableWhiteList = string.Format("{0}{1}", lootTableWhiteList, lootTable.fullLootTablePath);
                }
            }
        }
    }
}
