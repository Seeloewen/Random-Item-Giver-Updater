using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Gui.Menus;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page1_Start : UserControl, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page1_Start()
        {
            InitializeComponent();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowNextPage();

        private void btnBack_Click(object sender,  RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}