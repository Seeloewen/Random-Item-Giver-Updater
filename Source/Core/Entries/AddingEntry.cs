using RandomItemGiverUpdater.Core;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace RandomItemGiverUpdater
{
    public class AddingEntry : Item
    {
        //Id and prefix are stored seperately to make up the name
        public string id;
        public string prefix;
        public int index { get; set; }

        public List<LootTable> lootTableCheckList = new List<LootTable>();

        //public List<LootTable> lootTableWhiteList = new List<LootTable>();

        public bool defaultLootTables = true;


        //Canvas attributes
        public SolidColorBrush canvasBackColor { get; set; }

        //-- Constructor --//

        public AddingEntry(string prefix, string name, int index) : base(prefix, name.TrimEnd('\r', '\n'))
        {
            //Set the backcolor
            this.index = index;
            canvasBackColor = SetBackColor();

            //Set loot table checklist
            foreach (LootTableCategory category in RIGU.core.currentDatapack.lootTableCategories)
            {
                foreach(LootTable lootTable in category.lootTables)
                {
                    lootTableCheckList.Add(new LootTable(lootTable.name, lootTable.category, lootTable.path));
                }
            }
        }

        public SolidColorBrush SetBackColor()
        {
            //Set the backcolor depending on the item index
            return new SolidColorBrush(Convert.ToInt32(index) % 2 == 0 ? Color.FromArgb(100, 70, 70, 70) : Color.FromArgb(100, 90, 90, 90));
        }
    }
}
