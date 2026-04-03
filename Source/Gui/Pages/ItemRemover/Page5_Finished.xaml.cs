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

namespace RandomItemGiverUpdater.Gui.Pages.ItemRemover
{
    public partial class Page5_Finished : Page, IWizardPage
    {
        private wndRemoveItems wndRemoveItems;

        public Page5_Finished(wndRemoveItems wndRemoveItems)
        {
            InitializeComponent();

            this.wndRemoveItems = wndRemoveItems;
        }

        public void Execute()
        {
            foreach (ItemRemovalEntry entry in RIGU.itemRemover.removalEntries)
            {
                tbRemovedItems.AppendText(entry.name + "\n");
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            RIGU.core.wndMain.ReloadWorkspace();
            wndRemoveItems.ShowNextPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => wndRemoveItems.ShowPreviousPage();
    }
}
