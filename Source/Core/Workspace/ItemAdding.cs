using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RandomItemGiverUpdater.Core.Workspace
{
    public class ItemAdding
    {
        public ObservableCollection<AddingEntry> itemEntries { get; set; } = new ObservableCollection<AddingEntry>();
        private BackgroundWorker bgwAddItems = new BackgroundWorker();

        private wndAddItems wndAddItems;

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

        public void BeginSession()
        {
            wndAddItems = new wndAddItems();
            wndAddItems.ShowDialog();
        }

        public void ConstructEntries(string[] items, bool customPrefixes = false)
        {
            //Clear previous content
            itemEntries.Clear();

            //Go through all items to check if a prefix needs to be added
            for (int i = 0; i < items.Length; i++)
            {
                //If custom prefixes is checked but no custom prefix is found OR if custom prefixes is not checked, add the default prefix
                if ((customPrefixes && !items[i].Contains(':')) || !customPrefixes)
                {
                    items[i] = $"minecraft:{items[i]}";
                }

                string[] split = items[i].Split(':');

                itemEntries.Add(new AddingEntry(split[0], split[1].TrimEnd('\r', '\n'), i));
            }
        }

        public void AddItems()
        {
            //Reset worker progress
            workerProgress = 0;
            finishedItems = 0;
            finishedLootTables = 0;
            startTime = DateTime.Now;

            //Add the items
            bgwAddItems.RunWorkerAsync();
        }

        public void WriteToLootTable(LootTable lootTable)
        {
            //Get the array of items
            JObject fileObject = JObject.Parse(File.ReadAllText(lootTable.path));
            JArray items = fileObject.SelectToken("pools[0].entries") as JArray;

            //Create a template item entry and strip the NBT and Components, and possibly the functions part
            string templateString = items[items.Count - 1].ToString();
            Item template = new Item(templateString);
            template.RemoveNbtOrComponentBody();

            int i = 1;
            foreach (AddingEntry entry in itemEntries)
            {
                if (entry.lootTableWhiteList.Contains(lootTable) || entry.defaultLootTables) //TODO: This still counts as all loot tables, not default ones
                {
                    //Create the new item and add NBT/Item Stack Component if available
                    Item newItem = new Item(template.GetItemBody());
                    newItem.SetName(entry.name);

                    if (entry.HasNBT()) newItem.SetNBT(entry.GetNBT());
                    if (entry.HasItemStackComponent()) newItem.SetItemStackComponent(entry.GetItemStackComponent());

                    items.Add(newItem.GetItemBodyObject());
                    lootTable.items.Add(newItem);
                }

                bgwAddItems.ReportProgress(i, (double)(finishedLootTables / Datapack.Get().GetLootTableAmount()));
i++;
            }

            File.WriteAllText(lootTable.path, fileObject.ToString());
        }

        private void bgwAddItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args) => wndAddItems.ShowNextPage();

        private void bgwAddItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Go through each loot table and add the items
            //Even when only a few loot tables are selected, the adding process goes through all - items will still only be added to the correct ones
            //This is done to ensure the loot tables are each only saved once as that takes up a lot of resources
            foreach (LootTable lootTable in Datapack.Get().GetLootTables())
            {
                WriteToLootTable(lootTable);
                finishedLootTables++;
                finishedItems = 0;
            }
        }

        private void bgwAddItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            wndAddItems.UpdateProgress((double)(progress.UserState), progress.ProgressPercentage, itemEntries.Count, finishedLootTables);
        }
    }
}
