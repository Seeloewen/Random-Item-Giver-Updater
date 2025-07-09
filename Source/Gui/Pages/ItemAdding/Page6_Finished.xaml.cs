using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page6_Finished : Page, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page6_Finished(wndAddItems wndAddItems)
        {
            InitializeComponent();
            this.wndAddItems = wndAddItems;
        }

        public void Execute()
        {
            //Add all items to the added items list
            foreach (AddingEntry item in RIGU.itemAddingCore.itemEntries)
            {
                tbAddedItemsList.AppendText($"{item.prefix}:{item.id}\n");
            }

            //Show the elapsed time
            //tblElapsedTime.Text = $"Elapsed time: {(DateTime.Now - RIGU.itemAddingCore.startTime).ToString(@"hh\:mm\:ss")}"; 
            tblElapsedTime.Text = $"Elapsed time: {DateTime.Now - RIGU.itemAddingCore.startTime}";
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();
    }
}
