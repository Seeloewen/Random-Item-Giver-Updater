using Microsoft.Win32;
using RandomItemGiverUpdater.Core.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace RandomItemGiverUpdater.Core
{
    public class DuplicateFinder
    {
        private int nextIndex = 1; //Used for counting the duplicate entries

        private wndDuplicateFinder wndDuplicateFinder;
        private SaveFileDialog sfdDuplicateList = new SaveFileDialog() { Title = "Export duplicate list...", Filter = "Text (*.txt)|*.txt|All (*.*)|*.*" };

        public ObservableCollection<DuplicateEntry> duplicateEntries { get; set; } = new ObservableCollection<DuplicateEntry>();

        public bool? checkEntireDatapack = true;

        public void DisplayItemRemover()
        {
            //Add the duplicates to a string list
            List<string> duplicates = new List<string>();
            foreach (DuplicateEntry duplicate in duplicateEntries)
            {
                duplicates.Add(duplicate.identifier);
            }

            RIGU.itemRemover.BeginSession(duplicates);
        }

        public void BeginSession()
        {
            //Creates links for the current session
            wndDuplicateFinder = new wndDuplicateFinder();
            wndDuplicateFinder.ShowDialog();
        }

        public void Run(bool entireDatapack)
        {
            duplicateEntries.Clear();

            if (!entireDatapack) //Only check the current loot table
            {
                CheckLootTable(RIGU.core.currentLootTable);
            }
            else //Check all loot tables in the current datapack
            {
                duplicateEntries.Clear();
                nextIndex = 0;
                foreach (LootTable lootTable in RIGU.core.currentDatapack.GetLootTables())
                {
                    CheckLootTable(lootTable);
                }

            }

            MessageBox.Show($"Successfully searched for duplicates. Found {duplicateEntries.Count()} results.", "Search completed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckLootTable(LootTable lootTable)
        {
            HashSet<string> duplicateItems = new HashSet<string>();

            //Construct a list of all items as string
            foreach (Item item in lootTable.items)
            {
                //Add a new entry to the list depending on whether it has nbt or component or not
                if (item.HasTag())
                {
                    duplicateItems.Add($"{item.GetName()};{item.GetTag()}");
                }
                else
                {
                    duplicateItems.Add(item.GetName());
                }
            }

            foreach (string item in duplicateItems)
            {
                bool wasAdded = false;

                //For each item go through every item in the existing duplicate entries. If the item already exists, update the affected loot table
                foreach (DuplicateEntry duplicate in duplicateEntries)
                {
                    //If the item already exists in the duplicate list, update it and stop searching
                    if (duplicate.identifier == item)
                    {
                        duplicate.UpdateLootTables(lootTable.GetIdentifier());
                        duplicate.UpdateAmount();
                        wasAdded = true;
                        break;
                    }
                }

                //Add the item as a new entry if it hasn't been added yet
                if (!wasAdded) duplicateEntries.Add(new DuplicateEntry(item, lootTable.GetIdentifier(), nextIndex++));
            }
        }

        public void Export() //TODO: Could be reworked to export as CSV
        {
            //Create a list of strings and add each entry in the duplicate list as a new line
            List<string> fileConstruct = ["Item Name - Amount - Loot Table(s)"];
            foreach (DuplicateEntry duplicate in duplicateEntries)
            {
                fileConstruct.Add($"{duplicate.identifier} - {duplicate.amount} - {duplicate.lootTables}");
            }

            sfdDuplicateList.FileName = "Random_Item_Giver_Updater_Duplicate_List.txt";
            if (sfdDuplicateList.ShowDialog() == true)
            {
                //Save the duplicate list as file
                File.WriteAllLines(sfdDuplicateList.FileName, fileConstruct);
                MessageBox.Show($"Successfully saved the duplicate list to {sfdDuplicateList.FileName}", "Saved duplicate list", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
