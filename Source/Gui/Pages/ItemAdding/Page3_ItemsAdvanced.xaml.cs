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

        private void UpdateItem(TextBox textBox)
        {
            AddingEntry item = (AddingEntry)textBox.DataContext;

            //Change the attributes of the item based on the input
            item.id = textBox.Text;
            item.name = $"{item.prefix}:{item.id}";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void tbItemName_TextChanged(object sender, TextChangedEventArgs e) => UpdateItem((TextBox)sender);

        private void tbItemPrefix_TextChanged(object sender, TextChangedEventArgs e) => UpdateItem((TextBox)sender);

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            RIGU.itemAdding.itemEntries.Add(new AddingEntry("minecraft", "", RIGU.itemAdding.itemEntries.Count));
        }

        private void btnNbtComponentEditor_Click(object sender, RoutedEventArgs e)
        {
            AddingEntry item = (AddingEntry)((Button)sender).DataContext;

            if (RIGU.wndMain.datapackUsesLegacyNBT)
            {
                //Open the legacy nbt editor
                wndNBTEditor editor = new wndNBTEditor();
                (ModificationState result, string nbt) = editor.GetFromDialog(item.name, item.GetNBT());
                item.SetNBT(nbt);
            }
            else
            {
                //Open the item stack component editor
                wndComponentEditor editor = new wndComponentEditor();
                (ModificationState result, string component) = editor.GetFromDialog(item.name, item.GetItemStackComponent());
                item.SetItemStackComponent(component);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Canvas canvas = SeeloewenLib.Tools.FindVisualParent<Canvas>(btn);

            //Remove the current item from the list
            AddingEntry item = (AddingEntry)canvas.DataContext;
            RIGU.itemAdding.itemEntries.Remove(item);
        }
    }
}
