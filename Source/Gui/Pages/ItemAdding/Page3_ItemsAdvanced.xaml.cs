using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page3_ItemsAdvanced : Page, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page3_ItemsAdvanced(wndAddItems wndAddItems)
        {
            InitializeComponent();
            this.wndAddItems = wndAddItems;

            //Setup based on whether datapack uses legacy nbt or not
            tblEditCategories.Text = "  Prefix                                  " +
                                       "Name                                                   "
                                       + (RIGU.wndMain.datapackUsesLegacyNBT ? "NBT" : "Component");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            RIGU.itemAddingCore.itemEntries.Add(new AddingEntry("minecraft", "", RIGU.itemAddingCore.itemEntries.Count));
        }

        private void btnNbtComponentEditor_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            //Get the canvas which the button is in
            Canvas canvas = SeeloewenLib.Tools.FindVisualParent<Canvas>(btn);

            if (canvas.DataContext is AddingEntry item)
            {
                //Remove the current item from the list
                RIGU.itemAddingCore.itemEntries.Remove(item);
            }
        }

        private void tbItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tbItemPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
