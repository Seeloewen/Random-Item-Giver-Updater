using System.Collections.Generic;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Core.Entries
{
    public class ItemRemovalEntry : Item
    {
        public List<LootTable> lootTableCheckList = new List<LootTable>();
        public List<LootTable> lootTables = new List<LootTable>();
        public string lootTableWhiteList = "";
        public bool allLootTablesChecked = true;
        public string lootTablesString { get; set; }

        public SolidColorBrush canvasBackColor { get; set; }

        public ItemRemovalEntry(string name) : base(name, name)
        {
            SetName(name);
        }


        public void UpdateLootTables(LootTable newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables.Add(newLootTable);
            if (string.IsNullOrEmpty(lootTablesString))
            {
                lootTablesString = string.Format("{0}", newLootTable.path.Replace(RIGU.core.currentDatapack.directory, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }
            else
            {
                lootTablesString = string.Format("{0}, {1}", lootTablesString, newLootTable.path.Replace(RIGU.core.currentDatapack.directory, "").Replace("/data/randomitemgiver/loot_tables/", ""));
            }

            //Set loot table checklist
            lootTableCheckList.Clear();
            foreach (LootTable lootTable in lootTables)
            {
                //Check if the loot table is already on the list
                bool isAdded = false;
                foreach (LootTable lootTableCheck in lootTableCheckList)
                {
                    if (lootTableCheck.path == lootTable.path)
                    {
                        isAdded = true;
                    }
                }

                if (isAdded == false)
                {
                    lootTableCheckList.Add(new LootTable(lootTable.name, lootTable.category, lootTable.path));
                    lootTableWhiteList = string.Format("{0}{1}", lootTableWhiteList, lootTable.path);
                }
            }
        }
    }
}
