using Newtonsoft.Json.Linq;
using RandomItemGiverUpdater.Gui.Menus;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace RandomItemGiverUpdater.Core
{
    public class Main
    {
        public wndMain wndMain;
        //private BackgroundWorker bgwEditLootTable = new BackgroundWorker(); TODO: Move to somewhere else? or get rid of it entirely
        private double workerProgress = 0;

        public Datapack currentDatapack;
        public LootTable currentLootTable;

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

            //Set the current loot table, also as datacontext for the main window, and reload the main window's workspace
            currentLootTable = lootTable;
            wndMain.DataContext = currentLootTable;
            wndMain.ReloadWorkspace();
        }

        private void bgwEditLootTable_DoWork(object sender, DoWorkEventArgs e)
        {
            currentLootTable.Save();
        }

        private void bgwEditLootTable_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Pass the progress to the main window
            workerProgress += (double)e.UserState;
            wndMain.UpdateEditProgress(workerProgress);
        }

        private void bgwEditLootTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //When the editing is finished, reload the workspace to show changes
            wndMain.ReloadWorkspace();
            wndMain.SetSaveButtonState(false);
            MessageBox.Show("Successfully saved the loot table!", "Save Loot Table", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
