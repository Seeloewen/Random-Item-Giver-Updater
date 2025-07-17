using RandomItemGiverUpdater.Core.Entries;
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
    public partial class Page2_Finished : Page, IWizardPage
    {
        private wndDuplicateFinder wndDuplicateFinder;
        private TextBlock tblNoDuplicatesFound = new TextBlock() { Text = "No duplicates were found", Foreground = new SolidColorBrush(Colors.White), FontSize = 24, Visibility = Visibility.Hidden };

        public Page2_Finished(wndDuplicateFinder wndDuplicateFinder)
        {
            InitializeComponent();
            this.wndDuplicateFinder = wndDuplicateFinder;
            cvsContent.Children.Add(tblNoDuplicatesFound);
        }

        public void Execute()
        {            
            RIGU.duplicateFinder.Run((bool)RIGU.duplicateFinder.checkEntireDatapack);
        }

        public void UpdateDuplicateDisplay(int amount)
        {
            //Toggle controls based on whether duplicates were found or not
            if (amount == 0)
            {
                tblNoDuplicatesFound.Visibility = Visibility.Visible;
                tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                btnExportList.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            else
            {
                tblNoDuplicatesFound.Visibility = Visibility.Hidden;
                tblNoDuplicatesFound.Margin = new Thickness(225, 275, 0, 0);
                btnExportList.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowNextPage();

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndDuplicateFinder.ShowPreviousPage();

        private void btnExportList_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
