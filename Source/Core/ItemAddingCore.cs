using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RandomItemGiverUpdater.Core
{
    public class ItemAddingCore
    {
        public ObservableCollection<AddingEntry> itemEntries { get; set; } = new ObservableCollection<AddingEntry>();
        private BackgroundWorker bgwAddItems = new BackgroundWorker();

        private wndAddItems wndAddItems;
        private string lootTable;

        public DateTime startTime;
        private double workerProgress;
        private int finishedItems;
        private int finishedLootTables;

        public ItemAddingCore()
        {
            bgwAddItems.WorkerReportsProgress = true;
            bgwAddItems.DoWork += bgwAddItems_DoWork;
            bgwAddItems.ProgressChanged += bgwAddItems_ProgressChanged;
            bgwAddItems.RunWorkerCompleted += bgwAddItems_RunWorkerCompleted;
        }

        public void RunWorker()
        {
            //Reset worker progress
            workerProgress = 0;
            finishedItems = 0;
            finishedLootTables = 0;
            startTime = DateTime.Now;

            //Add the items
            bgwAddItems.RunWorkerAsync();
        }

        public void AddItems(string lootTable)
        {
            //Get the array of items
            JObject fileObject = JObject.Parse(File.ReadAllText(lootTable));
            JArray items = fileObject.SelectToken("pools[0].entries") as JArray;

            //Create a template item entry and strip the NBT and Components, and possibly the functions part
            string templateString = items[items.Count - 1].ToString();
            ItemEntry template = new ItemEntry(templateString);
            template.RemoveNbtOrComponentBody();

            foreach (AddingEntry entry in itemEntries)
            {
                if (entry.lootTableWhiteList.Contains(lootTable) || entry.defaultLootTables)
                {
                    //Create the new item and add NBT/Item Stack Component if available
                    ItemEntry newItem = new ItemEntry(template.GetItemBody());
                    newItem.SetName(entry.id);

                    if (entry.HasNBT()) newItem.SetNBT(entry.GetNBT());
                    if (entry.HasItemStackComponent()) newItem.SetItemStackComponent(entry.GetItemStackComponent());

                    items.Add(newItem.GetItemBodyObject());
                }
            }

            File.WriteAllText(lootTable, fileObject.ToString());
        }

        public void SetActiveEnvironment(wndAddItems wnd, string lootTable)
        {
            wndAddItems = wnd;
            this.lootTable = lootTable;
        }

        private void bgwAddItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args) => wndAddItems.ShowNextPage();

        private void bgwAddItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Go through each loot table and add the items
            foreach (lootTable lootTable in RIGU.wndMain.lootTableList)
            {
                AddItems($"{lootTable.lootTablePath}/{lootTable.lootTableName}");
                finishedLootTables++;
                finishedItems = 0;
            }
        }

        private void bgwAddItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            wndAddItems.UpdateProgress((double)progress.UserState, progress.ProgressPercentage, itemEntries.Count, finishedLootTables);
        }
    }
}
