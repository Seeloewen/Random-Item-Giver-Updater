using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page6_Finished : UserControl, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page6_Finished()
        {
            InitializeComponent();
        }

        public void Execute()
        {
            //Add all items to the added items list
            foreach (AddingEntry item in RIGU.itemAdding.itemEntries)
            {
                tbAddedItemsList.Text += $"{item.Prefix}:{item.Id}\n";
            }

            //Show the elapsed time
            //tblElapsedTime.Text = $"Elapsed time: {(DateTime.Now - RIGU.itemAddingCore.startTime).ToString(@"hh\:mm\:ss")}"; 
            tblElapsedTime.Text = $"Elapsed time: {DateTime.Now - RIGU.itemAdding.startTime}";
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            RIGU.core.ReloadLootTable();
            wndAddItems.ShowNextPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();
    }
}
