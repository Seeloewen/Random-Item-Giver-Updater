﻿using RandomItemGiverUpdater.Gui.Menus;
using System.Windows;
using System.Windows.Controls;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page1_Start : Page, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page1_Start(wndAddItems wndAddItems)
        {
            InitializeComponent();

            this.wndAddItems = wndAddItems;
        }

        public void SetDatapack(string datapack)
        {
            tblCurrentlySelectedDatapack.Text = $"Currently selected Datapack: {datapack}\n{RIGU.wndMain.GetDatapackVersionInfo(datapack)}";
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();
    }
}
