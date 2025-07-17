using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace RandomItemGiverUpdater.Core
{
    public class LootTable
    {
        public ObservableCollection<Item> items { get; set; } = new ObservableCollection<Item>();
        private JObject frame;

        private BackgroundWorker bgwEditLootTable = new BackgroundWorker();
        private double workerProgress = 0;

        public readonly string name;
        public readonly LootTableCategory category;
        public readonly string path;

        public LootTable(string name, LootTableCategory category, string path)
        {
            this.name = name;
            this.category = category;
            this.path = path;

            bgwEditLootTable.DoWork += bgwEditLootTable_DoWork;
            bgwEditLootTable.ProgressChanged += bgwEditLootTable_ProgressChanged;
            bgwEditLootTable.RunWorkerCompleted += bgwEditLootTable_RunWorkerCompleted;
            bgwEditLootTable.WorkerReportsProgress = true;

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
            //Check for each item if it is modified => if at least one item is modified, the loot table counts as modified
            foreach(Item item in items)
            {
                if(item.IsModified()) return true;
            }

            return false;
        }

        public void Save() => bgwEditLootTable.RunWorkerAsync();
        public string GetIdentifier() => $"{category.name}/{name}";

        private void bgwEditLootTable_DoWork(object sender, DoWorkEventArgs e)
        {
            RIGU.core.wndMain.SetSaveButtonState(false);

            //Reconstruct the full json object by adding the items to the frame
            JArray poolsArray = (JArray)frame.SelectToken("pools[0].entries");
            foreach(Item item in items)
            {
                poolsArray.Add(item.GetItemBodyObject());
            }

            File.WriteAllText(path, frame.ToString());
            poolsArray.Clear (); //Clear the frame again after the adding process
        }

        private void bgwEditLootTable_ProgressChanged(object sender, ProgressChangedEventArgs e) //TODO Probably not even needed anymore
        {
            //Pass the progress to the main window
            workerProgress += (double)e.UserState;
            RIGU.core.wndMain.UpdateEditProgress(workerProgress);
        }

        private void bgwEditLootTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //When the editing is finished, reload the workspace to show changes
            RIGU.core.wndMain.ReloadWorkspace();
            RIGU.core.wndMain.SetSaveButtonState(false);
            MessageBox.Show("Successfully saved the loot table!", "Save Loot Table", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
