using RandomItemGiverUpdater.Core.Data;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{
    public class RemovalEntry
    {
        public string lootTables { get; set; }
        public SolidColorBrush canvasBackColor { get; set; }

        //TODO: Refactor
        public List<LootTable> lootTableWhiteList = new List<LootTable>();
        public List<LootTable> lootTableCheckList = new List<LootTable>();

        public string lootTableCheckListStr { get; set; } //Used for wpf databind

        private int index;
        public string name { get; set; }

        public RemovalEntry(string name, int index)
        {
            this.index = index;
            this.name = name;
            canvasBackColor = SetBackColor();

            lootTableCheckList.AddRange(Datapack.Get().GetLootTables());
        }

        public void UpdateLootTables(LootTable lootTable)
        {
            lootTableCheckList.Add(lootTable);

            if (string.IsNullOrEmpty(lootTableCheckListStr))
            {
                lootTableCheckListStr = lootTable.identifier;
            }
            else
            {
                lootTableCheckListStr = $"{lootTableCheckListStr}\n{lootTable.identifier}";
            }
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
