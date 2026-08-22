using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page2_ItemList : UserControl, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page2_ItemList()
        {
            InitializeComponent();
        }

        public void SetWindow(Wizard wnd)
        {
            wndAddItems = (wndAddItems)wnd;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbItemName.Text))
            {
                RIGU.itemAdding.ConstructEntries(tbItemName.Text.Split('\n'), (bool)cbIncludesPrefixes.IsChecked);
                wndAddItems.ShowNextPage();
            }
            else
            {
                //Avalonia Rework here
                //MessageBox.Show("Please enter some items before continuing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
