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

namespace RandomItemGiverUpdater.Gui.Pages.DuplicateFinder
{
    public partial class Page1_Start : Page, IWizardPage
    {
        private wndDuplicateFinder wndDuplicateFinder;

        public Page1_Start(wndDuplicateFinder wndDuplicateFinder)
        {
            InitializeComponent();
            this.wndDuplicateFinder = wndDuplicateFinder;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Check if either the datapack or loot table is valid, depending on what's selected
            if(rbtnAll.IsChecked == true && RIGU.core.DatapackIsValid(RIGU.core.currentDatapack) && rbtnCurrent.IsChecked == true && RIGU.core.currentLootTable != null)
            {
                wndDuplicateFinder.ShowNextPage();
                RIGU.duplicateFinder.checkEntireDatapack = rbtnAll.IsChecked;
            }
            else
            {
                MessageBox.Show("Cannot search for duplicates. Please make sure that a datapack or loot table is loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowPreviousPage();
    }
}
