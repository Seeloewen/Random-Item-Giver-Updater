using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Util;
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

        private async void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbItemName.Text))
            {
                RIGU.itemAdding.ConstructEntries(tbItemName.Text.Split('\n'), (bool)cbIncludesPrefixes.IsChecked);
                wndAddItems.ShowNextPage();
            }
            else
            {
                await Crossplatform.Dialog(wndAddItems, "Please enter some items before continuing.", "Error", ButtonEnum.Ok, Icon.Error);
            }
        }
    }
}
