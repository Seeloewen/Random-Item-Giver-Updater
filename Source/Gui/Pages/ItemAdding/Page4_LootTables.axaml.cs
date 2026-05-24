using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System.Collections.Generic;
using Avalonia.VisualTree;
using SkiaSharp;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page4_LootTables : UserControl, IWizardPage
    {
        private const string EDIT_LOOTTABLES_BUTTON = "btnEditCertainLootTables";

        private wndAddItems wndAddItems;

        public Page4_LootTables()
        {
            InitializeComponent();
        }

        private void ToggleLootTableSelection(RadioButton rbtn)
        {
            var cvs = rbtn.FindAncestorOfType<Canvas>();
            AddingEntry item = (AddingEntry)cvs.DataContext;

            //Toggle edit button
            Button button = cvs.Find<Button>(EDIT_LOOTTABLES_BUTTON);
            button.IsEnabled = !button.IsEnabled;
            item.defaultLootTables = !item.defaultLootTables;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Check for each item entry if the selection is invalid
            foreach (AddingEntry addItemEntry in RIGU.itemAdding.itemEntries)
            {
                if (!addItemEntry.defaultLootTables && addItemEntry.lootTableWhiteList.Count <= 0)
                {
                    //Avalonia Rework
                    //MessageBox.Show("You have selected to only add the item to certain loot tables without actually selecting any. Please choose any loot tables before continuing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            wndAddItems.ShowNextPage();
        }

        private void btnEditCertainLootTables_Click(object sender, RoutedEventArgs e)
        {
            //Get the item entry from the canvas which the button is in
            Canvas cvsParent = ((Button)sender).FindAncestorOfType<Canvas>();
            AddingEntry item = (AddingEntry)cvsParent.DataContext;

            List<LootTableSelectionEntry> entries = wndSelectLootTables.Display(Datapack.Get().GetLootTables(), "Select the Loot Tables, that you want to add the item to.", wndAddItems);

            //Get whitelisted loot tables from loot table selection window
            foreach (LootTableSelectionEntry entry in entries)
            {
                if (entry.isSelected)
                {
                    item.lootTableWhiteList.Add(entry.lootTable);
                }
            }
        }

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            ToggleLootTableSelection((RadioButton)sender);
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            ToggleLootTableSelection((RadioButton)sender);
        }
    }
}
