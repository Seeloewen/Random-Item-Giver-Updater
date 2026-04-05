using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace RandomItemGiverUpdater.Core.Workspace
{
    public class Main
    {
        public wndMain wndMain;

        public Datapack currentDatapack;
        public LootTable currentLootTable;

        public ObservableCollection<MainEntry> itemEntries { get; set; } = new ObservableCollection<MainEntry>();

        public Main()
        {
            wndMain = new wndMain(this) { DataContext = this };
        }

        public bool DatapackIsValid(Datapack datapack) => datapack != null && datapack.IsValid();

        public void LoadDatapack(string path)
        {
            if ((!string.IsNullOrEmpty(path) && Directory.Exists(path)))
            {
                //If a datapack is currently loaded and has pending modifications, ask the user whether to overwrite them
                if (RIGU.core.currentLootTable != null
                    && RIGU.core.currentLootTable.IsModified()
                    && PromptUnsavedChanges() == MessageBoxResult.No)
                    return;

                currentLootTable = null;
                currentDatapack = new Datapack(path);
            }
            else
            {
                MessageBox.Show("Could not load datapack. Please select a valid datapack folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Exit() //Returns whether to actually close the software
        {
            //Show warning if there are unsaved changes to the loot table
            if (RIGU.core.currentLootTable != null
                && RIGU.core.currentLootTable.IsModified()
                && PromptUnsavedChanges() == MessageBoxResult.No)
                return false;

            return true;
        }

        public MessageBoxResult PromptUnsavedChanges()
        {
            //Prompt the user that the currently loaded loot table has unsaved changes
            MessageBoxResult result =
                MessageBox.Show("You have changes in your current loot table, that have not been saved yet. Exiting this loot table now will discard all unsaved changes. Do you really wish to continue?",
                "Unsaved changes",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result;
        }

        public void SetLootTable(LootTable lootTable)
        {
            //Show warning if there are unsaved changes to the loot table
            if (RIGU.core.currentLootTable != null
                && RIGU.core.currentLootTable.IsModified()
                && PromptUnsavedChanges() == MessageBoxResult.No)
                return;

            currentLootTable = lootTable;

            itemEntries.Clear();
            if (currentLootTable != null)
            {
                int i = 0;
                foreach (Item item in currentLootTable.items) //Create a main entry for each item
                {
                    item.Refresh(); //Overwrite any previous modification markers
                    itemEntries.Add(new MainEntry(item, i));
                    i++;
                }
            }

            wndMain.ReloadWorkspace();
        }

        public void ReloadLootTable()
        {
            itemEntries.Clear();
            if (currentLootTable != null)
            {
                int i = 0;
                foreach (Item item in currentLootTable.items) //Create a main entry for each item
                {
                    item.Refresh(); //Overwrite any previous modification markers
                    itemEntries.Add(new MainEntry(item, i));
                    i++;
                }
            }

            wndMain.ReloadWorkspace();
        }
    }
}