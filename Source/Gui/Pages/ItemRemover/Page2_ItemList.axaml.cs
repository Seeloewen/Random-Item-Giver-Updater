using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page2_ItemList : UserControl, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page2_ItemList()
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbItems.Text))
            {
                RIGU.itemRemover.ConstructEntries(tbItems.Text.Split('\n'), (bool)cbIncludesCustomPrefixes.IsChecked);
                wndRemoveItems.ShowNextPage();
            }
            else
            {
                //TODO: Avalonia Rework
                //MessageBox.Show("Please enter items you want to remove from the datapack to continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}