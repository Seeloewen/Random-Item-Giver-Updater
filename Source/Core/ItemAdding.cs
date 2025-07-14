using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RandomItemGiverUpdater.Core
{
    public class ItemAdding
    {
        public ObservableCollection<AddingEntry> itemEntries { get; set; } = new ObservableCollection<AddingEntry>();
        private BackgroundWorker bgwAddItems = new BackgroundWorker();

        private wndAddItems wndAddItems;
        private string lootTable;

        public DateTime startTime;
        private double workerProgress;
        private int finishedItems;
        private int finishedLootTables;

        public ItemAdding()
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

        public void AddItems(LootTable lootTable)
        {
            //Get the array of items
            JObject fileObject = JObject.Parse(File.ReadAllText(lootTable));
            JArray items = fileObject.SelectToken("pools[0].entries") as JArray;

            //Create a template item entry and strip the NBT and Components, and possibly the functions part
            string templateString = items[items.Count - 1].ToString();
            Item template = new Item(templateString);
            template.RemoveNbtOrComponentBody();

            foreach (AddingEntry entry in itemEntries)
            {
                if (entry.lootTableWhiteList.Contains(lootTable) || entry.defaultLootTables) //TODO: This still counts as all loot tables, not default ones
                {
                    //Create the new item and add NBT/Item Stack Component if available
                    Item newItem = new Item(template.GetItemBody());
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
            foreach (LootTable lootTable in RIGU.wndMain.lootTableList)
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
