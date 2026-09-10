using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Util;
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

        public void SetWindow(Wizard wnd)
        {
            wndRemoveItems = (wndRemoveItems)wnd;
        }

        private async void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbItems.Text))
            {
                RIGU.itemRemover.ConstructEntries(tbItems.Text.Split('\n'), (bool)cbIncludesCustomPrefixes.IsChecked);
                wndRemoveItems.ShowNextPage();
            }
            else
            {
                await Crossplatform.Dialog(wndRemoveItems, "Please enter items you want to remove from the datapack to continue!", "Error", ButtonEnum.Ok, Icon.Error);
            }
        }
       

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}