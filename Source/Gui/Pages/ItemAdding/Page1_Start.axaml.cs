using Avalonia.Controls;
using Avalonia.Interactivity;
using RandomItemGiverUpdater.Core;
using RandomItemGiverUpdater.Core.Data;
using RandomItemGiverUpdater.Gui.Menus;
using System.Windows;

namespace RandomItemGiverUpdater.Gui.Pages.ItemAdding
{
    public partial class Page1_Start : UserControl, IWizardPage
    {
        private wndAddItems wndAddItems;

        public Page1_Start()
        {
            InitializeComponent();
        }

        public void SetDatapack(Datapack datapack)
        {
            tblCurrentlySelectedDatapack.Text = $"Currently selected Datapack:\n{datapack.rootDirectory}\n{datapack.GetVersionString(datapack.rootDirectory)}";
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowNextPage();

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndAddItems.ShowPreviousPage();
    }
}
