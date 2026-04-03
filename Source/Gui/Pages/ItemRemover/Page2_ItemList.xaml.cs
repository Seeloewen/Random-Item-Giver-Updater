using RandomItemGiverUpdater.Gui.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page2_ItemList : Page, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page2_ItemList(wndRemoveItems wndRemoveItems)
        {
            InitializeComponent();
            this.wndRemoveItems = wndRemoveItems;
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
                MessageBox.Show("Please enter items you want to remove from the datapack to continue!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}
