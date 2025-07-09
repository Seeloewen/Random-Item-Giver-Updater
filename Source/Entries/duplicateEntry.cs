using System.Linq;

namespace RandomItemGiverUpdater
{
    public class duplicateEntry
    {
        //Item attributes
        public string itemName { get; set; }
        public string lootTables { get; set; }
        public int amount { get; set; }


        //-- Constructor --//

        public duplicateEntry(string name, string location, int index)
        {

            //Initialize variables
            itemName = name.TrimEnd('\r', '\n');
            lootTables = location;
            amount = 1;
        }

        //-- Custom Methods --//

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
            lootTables = string.Format("{0}, {1}", lootTables, newLootTable);
            //tblLootTables.Text = lootTables;
        }
    }
}
