using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;

namespace RandomItemGiverUpdater.Core.Data
{
    public class LootTable
    {
        public ObservableCollection<Item> items { get; set; } = new ObservableCollection<Item>();
        private JObject frame;

        public readonly string name;
        public readonly string identifier;
        public readonly string path;

        public LootTable(string name, string identifier, string path)
        {
            this.name = name;
            this.identifier = identifier;
            this.path = path;

            Load();
        }

        public static LootTable Get() => RIGU.core.currentLootTable;

        public void Load()
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Could not parse the specified loot table: File does not exist");

            //Get the file content as json object
            JObject rootObject = JObject.Parse(File.ReadAllText(path));
            JArray poolsArray = (JArray)rootObject.SelectToken("pools[0].entries");

            //Get all the items
            foreach (JObject entry in poolsArray)
            {
                items.Add(new Item(entry.ToString()));
            }

            //Clear the entries from the frame as it will be reassemabled later
            poolsArray.Clear();
            frame = rootObject;
        }

        public bool IsModified()
        {
            //Check for each item if it is modified => if at least one item is modified, the loot table counts as modified
            foreach (Item item in items)
            {
                if (item.IsModified()) return true;
            }

            return false;
        }

        public void AddItem(string name)
        {
            //Something should be here???
        }

        public void RemoveItem(string name)
        {
            //Go through all items and remove the one with the name -- only removes ONE instance of the item
            for (int i = items.Count - 1; i > 0; i--)
            {
                Item item = items[i];
                if (name == item.name)
                {
                    items.Remove(item);
                    break;
                }
            }
        }

        public async Task Save()
        {
            List<Item> copy = items.ToList();

            //Reconstruct the full json object by adding the items to the frame
            JArray poolsArray = (JArray)frame.SelectToken("pools[0].entries");
            foreach (Item item in items)
            {
                if (item.isDeleted) continue;
                poolsArray.Add(item.GetItemBodyObject());
            }

            await File.WriteAllTextAsync(path, frame.ToString());
            poolsArray.Clear(); //Clear the frame again after the adding process

            //Remove items that were deleted
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].isDeleted) items.RemoveAt(i);
            }
        }
    }
}
