using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page4_LootTables : Page, IWizardPage
    {
        private wndAddItems wndAddItems;
        private wndSelectLootTables wndSelectLootTables;

        public Page4_LootTables(wndAddItems wndAddItems)
        {
            InitializeComponent();
            this.wndAddItems = wndAddItems;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Check for each item entry if the selection is invalid
            foreach (AddingEntry addItemEntry in RIGU.itemAddingCore.itemEntries)
            {
                if (!addItemEntry.defaultLootTables && addItemEntry.lootTableWhiteList.Count <= 0)
                {
                    MessageBox.Show("You have selected to only add the item to certain loot tables without actually selecting any. Please choose any loot tables before continuing.");
                    return;
                }
            }

            wndAddItems.ShowNextPage();
        }

        private void btnEditCertainLootTables_Click(object sender, RoutedEventArgs e)
        {
            //Get the item entry from the canvas which the button is in
            Canvas cvsParent = SeeloewenLib.Tools.FindVisualParent<Canvas>((Button)sender);
            AddingEntry item = (AddingEntry)cvsParent.DataContext;

            wndSelectLootTables = new wndSelectLootTables(item.lootTableCheckList, "Select the Loot Tables, that you want to add the item to.") { Owner = Application.Current.MainWindow };
            wndSelectLootTables.ShowDialog();

            //Get whitelisted loot tables from loot table selection window
            foreach (lootTable lootTable in wndSelectLootTables.lootTableList)
            {
                if (lootTable.cbAddToLootTable.IsChecked == true)
                {
                    item.lootTableWhiteList.Add(lootTable.fullLootTablePath);
                }
            }
        }

        private void rbtnAllLootTables_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void rbtnCertainLootTables_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
