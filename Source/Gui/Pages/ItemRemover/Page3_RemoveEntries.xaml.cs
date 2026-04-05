using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page3_RemoveEntries : Page, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page3_RemoveEntries(wndRemoveItems wndRemoveItems)
        {
            InitializeComponent();
            this.wndRemoveItems = wndRemoveItems;
        }

        private void btnEditLootTables_Click(object sender, RoutedEventArgs e)
        {
            //Get the item entry from the canvas which the button is in
            Canvas cvsParent = SeeloewenLib.Tools.FindVisualParent<Canvas>((Button)sender);
            RemovalEntry item = (RemovalEntry)cvsParent.DataContext;

            List<LootTableSelectionEntry> entries = wndSelectLootTables.Display(item.lootTableCheckList, "Select the Loot Tables, that you want to add the item to.");

            //Get whitelisted loot tables from loot table selection window
            foreach (LootTableSelectionEntry entry in entries)
            {
                if (entry.isSelected == true)
                {
                    item.lootTableWhiteList.Add(entry.lootTable);
                }
            }
        }

        private void tblLootTables_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is RemovalEntry item)
            {
                //Add all loot tables that the item is in to a list and display that list
                string lootTables = item.lootTableCheckListStr;
                MessageBox.Show(lootTables, "List of loot tables", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tblItemName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textblock && textblock.DataContext is RemovalEntry item)
            {
                //Show the full item name in case it's cut off
                MessageBox.Show(item.name, "Full item name", MessageBoxButton.OK, MessageBoxImage.Information);
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
            bool canContinue = true;

            if (RIGU.itemRemover.removalEntries.Count <= 0) canContinue = false;

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
                MessageBox.Show("No items to remove were found or you have not selected loot tables to remove from!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}
