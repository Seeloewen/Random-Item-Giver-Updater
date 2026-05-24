using System;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page3_RemoveEntries : UserControl, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page3_RemoveEntries()
        {
            InitializeComponent();
        }

        private void btnEditLootTables_Click(object sender, RoutedEventArgs e)
        {
            //Get the item entry from the canvas which the button is in
            Canvas cvsParent = ((Button)sender).FindAncestorOfType<Canvas>();
            RemovalEntry item = (RemovalEntry)cvsParent.DataContext;

            List<LootTableSelectionEntry> entries = wndSelectLootTables.Display(item.lootTableCheckList, "Select the Loot Tables, that you want to add the item to.", wndRemoveItems);

            //Get whitelisted loot tables from loot table selection window
            foreach (LootTableSelectionEntry entry in entries)
            {
                if (entry.isSelected == true)
                {
                    item.lootTableWhiteList.Add(entry.lootTable);
                }
            }
        }

        private void tblLootTables_MouseDown(object sender, PointerPressedEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is RemovalEntry item)
            {
                //Add all loot tables that the item is in to a list and display that list
                string lootTables = item.lootTableCheckListStr;
                //TODO: Avalonia UI
                //MessageBox.Show(lootTables, "List of loot tables", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tblItemName_MouseDown(object sender, PointerPressedEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is RemovalEntry item)
            {
                //Show the full item name in case it's cut off
                //TODO: Avalonia UI
                //MessageBox.Show(item.name, "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RemovalEntry item)
            {
                //Remove the item from the item removal list
                RIGU.itemRemover.removalEntries.Remove(item);
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            bool canContinue = !(RIGU.itemRemover.removalEntries.Count <= 0);

            //Check for each item if the selected loot table amount is valid
            foreach (RemovalEntry entry in RIGU.itemRemover.removalEntries)
            {
                if (entry.lootTableWhiteList.Count <= 0)
                {
                    canContinue = false;
                }
            }

            if (canContinue)
            {
                wndRemoveItems.ShowNextPage();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("No items to remove were found or you have not selected loot tables to remove from!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}