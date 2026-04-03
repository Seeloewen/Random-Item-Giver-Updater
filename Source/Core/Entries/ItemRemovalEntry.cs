using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Core.Entries
{
    public class ItemRemovalEntry
    {
        public string lootTables { get; set; }
        public SolidColorBrush canvasBackColor { get; set; }

        //TODO: Refactor
        public List<LootTable> lootTableWhiteList = new List<LootTable>();
        public List<LootTable> lootTableCheckList = new List<LootTable>();

        private int index;
        public string name;

        public ItemRemovalEntry(string id, int index)
        {
            this.index = index;
            canvasBackColor = SetBackColor();

            foreach (LootTable lootTable in RIGU.core.currentDatapack.GetLootTables())
            {
                lootTableCheckList.Add(new LootTable(lootTable.name, lootTable.category, lootTable.path));
            }
        }

        public void UpdateLootTables(LootTable lootTable)
        {
            lootTableCheckList.Add(lootTable);
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            return new SolidColorBrush(Convert.ToInt32(index) % 2 == 0 
                                                              ? Color.FromArgb(100, 70, 70, 70) 
                                                              : Color.FromArgb(100, 90, 90, 90));
        }
    }
}
