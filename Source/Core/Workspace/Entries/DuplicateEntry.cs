using System.Linq;

namespace RandomItemGiverUpdater.Core.Workspace.Entries
{
    public class DuplicateEntry
    {
        public string name { get; set; }
        public string lootTables { get; set; }
        public int amount { get; set; }

        public DuplicateEntry(string name, string location, int index)
        {
            this.name = name;
            lootTables = location;
            amount = 1;
        }

        public void UpdateAmount()
        {
            //Splits the location list and set the amount of different loot tables. Note that items that occur more than twice in loot tables are handled as seperate duplicates
            string[] lootTableSplitted = lootTables.Split(',');
            amount = lootTableSplitted.Count();
        }

        public void UpdateLootTables(string newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables = $"{lootTables}, {newLootTable}";
        }
    }
}
