using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace RandomItemGiverUpdater.Core
{
    public class LootTable
    {
        public ObservableCollection<Item> items { get; set; } = new ObservableCollection<Item>();
        private JObject frame;

        public readonly string name;
        public readonly LootTableCategory category;
        public readonly string path;

        public LootTable(string name, LootTableCategory category, string path)
        {
            this.name = name;
            this.category = category;
            this.path = path;

            Load();
        }

        public void Load()
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Could not parse the specified loot table: File does not exist");

            //Get the file content as json object
            JObject rootObject = JObject.Parse(File.ReadAllText(path));
            JArray poolsArray = (JArray)rootObject.SelectToken("pools[0].entries");

            //Get all the items
            foreach(JObject entry in poolsArray)
            {
                items.Add(new Item(entry.ToString()));
            }

            //Clear the entries from the frame as it will be reassemabled later
            poolsArray.Clear();
            frame = rootObject;
        }

        public bool IsModified()
        {

        }

        public bool Save()
        {

        }
    }
}
