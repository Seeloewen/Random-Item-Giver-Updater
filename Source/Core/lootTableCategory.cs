using System.Collections.Generic;

namespace RandomItemGiverUpdater.Core
{
    public class LootTableCategory
    {
        public readonly string name;
        public List<LootTable> lootTables = new List<LootTable>();

        public LootTableCategory(string name)
        {
            this.name = name;
        }
    }
}
