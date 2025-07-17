using System.Linq;

namespace RandomItemGiverUpdater.Core.Entries
{
    public class DuplicateEntry
    {
        public string identifier;
        public string lootTables { get; set; }
        public int amount { get; set; }

        public DuplicateEntry(string identifier, string location, int index)
        {
            this.identifier = identifier;
            lootTables = location;
            amount = 1;
        }

        public void UpdateAmount()
        {
            //Splits the location list and set the amount of different loot tables. Note that items that occur more than twice in loot tables are handled as seperate duplicates
            string[] lootTableSplitted = lootTables.Split(',');
            amount = lootTableSplitted.Count();
            //tblAmount.Text = amount.ToString();
        }

        public void UpdateLootTables(string newLootTable)
        {
            //Add a new loot table to the loot tables string and display it
            lootTables = $"{lootTables}, {newLootTable}";
            //tblLootTables.Text = lootTables;
        }
    }
}
