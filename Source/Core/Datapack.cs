using System.Collections.Generic;
using System.IO;

namespace RandomItemGiverUpdater.Core
{
    public class Datapack
    {
        public List<LootTableCategory> lootTableCategories = new List<LootTableCategory>();

        public string directory;
        public bool usesLegacyNBT = false;
        public bool usesOldFolderStructure = false;

        public Datapack(string path)
        {
            directory = path;

            //Load the datapack
            Load();
        }

        public bool IsValid() => Directory.Exists(directory); //TODO: could probably implement a better check if the datapack is actually valid

        public LootTable GetLootTable(string path)
        {
            //Get the corresponding loot table
            foreach (LootTableCategory category in lootTableCategories)
            {
                foreach(LootTable lootTable in category.lootTables)
                {
                    if (lootTable.path == path) return lootTable;
                }
            }

            return null;
        }

        private void Load()
        {
            //Get the rootpath depending on the version
            string rootPath = "";
            if (Directory.Exists($"{directory}/data/randomitemgiver/loot_tables/")) //1.20 and before
            {
                rootPath = $"{directory}/data/randomitemgiver/loot_tables/";
                usesOldFolderStructure = true;
            }
            else if (Directory.Exists($"{directory}/data/randomitemgiver/loot_table/")) //1.21 and above
            {
                rootPath = $"{directory}/data/randomitemgiver/loot_table/";
            }

            //Get the different loot table categories
            string[] categories = Directory.GetDirectories(rootPath);
            foreach(string category in categories)
            {
                lootTableCategories.Add(new LootTableCategory(category.Replace(rootPath, "")));
            }

            //Get all the loot tables and add them to their categories
            foreach (LootTableCategory category in lootTableCategories)
            {
                string[] lootTables = Directory.GetFiles(rootPath + "/" + category.name);
                foreach (string lootTable in lootTables)
                {
                    string name = lootTable.Replace(rootPath + "/", "");
                    category.lootTables.Add(new LootTable(name, category, lootTable));
                }
            }
        }
    }
}
