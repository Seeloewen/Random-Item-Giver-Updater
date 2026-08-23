using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Workspace.Entries;
using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Avalonia.VisualTree;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page3_ItemsAdvanced : UserControl, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page3_ItemsAdvanced()
        {
            InitializeComponent();
            DataContext = this;

            //Setup based on whether datapack uses legacy nbt or not
            tblListHeaderNBT.Text = RIGU.core.currentDatapack.usesLegacyNBT ? "NBT" : "Component";
        }

        public void SetWindow(Wizard wnd)
        {
            wndAddItems = (wndAddItems)wnd;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnAddAdditionalItem_Click(object sender, RoutedEventArgs e)
        {
            RIGU.itemAdding.itemEntries.Add(new AddingEntry("minecraft", "", RIGU.itemAdding.itemEntries.Count));
        }

        private async void btnNbtComponentEditor_Click(object sender, RoutedEventArgs e)
        {
            AddingEntry item = (AddingEntry)((Button)sender).DataContext;

            if (RIGU.core.currentDatapack.usesLegacyNBT)
            {
                //Open the legacy nbt editor
                wndNBTEditor editor = new wndNBTEditor();
                (ModificationState result, string nbt) = await editor.GetFromDialog(item.name, item.GetNBT());
                item.SetNBT(nbt);
            }
            else
            {
                //Open the item stack component editor
                wndComponentEditor editor = new wndComponentEditor();
                (ModificationState result, string component) = await editor.GetFromDialog(item.name, item.GetItemStackComponent());
                item.SetItemStackComponent(component);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Canvas canvas = btn.FindAncestorOfType<Canvas>();

            //Remove the current item from the list
            AddingEntry item = (AddingEntry)canvas.DataContext;
            RIGU.itemAdding.itemEntries.Remove(item);
        }
    }
}
