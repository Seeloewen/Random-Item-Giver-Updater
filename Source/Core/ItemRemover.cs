using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Core.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RandomItemGiverUpdater.Core
{
    public class ItemRemover
    {
        private wndRemoveItems wndRemoveItems;
        public ObservableCollection<ItemRemovalEntry> removalEntries { get; set; } = new ObservableCollection<ItemRemovalEntry>();

        private BackgroundWorker bgwRemoveItems = new BackgroundWorker();
        private double workerProgress = 0;
        private int processedItems = 0; //per Loot Table
        private int processedLootTables = 0;

        private List<string> predefinedInput = new List<string>();

        public ItemRemover()
        {
            bgwRemoveItems.WorkerReportsProgress = true;
            bgwRemoveItems.DoWork += bgwRemoveItems_DoWork;
            bgwRemoveItems.RunWorkerCompleted += bgwRemoveItems_RunWorkerCompleted;
            bgwRemoveItems.ProgressChanged += bgwRemoveItems_ProgressChanged;
        }

        public void BeginSession(List<string> predefinedInput = null)
        {
            //Creates links for the current session
            wndRemoveItems = new wndRemoveItems();
            this.predefinedInput = predefinedInput;
            removalEntries.Clear();

            //If the session begins with input predefined, instantly switch to page 2
            if (predefinedInput != null && predefinedInput.Count > 0)
            {
                wndRemoveItems.SkipWithInput(predefinedInput);
            }

            wndRemoveItems.ShowDialog();
        }

        public void Run()
        {
            workerProgress = 0;
            processedLootTables = 0;
            bgwRemoveItems.RunWorkerAsync();
        }

        public void ConstructEntries(string[] items, bool customPrefixes = false)
        {
            //Clear previous content
            removalEntries.Clear();

            //Go through all items to check if a prefix needs to be added
            for (int i = 0; i < items.Length; i++)
            {
                //If custom prefixes is checked but no custom prefix is found OR if custom prefixes is not checked, add the default prefix
                if ((customPrefixes && !items[i].Contains(':')) || !customPrefixes)
                {
                    items[i] = $"minecraft:{items[i]}";
                }
            }

            int j = 0;
            foreach (LootTable lootTable in RIGU.core.currentDatapack.GetLootTables())
            {
                foreach (Item item in lootTable.items)
                {
                    if (items.Contains(item.name))
                    {
                        //Check if the item already has an entry
                        bool isAdded = false;
                        foreach (ItemRemovalEntry entry in removalEntries)
                        {
                            //If it does, add the loot table to the entry
                            if (entry.name == item.name)
                            {
                                entry.UpdateLootTables(lootTable);
                                isAdded = true;
                            }
                        }

                        //If it hasn't already been added, create a new entry and add the loot table
                        if (!isAdded)
                        {
                            ItemRemovalEntry newEntry = new ItemRemovalEntry(item.name, j);
                            newEntry.UpdateLootTables(lootTable);
                            removalEntries.Add(newEntry);
                            j++;
                        }
                    }
                }
            }
        }

        private void bgwRemoveItems_DoWork(object s, DoWorkEventArgs args)
        {
            //Go through each item removal entry and through each loot table in the currently loaded datapack
            foreach (LootTableCategory category in RIGU.core.currentDatapack.lootTableCategories)
            {
                foreach (LootTable lootTable in category.lootTables)
                {
                    processedItems = 0;
                    processedLootTables++;

                    foreach (ItemRemovalEntry entry in removalEntries)
                    {
                        //If the loot table whitelist of the entry contains the loot table, then remove the item from the loot table
                        if (entry.lootTableWhiteList.Contains(lootTable))
                        {
                            lootTable.RemoveItem(entry.name);
                        }

                        //Report worker progress
                        workerProgress += 100 / Convert.ToDouble(removalEntries.Count * RIGU.core.currentDatapack.GetLootTableAmount());
                        bgwRemoveItems.ReportProgress(++processedItems, workerProgress);
                    }

                    lootTable.Save();
                }
            }

        }

        private void bgwRemoveItems_ProgressChanged(object s, ProgressChangedEventArgs progress)
        {
            wndRemoveItems.UpdateProgress((double)progress.UserState, progress.ProgressPercentage, removalEntries.Count, processedLootTables);
        }

        private void bgwRemoveItems_RunWorkerCompleted(object s, RunWorkerCompletedEventArgs args)
        {
            wndRemoveItems.ShowNextPage();
        }
    }
}
